namespace Palmmedia.ReportGenerator.Builders
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Palmmedia.ReportGenerator.Parser.Analysis;
    using Palmmedia.ReportGenerator.Rendering;
    using Palmmedia.ReportGenerator.Reporting;

    /// <summary>
    /// Creates report in HTML format.
    /// </summary>
    [Export(typeof(IReportBuilder))]
    public class HtmlReportBuilder : ReportBuilderBase
    {
        /// <summary>
        /// Gets the report type.
        /// </summary>
        /// <value>
        /// The report format.
        /// </value>
        public override string ReportType
        {
            get { return "Html"; }
        }

        /// <summary>
        /// Creates a class report.
        /// </summary>
        /// <param name="reportClass">The class.</param>
        /// <param name="fileAnalyses">The file analyses that correspond to the class.</param>
        public override void CreateClassReport(Class reportClass, IEnumerable<FileAnalysis> fileAnalyses)
        {
            using (var renderer = new HtmlRenderer(false))
            {
                this.CreateClassReport(renderer, reportClass, fileAnalyses);
            }
        }

        /// <summary>
        /// Creates the summary report.
        /// </summary>
        /// <param name="summaryResult">The summary result.</param>
        public override void CreateSummaryReport(SummaryResult summaryResult)
        {
            using (var renderer = new HtmlRenderer(false))
            {
                this.CreateSummaryReport(renderer, summaryResult);
            }
        }
    }
}
