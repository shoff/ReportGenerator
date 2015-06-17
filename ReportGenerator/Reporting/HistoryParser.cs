﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using log4net;
using Palmmedia.ReportGenerator.Parser.Analysis;
using Palmmedia.ReportGenerator.Properties;

namespace Palmmedia.ReportGenerator.Reporting
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Reads all historic coverage files created by <see cref="HistoryReportGenerator"/> and adds the information to all classes.
    /// </summary>
    public class HistoryParser
    {
        /// <summary>
        /// The Logger.
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(typeof(HistoryParser));

        /// <summary>
        /// The assemblies.
        /// </summary>
        private readonly ICollection<Assembly> assemblies;

        /// <summary>
        /// The history directory.
        /// </summary>
        private readonly string historyDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryParser"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="historyDirectory">The history directory.</param>
        public HistoryParser(ICollection<Assembly> assemblies, string historyDirectory)
        {
            Contract.Requires<ArgumentNullException>(assemblies != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(historyDirectory));
            Contract.Requires<DirectoryNotFoundException>(Directory.Exists(historyDirectory));

            this.assemblies = assemblies;
            this.historyDirectory = historyDirectory;
        }

        /// <summary>
        /// Reads all historic coverage files created by <see cref="HistoryReportGenerator" /> and adds the information to all classes.
        /// </summary>
        /// <exception cref="IOException"><paramref name="path" /> is a file name.</exception>
        public void ApplyHistoricCoverage()
        {
            logger.Info(Resources.ReadingHistoricReports);



            // ReSharper disable once ExceptionNotDocumented
            foreach (var file in Directory.EnumerateFiles(this.historyDirectory, "*_CoverageHistory.xml"))
            {
                try
                {
                    XDocument document = XDocument.Load(file);

                    DateTime date = DateTime.ParseExact(document.Root.Attribute("date").Value, "yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);

                    foreach (var assemblyElement in document.Root.Elements("assembly"))
                    {
                        Assembly assembly = this.assemblies
                            .SingleOrDefault(a => a.Name == assemblyElement.Attribute("name").Value);

                        if (assembly == null)
                        {
                            continue;
                        }

                        foreach (var classElement in assemblyElement.Elements("class"))
                        {
                            Class @class = assembly.Classes
                                .SingleOrDefault(c => c.Name == classElement.Attribute("name").Value);

                            if (@class == null)
                            {
                                continue;
                            }

                            HistoricCoverage historicCoverage = new HistoricCoverage(date)
                            {
                                CoveredLines = int.Parse(classElement.Attribute("coveredlines").Value, CultureInfo.InvariantCulture),
                                CoverableLines = int.Parse(classElement.Attribute("coverablelines").Value, CultureInfo.InvariantCulture),
                                TotalLines = int.Parse(classElement.Attribute("totallines").Value, CultureInfo.InvariantCulture),
                                CoveredBranches = int.Parse(classElement.Attribute("coveredbranches").Value, CultureInfo.InvariantCulture),
                                TotalBranches = int.Parse(classElement.Attribute("totalbranches").Value, CultureInfo.InvariantCulture)
                            };

                            @class.AddHistoricCoverage(historicCoverage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat(" " + Resources.ErrorDuringReadingHistoricReport, file, ex.Message);
                }
            }
        }
    }
}