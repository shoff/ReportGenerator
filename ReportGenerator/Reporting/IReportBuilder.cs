

namespace Palmmedia.ReportGenerator.Reporting
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Collections.Generic;
    using Palmmedia.ReportGenerator.Parser.Analysis;

    /// <summary>
    /// Interface that has to be implemented by classes capable of rendering reports.
    /// </summary>
    [ContractClass(typeof(ReportBuilderContract))]
    public interface IReportBuilder
    {
        /// <summary>
        /// Gets the report type.
        /// </summary>
        /// <value>
        /// The report type.
        /// </value>
        string ReportType { get; }

        /// <summary>
        /// Gets or sets the target directory where reports are stored.
        /// </summary>
        /// <value>
        /// The target directory.
        /// </value>
        string TargetDirectory { get; set; }

        /// <summary>
        /// Creates a class report.
        /// </summary>
        /// <param name="reportClass">The class.</param>
        /// <param name="fileAnalyses">The file analyses that correspond to the class.</param>
        void CreateClassReport(Class reportClass, IEnumerable<FileAnalysis> fileAnalyses);

        /// <summary>
        /// Creates the summary report.
        /// </summary>
        /// <param name="summaryResult">The summary result.</param>
        void CreateSummaryReport(SummaryResult summaryResult);
    }

    [ExcludeFromCodeCoverage]
    [ContractClassFor(typeof(IReportBuilder))]
    public abstract class ReportBuilderContract : IReportBuilder
    {
        public string ReportType { get; private set; }
        public string TargetDirectory { get; set; }

        public void CreateClassReport(Class reportClass, IEnumerable<FileAnalysis> fileAnalyses)
        {
            Contract.Requires<ArgumentNullException>(reportClass != null);
            Contract.Requires<ArgumentNullException>(fileAnalyses != null);
        }

        public void CreateSummaryReport(SummaryResult summaryResult)
        {
            Contract.Requires<ArgumentNullException>(summaryResult != null);
        }
    }
}
