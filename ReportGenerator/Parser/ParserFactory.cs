namespace Palmmedia.ReportGenerator.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Xml.Linq;
    using log4net;
    using Palmmedia.ReportGenerator.Parser.Preprocessing;
    using Palmmedia.ReportGenerator.Parser.Preprocessing.FileSearch;
    using Palmmedia.ReportGenerator.Properties;

    /// <summary>
    ///   Initiates the corresponding parser to the given report file.
    /// </summary>
    public static class ParserFactory
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ParserFactory));

        /// <summary>
        ///   Tries to initiate the correct parsers for the given reports.
        /// </summary>
        /// <param name="reportFiles">The report files to parse.</param>
        /// <param name="sourceDirectories">The source directories.</param>
        /// <returns>
        ///   The IParser instance.
        /// </returns>
        public static IParser CreateParser(IEnumerable<string> reportFiles, IEnumerable<string> sourceDirectories)
        {
            Contract.Requires<ArgumentNullException>(reportFiles != null);
            Contract.Requires<ArgumentNullException>(sourceDirectories != null);

            IClassSearcherFactory classSearcherFactory = new ClassSearcherFactory();
            var globalClassSearcher = classSearcherFactory.CreateClassSearcher(sourceDirectories.ToArray());

            var multiReportParser = new MultiReportParser();

            foreach (var report in reportFiles)
            {
                foreach (var parser in GetParsersOfFile(report, classSearcherFactory, globalClassSearcher))
                {
                    multiReportParser.AddParser(parser);
                }
            }

            return multiReportParser;
        }

        /// <summary>
        ///   Tries to initiate the correct parsers for the given report. An empty list is returned if no parser has been found.
        ///   The report may contain several reports. For every report an extra parser is initiated.
        /// </summary>
        /// <param name="reportFile">The report file to parse.</param>
        /// <param name="classSearcherFactory">The class searcher factory.</param>
        /// <param name="globalClassSearcher">The global class searcher.</param>
        /// <returns>
        ///   The IParser instances or an empty list if no matching parser has been found.
        /// </returns>
        private static IEnumerable<IParser> GetParsersOfFile(
            string reportFile, 
            IClassSearcherFactory classSearcherFactory, 
            IClassSearcher globalClassSearcher)
        {
            var parsers = new List<IParser>();

            XContainer report = null;
            try
            {
                logger.InfoFormat(Resources.LoadingReport, reportFile);
                report = XDocument.Load(reportFile);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(" " + Resources.ErrorDuringReadingReport, reportFile, ex.Message);
                return parsers;
            }

            if (report.Descendants("PartCoverReport").Any())
            {
                // PartCover 2.2 and PartCover 2.3 reports differ in version attribute, so use this to determine the correct parser
                if (report.Descendants("PartCoverReport").First().Attribute("ver") != null)
                {
                    foreach (var item in report.Descendants("PartCoverReport"))
                    {
                        logger.Debug(" " + Resources.PreprocessingReport);
                        new PartCover22ReportPreprocessor(item, classSearcherFactory, globalClassSearcher).Execute();
                        logger.DebugFormat(" " + Resources.InitiatingParser, "PartCover 2.2");
                        parsers.Add(new PartCover22Parser(item));
                    }
                }
                else
                {
                    foreach (var item in report.Descendants("PartCoverReport"))
                    {
                        logger.Debug(" " + Resources.PreprocessingReport);
                        new PartCover23ReportPreprocessor(item, classSearcherFactory, globalClassSearcher).Execute();
                        logger.DebugFormat(" " + Resources.InitiatingParser, "PartCover 2.3");
                        parsers.Add(new PartCover23Parser(item));
                    }
                }
            }
            else if (report.Descendants("CoverageSession").Any())
            {
                foreach (var item in report.Descendants("CoverageSession"))
                {
                    logger.Debug(" " + Resources.PreprocessingReport);
                    new OpenCoverReportPreprocessor(item, classSearcherFactory, globalClassSearcher).Execute();
                    logger.DebugFormat(" " + Resources.InitiatingParser, "OpenCover");
                    parsers.Add(new OpenCoverParser(item));
                }
            }
            else if (report.Descendants("coverage").Any())
            {
                foreach (var item in report.Descendants("coverage"))
                {
                    logger.DebugFormat(" " + Resources.InitiatingParser, "NCover");
                    parsers.Add(new NCoverParser(item));
                }
            }
            else if (report.Descendants("CoverageDSPriv").Any())
            {
                foreach (var item in report.Descendants("CoverageDSPriv"))
                {
                    logger.DebugFormat(" " + Resources.InitiatingParser, "Visual Studio");
                    new VisualStudioReportPreprocessor(item).Execute();
                    parsers.Add(new VisualStudioParser(item));
                }
            }
            else if (report.Descendants("results").Any())
            {
                foreach (var item in report.Descendants("results"))
                {
                    if (item.Element("modules") != null)
                    {
                        logger.DebugFormat(" " + Resources.InitiatingParser, "Dynamic Code Coverage");
                        new DynamicCodeCoverageReportPreprocessor(item).Execute();
                        parsers.Add(new DynamicCodeCoverageParser(item));
                    }
                }
            }

            return parsers;
        }
    }
}