using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;
using Palmmedia.ReportGenerator.Parser.Analysis;
using Palmmedia.ReportGenerator.Properties;

namespace Palmmedia.ReportGenerator.Parser
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Parser for XML reports generated by Visual Studio.
    /// </summary>
    public class VisualStudioParser : ParserBase
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(VisualStudioParser));
        private readonly XElement[] modules;
        private readonly XElement[] files;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualStudioParser"/> class.
        /// </summary>
        /// <param name="report">The report file as XContainer.</param>
        public VisualStudioParser(XContainer report)
        {
            Contract.Requires<ArgumentNullException>(report != null);

            this.modules = report.Descendants("Module").ToArray();
            this.files = report.Descendants("SourceFileNames").ToArray();

            var assemblyNames = this.modules
                .Select(m => m.Element("ModuleName").Value)
                .Distinct()
                .OrderBy(a => a)
                .ToArray();

            Parallel.ForEach(assemblyNames, assemblyName => this.AddAssembly(this.ProcessAssembly(assemblyName)));

            this.modules = null;
            this.files = null;
        }

        /// <summary>
        /// Extracts the metrics from the given <see cref="XElement">XElements</see>.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <param name="class">The class.</param>
        private static void SetMethodMetrics(IEnumerable<XElement> methods, Class @class)
        {
            foreach (var method in methods)
            {
                string methodName = method.Element("MethodName").Value;

                // Exclude properties and lambda expressions
                if (methodName.StartsWith("get_", StringComparison.Ordinal)
                    || methodName.StartsWith("set_", StringComparison.Ordinal)
                    || Regex.IsMatch(methodName, "<.+>.+__"))
                {
                    continue;
                }

                var metrics = new[] 
                {
                    new Metric(
                        "Blocks covered", 
                        int.Parse(method.Element("BlocksCovered").Value, CultureInfo.InvariantCulture)),
                    new Metric(
                        "Blocks not covered", 
                        int.Parse(method.Element("BlocksNotCovered").Value, CultureInfo.InvariantCulture))
                };

                @class.AddMethodMetric(new MethodMetric(methodName, metrics));
            }
        }

        /// <summary>
        /// Processes the given assembly.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>The <see cref="Assembly"/>.</returns>
        private Assembly ProcessAssembly(string assemblyName)
        {
            logger.DebugFormat("  " + Resources.CurrentAssembly, assemblyName);

            var classNames = this.modules
                .Where(m => m.Element("ModuleName").Value.Equals(assemblyName))
                .Elements("NamespaceTable")
                .Elements("Class")
                .Elements("ClassName")
                .Where(c => !c.Value.Contains("__")
                    && !c.Value.Contains("<>")
                    && !c.Value.Contains(".")
                    && !c.Value.StartsWith("$", StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Parent.Parent.Element("NamespaceName").Value + "." + c.Value)
                .Distinct()
                .OrderBy(name => name)
                .ToArray();

            var assembly = new Assembly(assemblyName);

            Parallel.ForEach(classNames, className => assembly.AddClass(this.ProcessClass(assembly, className)));

            return assembly;
        }

        /// <summary>
        /// Processes the given class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <returns>The <see cref="Class"/>.</returns>
        private Class ProcessClass(Assembly assembly, string className)
        {
            var fileIdsOfClass = this.modules
                .Where(m => m.Element("ModuleName").Value.Equals(assembly.Name))
                .Elements("NamespaceTable")
                .Elements("Class")
                .Where(c => (c.Parent.Element("NamespaceName").Value + "." + c.Element("ClassName").Value).Equals(className))
                .Elements("Method")
                .Elements("Lines")
                .Elements("SourceFileID")
                .Select(m => m.Value)
                .Distinct();

            var @class = new Class(className, assembly);

            foreach (var fileId in fileIdsOfClass)
            {
                string file = this.files.First(f => f.Element("SourceFileID").Value == fileId).Element("SourceFileName").Value;
                @class.AddFile(this.ProcessFile(fileId, @class, file));
            }

            return @class;
        }

        /// <summary>
        /// Processes the file.
        /// </summary>
        /// <param name="fileId">The file id.</param>
        /// <param name="class">The class.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>The <see cref="CodeFile"/>.</returns>
        private CodeFile ProcessFile(string fileId, Class @class, string filePath)
        {
            var methods = this.modules
                .Where(m => m.Element("ModuleName").Value.Equals(@class.Assembly.Name))
                .Elements("NamespaceTable")
                .Elements("Class")
                .Where(c => (c.Parent.Element("NamespaceName").Value + "." + c.Element("ClassName").Value).Equals(@class.Name, StringComparison.Ordinal)
                            || (c.Parent.Element("NamespaceName").Value + "." + c.Element("ClassName").Value).StartsWith(@class.Name + ".", StringComparison.Ordinal))
                .Elements("Method")
                .Where(m => m.Elements("Lines").Elements("SourceFileID").Any(s => s.Value == fileId))
                .ToArray();

            SetMethodMetrics(methods, @class);

            var linesOfFile = methods
                .Elements("Lines")
                .Select(l => new
                {
                    LineNumberStart = int.Parse(l.Element("LnStart").Value, CultureInfo.InvariantCulture),
                    LineNumberEnd = int.Parse(l.Element("LnEnd").Value, CultureInfo.InvariantCulture),
                    Coverage = int.Parse(l.Element("Coverage").Value, CultureInfo.InvariantCulture)
                })
                .OrderBy(seqpnt => seqpnt.LineNumberEnd)
                .ToArray();

            int[] coverage = new int[] { };

            if (linesOfFile.Length > 0)
            {
                coverage = new int[linesOfFile[linesOfFile.LongLength - 1].LineNumberEnd + 1];

                for (int i = 0; i < coverage.Length; i++)
                {
                    coverage[i] = -1;
                }

                foreach (var seqpnt in linesOfFile)
                {
                    for (int lineNumber = seqpnt.LineNumberStart; lineNumber <= seqpnt.LineNumberEnd; lineNumber++)
                    {
                        int visits = seqpnt.Coverage < 2 ? 1 : 0;
                        coverage[lineNumber] = coverage[lineNumber] == -1 ? visits : Math.Min(coverage[lineNumber] + visits, 1);
                    }
                }
            }

            return new CodeFile(filePath, coverage);
        }
    }
}
