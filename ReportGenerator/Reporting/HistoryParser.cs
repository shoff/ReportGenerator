namespace Palmmedia.ReportGenerator.Reporting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Xml.Linq;
    using log4net;
    using Palmmedia.ReportGenerator.Parser.Analysis;
    using Palmmedia.ReportGenerator.Properties;

    /// <summary>
    ///   Reads all historic coverage files created by <see cref="HistoryReportGenerator" /> and adds the information to all
    ///   classes.
    /// </summary>
    public class HistoryParser
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(HistoryParser));
        private readonly ICollection<Assembly> assemblies;
        private readonly string historyDirectory;

        /// <summary>
        ///   Initializes a new instance of the <see cref="HistoryParser" /> class.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="historyDirectory">The history directory.</param>
        [Pure]
        public HistoryParser(ICollection<Assembly> assemblies, string historyDirectory)
        {
            Contract.Requires<ArgumentNullException>(assemblies != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(historyDirectory));
            Contract.Requires<DirectoryNotFoundException>(Directory.Exists(historyDirectory));

            this.assemblies = assemblies;
            this.historyDirectory = historyDirectory;
        }

        /// <summary>
        ///   Reads all historic coverage files created by <see cref="HistoryReportGenerator" /> and adds the information to
        ///   all classes.
        /// </summary>
        /// <exception cref="IOException">
        ///   <paramref>
        ///     <name>path</name>
        ///   </paramref>
        ///   is a file name.
        /// </exception>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="ArgumentNullException"><paramref>
        ///     <name>s</name>
        ///   </paramref>
        ///   is null. </exception>
        public void ApplyHistoricCoverage()
        {
            logger.Info(Resources.ReadingHistoricReports);

            // ReSharper disable once ExceptionNotDocumented
            foreach (var file in Directory.EnumerateFiles(this.historyDirectory, "*_CoverageHistory.xml"))
            {
                //try
                //{
                var document = XDocument.Load(file);

                if (document.Root != null)
                {
                    var date = DateTime.ParseExact(document.Root.Attribute("date").Value, "yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);

                    foreach (var assemblyElement in document.Root.Elements("assembly"))
                    {
                        var assembly = this.assemblies.SingleOrDefault(a => a.Name == assemblyElement.Attribute("name").Value);

                        if (assembly == null)
                        {
                            continue;
                        }

                        foreach (var classElement in assemblyElement.Elements("class"))
                        {
                            var classType = assembly.Classes.SingleOrDefault(c => c.Name == classElement.Attribute("name").Value);

                            if (classType == null)
                            {
                                continue;
                            }

                            var historicCoverage = new HistoricCoverage(date);

                            int workingInt;

                            int.TryParse(classElement.Attribute("coveredlines").Value, out workingInt);
                            historicCoverage.CoveredLines = workingInt;

                            int.TryParse(classElement.Attribute("coverablelines").Value, out workingInt);
                            historicCoverage.CoverableLines = workingInt;

                            int.TryParse(classElement.Attribute("totallines").Value, out workingInt);
                            historicCoverage.TotalLines = workingInt;

                            int.TryParse(classElement.Attribute("coveredbranches").Value, out workingInt);
                            historicCoverage.CoveredBranches = workingInt;

                            int.TryParse(classElement.Attribute("totalbranches").Value, out workingInt);
                            historicCoverage.TotalBranches = workingInt;

                            classType.AddHistoricCoverage(historicCoverage);
                        }
                    }
                }
            }
        }
    }
}