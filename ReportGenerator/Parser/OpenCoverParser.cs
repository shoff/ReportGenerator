using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;
using Palmmedia.ReportGenerator.Common;
using Palmmedia.ReportGenerator.Parser.Analysis;
using Palmmedia.ReportGenerator.Properties;

namespace Palmmedia.ReportGenerator.Parser
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Parser for XML reports generated by OpenCover.
    /// </summary>
    public class OpenCoverParser : ParserBase
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(OpenCoverParser));
        private readonly XElement[] modules;
        private readonly XElement[] files;
        private readonly IDictionary<string, string> trackedMethods;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenCoverParser"/> class.
        /// </summary>
        /// <param name="report">The report file as XContainer.</param>
        public OpenCoverParser(XContainer report)
        {
            Contract.Requires<ArgumentNullException>(report != null);

            this.modules = report.Descendants("Module")
                .Where(m => m.Attribute("skippedDueTo") == null)
                .ToArray();
            this.files = report.Descendants("File").ToArray();
            this.trackedMethods = report.Descendants("TrackedMethod")
                .ToDictionary(t => t.Attribute("uid").Value, t => t.Attribute("name").Value);

            var assemblyNames = this.modules
                .Select(m => m.Element("ModuleName").Value)
                .Distinct()
                .OrderBy(a => a)
                .ToArray();

            Parallel.ForEach(assemblyNames, assemblyName => this.AddAssembly(this.ProcessAssembly(assemblyName)));

            this.modules = null;
            this.files = null;
            this.trackedMethods = null;
        }

        /// <summary>
        /// Extracts the metrics from the given <see cref="XElement">XElements</see>.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <param name="class">The class.</param>
        private static void SetMethodMetrics(IEnumerable<XElement> methods, Class @class)
        {
            foreach (var methodGroup in methods.GroupBy(m => m.Element("Name").Value))
            {
                var method = methodGroup.First();

                // Exclude properties and lambda expressions
                if (method.Attribute("skippedDueTo") != null
                    || method.HasAttributeWithValue("isGetter", "true")
                    || method.HasAttributeWithValue("isSetter", "true")
                    || Regex.IsMatch(methodGroup.Key, "::<.+>.+__"))
                {
                    continue;
                }

                var metrics = new[] 
                { 
                    new Metric(
                        "Cyclomatic Complexity", 
                        methodGroup.Max(m => int.Parse(m.Attribute("cyclomaticComplexity").Value, CultureInfo.InvariantCulture))),
                    new Metric(
                        "Sequence Coverage", 
                        methodGroup.Max(m => decimal.Parse(m.Attribute("sequenceCoverage").Value, CultureInfo.InvariantCulture))),
                    new Metric(
                        "Branch Coverage", 
                        methodGroup.Max(m => decimal.Parse(m.Attribute("branchCoverage").Value, CultureInfo.InvariantCulture)))
                };

                @class.AddMethodMetric(new MethodMetric(methodGroup.Key, metrics));
            }
        }

        /// <summary>
        /// Gets the branches by line number.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <param name="fileIds">The file ids of the class.</param>
        /// <returns>The branches by line number.</returns>
        private static Dictionary<int, List<Branch>> GetBranches(XElement[] methods, HashSet<string> fileIds)
        {
            var result = new Dictionary<int, List<Branch>>();

            var branchPoints = methods
                .Elements("BranchPoints")
                .Elements("BranchPoint")
                .ToArray();

            // OpenCover supports this since version 4.5.3207
            if (branchPoints.Length == 0 || branchPoints[0].Attribute("sl") == null)
            {
                return result;
            }

            foreach (var branchPoint in branchPoints)
            {
                if (branchPoint.Attribute("fileid") != null
                    && !fileIds.Contains(branchPoint.Attribute("fileid").Value))
                {
                    // If fileid is available, verify that branch belongs to same file (available since version OpenCover.4.5.3418)
                    continue;
                }

                int lineNumber = int.Parse(branchPoint.Attribute("sl").Value, CultureInfo.InvariantCulture);

                string identifier = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}_{1}_{2}_{3}",
                    lineNumber,
                    branchPoint.Attribute("path").Value,
                    branchPoint.Attribute("offset").Value,
                    branchPoint.Attribute("offsetend").Value);

                var branch = new Branch(
                    int.Parse(branchPoint.Attribute("vc").Value, CultureInfo.InvariantCulture),
                    identifier);

                List<Branch> branches = null;
                if (result.TryGetValue(lineNumber, out branches))
                {
                    branches.Add(branch);
                }
                else
                {
                    branches = new List<Branch>();
                    branches.Add(branch);

                    result.Add(lineNumber, branches);
                }
            }

            return result;
        }

        /// <summary>
        /// Processes the given assembly.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>The <see cref="Assembly"/>.</returns>
        private Assembly ProcessAssembly(string assemblyName)
        {
            logger.DebugFormat("  " + Resources.CurrentAssembly, assemblyName);

            var fileIdsByFilename = this.modules
                .Where(m => m.Element("ModuleName").Value.Equals(assemblyName))
                .Elements("Files")
                .Elements("File")
                .GroupBy(f => f.Attribute("fullPath").Value, f => f.Attribute("uid").Value)
                .ToDictionary(g => g.Key, g => g.ToHashSet());

            var classNames = this.modules
                .Where(m => m.Element("ModuleName").Value.Equals(assemblyName))
                .Elements("Classes")
                .Elements("Class")
                .Where(c => !c.Element("FullName").Value.Contains("__")
                    && !c.Element("FullName").Value.Contains("<")
                    && c.Attribute("skippedDueTo") == null)
                .Select(c =>
                    {
                        string fullname = c.Element("FullName").Value;
                        int nestedClassSeparatorIndex = fullname.IndexOf('/');
                        return nestedClassSeparatorIndex > -1 ? fullname.Substring(0, nestedClassSeparatorIndex) : fullname;
                    })
                .Distinct()
                .OrderBy(name => name)
                .ToArray();

            var assembly = new Assembly(assemblyName);

            Parallel.ForEach(classNames, className => assembly.AddClass(this.ProcessClass(fileIdsByFilename, assembly, className)));

            return assembly;
        }

        /// <summary>
        /// Processes the given class.
        /// </summary>
        /// <param name="fileIdsByFilename">Dictionary containing the file ids by filename.</param>
        /// <param name="assembly">The assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <returns>The <see cref="Class"/>.</returns>
        private Class ProcessClass(Dictionary<string, HashSet<string>> fileIdsByFilename, Assembly assembly, string className)
        {
            var methods = this.modules
                .Where(m => m.Element("ModuleName").Value.Equals(assembly.Name))
                .Elements("Classes")
                .Elements("Class")
                .Where(c => c.Element("FullName").Value.Equals(className)
                            || c.Element("FullName").Value.StartsWith(className + "/", StringComparison.Ordinal))
                .Elements("Methods")
                .Elements("Method");

            var fileIdsOfClassInSequencePoints = methods
                .Elements("SequencePoints")
                .Elements("SequencePoint")
                .Where(seqpnt => seqpnt.Attribute("fileid") != null
                                 && seqpnt.Attribute("fileid").Value != "0")
                .Select(seqpnt => seqpnt.Attribute("fileid").Value)
                .ToArray();

            // Only required for backwards compatibility, older versions of OpenCover did not apply fileid for partial classes
            var fileIdsOfClassInFileRef = methods
                .Where(m => m.Element("FileRef") != null)
                .Select(m => m.Element("FileRef").Attribute("uid").Value)
                .ToArray();

            var fileIdsOfClass = fileIdsOfClassInSequencePoints
                .Concat(fileIdsOfClassInFileRef)
                .Distinct()
                .ToHashSet();

            var filesOfClass = this.files
                .Where(file => fileIdsOfClass.Contains(file.Attribute("uid").Value))
                .Select(file => file.Attribute("fullPath").Value)
                .Distinct()
                .ToArray();

            var @class = new Class(className, assembly);

            foreach (var file in filesOfClass)
            {
                @class.AddFile(this.ProcessFile(fileIdsByFilename[file], @class, file));
            }

            @class.CoverageQuota = this.GetCoverageQuotaOfClass(assembly, className);

            return @class;
        }

        /// <summary>
        /// Processes the file.
        /// </summary>
        /// <param name="fileIds">The file ids of the class.</param>
        /// <param name="class">The class.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>The <see cref="CodeFile"/>.</returns>
        private CodeFile ProcessFile(HashSet<string> fileIds, Class @class, string filePath)
        {
            var methods = this.modules
                .Where(m => m.Element("ModuleName").Value.Equals(@class.Assembly.Name))
                .Elements("Classes")
                .Elements("Class")
                .Where(c => c.Element("FullName").Value.Equals(@class.Name)
                            || c.Element("FullName").Value.StartsWith(@class.Name + "/", StringComparison.Ordinal))
                .Elements("Methods")
                .Elements("Method")
                .ToArray();

            var methodsOfFile = methods
                .Where(m => m.Element("FileRef") != null && fileIds.Contains(m.Element("FileRef").Attribute("uid").Value))
                .ToArray();

            SetMethodMetrics(methodsOfFile, @class);

            var seqpntsOfFile = methods
                .Elements("SequencePoints")
                .Elements("SequencePoint")
                .Where(seqpnt => (seqpnt.Attribute("fileid") != null
                                    && fileIds.Contains(seqpnt.Attribute("fileid").Value))
                    || (seqpnt.Attribute("fileid") == null && seqpnt.Parent.Parent.Element("FileRef") != null
                        && fileIds.Contains(seqpnt.Parent.Parent.Element("FileRef").Attribute("uid").Value)))
                .Select(seqpnt => new
                {
                    LineNumberStart = int.Parse(seqpnt.Attribute("sl").Value, CultureInfo.InvariantCulture),
                    LineNumberEnd = seqpnt.Attribute("el") != null ? int.Parse(seqpnt.Attribute("el").Value, CultureInfo.InvariantCulture) : int.Parse(seqpnt.Attribute("sl").Value, CultureInfo.InvariantCulture),
                    Visits = int.Parse(seqpnt.Attribute("vc").Value, CultureInfo.InvariantCulture),
                    TrackedMethodRefs = seqpnt.Elements("TrackedMethodRefs")
                        .Elements("TrackedMethodRef")
                        .Select(t => new
                        {
                            Visits = int.Parse(t.Attribute("vc").Value, CultureInfo.InvariantCulture),
                            TrackedMethodId = t.Attribute("uid").Value
                        })
                })
                .OrderBy(seqpnt => seqpnt.LineNumberEnd)
                .ToArray();

            int[] coverage = new int[] { };
            var branches = GetBranches(methods, fileIds);

            var trackedMethodsCoverage = seqpntsOfFile
                .SelectMany(s => s.TrackedMethodRefs)
                .Select(t => t.TrackedMethodId)
                .Distinct()
                .ToDictionary(id => id, id => new int[] { });

            if (seqpntsOfFile.Length > 0)
            {
                coverage = new int[seqpntsOfFile[seqpntsOfFile.LongLength - 1].LineNumberEnd + 1];

                for (int i = 0; i < coverage.Length; i++)
                {
                    coverage[i] = -1;
                }

                foreach (var name in trackedMethodsCoverage.Keys.ToArray())
                {
                    trackedMethodsCoverage[name] = (int[])coverage.Clone();
                }

                foreach (var seqpnt in seqpntsOfFile)
                {
                    for (int lineNumber = seqpnt.LineNumberStart; lineNumber <= seqpnt.LineNumberEnd; lineNumber++)
                    {
                        int visits = coverage[lineNumber] == -1 ? seqpnt.Visits : coverage[lineNumber] + seqpnt.Visits;
                        coverage[lineNumber] = visits;

                        if (visits > -1)
                        {
                            foreach (var trackedMethodCoverage in trackedMethodsCoverage)
                            {
                                if (trackedMethodCoverage.Value[lineNumber] == -1)
                                {
                                    trackedMethodCoverage.Value[lineNumber] = 0;
                                }
                            }
                        }

                        foreach (var trackedMethod in seqpnt.TrackedMethodRefs)
                        {
                            var trackedMethodCoverage = trackedMethodsCoverage[trackedMethod.TrackedMethodId];
                            trackedMethodCoverage[lineNumber] = trackedMethodCoverage[lineNumber] == -1 ? trackedMethod.Visits : trackedMethodCoverage[lineNumber] + trackedMethod.Visits;
                        }
                    }
                }
            }

            var codeFile = new CodeFile(filePath, coverage, branches);

            foreach (var trackedMethodCoverage in trackedMethodsCoverage)
            {
                string name = null;

                // Sometimes no corresponding MethodRef element exists
                if (this.trackedMethods.TryGetValue(trackedMethodCoverage.Key, out name))
                {
                    string shortName = name.Substring(name.Substring(0, name.IndexOf(':') + 1).LastIndexOf('.') + 1);
                    TestMethod testMethod = new TestMethod(name, shortName);
                    codeFile.AddCoverageByTestMethod(testMethod, trackedMethodCoverage.Value);
                }
            }

            return codeFile;
        }

        /// <summary>
        /// Gets the coverage quota of a class.
        /// This method is used to get coverage quota if line coverage is not available.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <returns>The coverage quota.</returns>
        private decimal? GetCoverageQuotaOfClass(Assembly assembly, string className)
        {
            var methodGroups = this.modules
                .Where(m => m.Element("ModuleName").Value.Equals(assembly.Name))
                .Elements("Classes")
                .Elements("Class")
                .Where(c => c.Element("FullName").Value.Equals(className)
                            || c.Element("FullName").Value.StartsWith(className + "/", StringComparison.Ordinal))
                .Elements("Methods")
                .Elements("Method")
                .Where(m => m.Attribute("skippedDueTo") == null && m.Element("FileRef") == null && !m.Element("Name").Value.EndsWith(".ctor()", StringComparison.OrdinalIgnoreCase))
                .GroupBy(m => m.Element("Name").Value)
                .ToArray();

            int visitedMethods = methodGroups.Count(g => g.Any(m => m.Attribute("visited").Value == "true"));

            return (methodGroups.Length == 0) ? (decimal?)null : (decimal)Math.Truncate(1000 * (double)visitedMethods / (double)methodGroups.Length) / 10;
        }
    }
}
