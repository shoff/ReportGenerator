﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;
using Palmmedia.ReportGenerator.Common;
using Palmmedia.ReportGenerator.Parser.Analysis;
using Palmmedia.ReportGenerator.Properties;

namespace Palmmedia.ReportGenerator.Parser
{
    /// <summary>
    /// Parser for XML reports generated by PartCover 2.2.
    /// </summary>
    public class PartCover22Parser : ParserBase
    {
        /// <summary>
        /// The Logger.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PartCover22Parser));

        /// <summary>
        /// Dictionary containing the file ids by the file's path.
        /// </summary>
        private Dictionary<string, string> fileIdByFilenameDictionary;

        /// <summary>
        /// The type elements of the report.
        /// </summary>
        private XElement[] types;

        /// <summary>
        /// The file elements of the report.
        /// </summary>
        private XElement[] files;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartCover22Parser"/> class.
        /// </summary>
        /// <param name="report">The report file as XContainer.</param>
        public PartCover22Parser(XContainer report)
        {
            if (report == null)
            {
                throw new ArgumentNullException("report");
            }

            this.types = report.Descendants("type").ToArray();
            this.files = report.Descendants("file").ToArray();

            this.fileIdByFilenameDictionary = this.files.ToDictionary(f => f.Attribute("url").Value, f => f.Attribute("id").Value);

            var assemblyNames = this.types
                .Select(type => type.Attribute("asm").Value)
                .Distinct()
                .OrderBy(value => value)
                .ToArray();

            Parallel.ForEach(assemblyNames, assemblyName => this.AddAssembly(this.ProcessAssembly(assemblyName)));

            this.types = null;
            this.files = null;
            this.fileIdByFilenameDictionary = null;
        }

        /// <summary>
        /// Processes the given assembly.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>The <see cref="Assembly"/>.</returns>
        private Assembly ProcessAssembly(string assemblyName)
        {
            Logger.DebugFormat("  " + Resources.CurrentAssembly, assemblyName);

            var classNames = this.types
                .Where(type => type.Attribute("asm").Value.Equals(assemblyName) && !type.Attribute("name").Value.Contains("__"))
                .Select(type => type.Attribute("name").Value)
                .OrderBy(name => name)
                .Distinct()
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
            var fileIdsOfClass = this.types
                .Where(type => type.Attribute("asm").Value.Equals(assembly.Name) 
                    && (type.Attribute("name").Value.Equals(className, StringComparison.Ordinal)
                        || type.Attribute("name").Value.StartsWith(className + "<", StringComparison.Ordinal)))
                .Elements("method")
                .Elements("code")
                .Elements("pt")
                .Where(pt => pt.Attribute("fid") != null)
                .Select(pt => pt.Attribute("fid").Value)
                .Distinct()
                .ToHashSet();

            var filesOfClass = this.files
                .Where(file => fileIdsOfClass.Contains(file.Attribute("id").Value))
                .Select(file => file.Attribute("url").Value)
                .ToArray();

            var @class = new Class(className, assembly);

            foreach (var file in filesOfClass)
            {
                @class.AddFile(this.ProcessFile(@class, file));
            }

            return @class;
        }

        /// <summary>
        /// Processes the file.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>The <see cref="CodeFile"/>.</returns>
        private CodeFile ProcessFile(Class @class, string filePath)
        {
            string fileId = this.fileIdByFilenameDictionary[filePath];

            var seqpntsOfFile = this.types
                .Where(type => type.Attribute("asm").Value.Equals(@class.Assembly.Name) 
                    && (type.Attribute("name").Value.Equals(@class.Name, StringComparison.Ordinal)
                        || type.Attribute("name").Value.StartsWith(@class.Name + "<", StringComparison.Ordinal)))
                .Elements("method")
                .Elements("code")
                .Elements("pt")
                .Where(seqpnt => seqpnt.HasAttributeWithValue("fid", fileId))
                .Select(seqpnt => new
                {
                    LineNumberStart = int.Parse(seqpnt.Attribute("sl").Value, CultureInfo.InvariantCulture),
                    LineNumberEnd = seqpnt.Attribute("el") != null ? int.Parse(seqpnt.Attribute("el").Value, CultureInfo.InvariantCulture) : int.Parse(seqpnt.Attribute("sl").Value, CultureInfo.InvariantCulture),
                    Visits = int.Parse(seqpnt.Attribute("visit").Value, CultureInfo.InvariantCulture)
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
                        coverage[lineNumber] = coverage[lineNumber] == -1 ? seqpnt.Visits : coverage[lineNumber] + seqpnt.Visits;
                    }
                }
            }

            return new CodeFile(filePath, coverage);
        }
    }
}
