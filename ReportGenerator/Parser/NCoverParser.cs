﻿using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;
using Palmmedia.ReportGenerator.Parser.Analysis;
using Palmmedia.ReportGenerator.Properties;

namespace Palmmedia.ReportGenerator.Parser
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Parser for XML reports generated by NCover.
    /// </summary>
    public class NCoverParser : ParserBase
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(NCoverParser));
        private readonly XElement[] modules;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="NCoverParser"/> class.
        /// </summary>
        /// <param name="report">The report file as XContainer.</param>
        public NCoverParser(XContainer report)
        {
            Contract.Requires<ArgumentNullException>(report != null);
            this.modules = report.Descendants("module").ToArray();

            var assemblyNames = this.modules
                .Select(module => module.Attribute("assembly").Value)
                .Distinct()
                .OrderBy(a => a)
                .ToArray();

            //Parallel.ForEach(assemblyNames, assemblyName => this.AddAssembly(this.ProcessAssembly(assemblyName)));
            foreach (var assemblyName in assemblyNames)
            {
                var processedAssembly = this.ProcessAssembly(assemblyName);
                this.AddAssembly(processedAssembly);
            }
            this.modules = null;
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
                .Where(module => module.Attribute("assembly").Value.Equals(assemblyName))
                .Elements("method")
                .Where(m => m.Attribute("excluded").Value == "false")
                .Select(method => method.Attribute("class").Value)
                .Where(value => !value.Contains("__") && !value.Contains("+"))
                .Distinct()
                .OrderBy(name => name)
                .ToArray();

            var assembly = new Assembly(assemblyName);

            // Parallel.ForEach(classNames, className => assembly.AddClass(this.ProcessClass(assembly, className)));
            foreach (var className in classNames)
            {
                var processedClass = this.ProcessClass(assembly, className);
                assembly.AddClass(processedClass);
            }
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
            var filesOfClass = this.modules
                .Where(module => module.Attribute("assembly").Value.Equals(assembly.Name)).Elements("method")
                .Where(method => method.Attribute("class").Value.Equals(className))
                .Where(m => m.Attribute("excluded").Value == "false")
                .Elements("seqpnt").Select(seqpnt => seqpnt.Attribute("document").Value)
                .Distinct()
                .ToArray();

            var processClass = new Class(className, assembly);

            foreach (var file in filesOfClass)
            {
                processClass.AddFile(this.ProcessFile(processClass, file));
            }

            return processClass;
        }

        /// <summary>
        /// Processes the file.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>The <see cref="CodeFile"/>.</returns>
        private CodeFile ProcessFile(Class @class, string filePath)
        {
            var seqpntsOfFile = this.modules
                .Where(type => type.Attribute("assembly").Value.Equals(@class.Assembly.Name))
                .Elements("method")
                .Where(m => m.Attribute("excluded").Value == "false")
                .Where(method => method.Attribute("class").Value.StartsWith(@class.Name, StringComparison.Ordinal))
                .Elements("seqpnt")
                .Where(seqpnt => seqpnt.Attribute("document").Value.Equals(filePath) && seqpnt.Attribute("line").Value != "16707566")
                .Select(seqpnt => new
                {
                    LineNumberStart = int.Parse(seqpnt.Attribute("line").Value, CultureInfo.InvariantCulture),
                    LineNumberEnd = int.Parse(seqpnt.Attribute("endline").Value, CultureInfo.InvariantCulture),
                    Visits = int.Parse(seqpnt.Attribute("visitcount").Value, CultureInfo.InvariantCulture)
                })
                .OrderBy(seqpnt => seqpnt.LineNumberEnd)
                .ToArray();

            int[] coverage = new int[] { };

            if (seqpntsOfFile.Length > 0)
            {
                coverage = new int[seqpntsOfFile[seqpntsOfFile.LongLength - 1].LineNumberEnd + 1];

                for (int i = 0; i < coverage.Length; i++)
                {
                    coverage[i] = -1;
                }

                foreach (var seqpnt in seqpntsOfFile)
                {
                    for (int lineNumber = seqpnt.LineNumberStart; lineNumber <= seqpnt.LineNumberEnd; lineNumber++)
                    {
                        coverage[seqpnt.LineNumberStart] = coverage[seqpnt.LineNumberStart] == -1 ? seqpnt.Visits : coverage[seqpnt.LineNumberStart] + seqpnt.Visits;
                    }
                }
            }

            return new CodeFile(filePath, coverage);
        }
    }
}
