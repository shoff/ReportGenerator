

// ReSharper disable InconsistentNaming
namespace ReportGenerator.Tests.Reporting
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Palmmedia.ReportGenerator.Parser.Analysis;
    using Palmmedia.ReportGenerator.Reporting;

    /// <summary>
    /// This is a test class for MefReportBuilderFactory and is intended
    /// to contain all MefReportBuilderFactory Unit Tests
    /// </summary>
    [TestFixture]
    public class MefReportBuilderFactoryTest
    {
        [Test]
        public void GetAvailableReportTypes_All_Report_Types_Returned()
        {
            var factory = new MefReportBuilderFactory();
            var expected = 6;
            var actual = factory.GetAvailableReportTypes().Count();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetReportBuilders_Default_Report_Builder_Returned()
        {
            var factory = new MefReportBuilderFactory();
            var reportBuilders = factory.GetReportBuilders("C:\\temp", new[] { "Html" });
            Assert.AreEqual(1, reportBuilders.Count(), "Default report builder not available.");

            reportBuilders = factory.GetReportBuilders("C:\\temp", new[] { "Latex" });
            
            Assert.AreEqual(1, reportBuilders.Count(), "Report builder not available.");

            Assert.AreEqual(typeof(AdditionalLatexReportBuilder), reportBuilders.First().GetType(), "Non default report builder should get returned");
        }
    }

    [System.ComponentModel.Composition.Export(typeof(IReportBuilder))]
    public class AdditionalLatexReportBuilder : IReportBuilder
    {
        /// <summary>
        /// Gets the type of the report.
        /// </summary>
        /// <value>
        /// The type of the report.
        /// </value>
        public string ReportType
        {
            get { return "Latex"; }
        }

        /// <summary>
        /// Gets or sets the target directory where reports are stored.
        /// </summary>
        /// <value>
        /// The target directory.
        /// </value>
        public string TargetDirectory { get; set; }

        /// <summary>
        /// Creates a class report.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <param name="fileAnalyses">The file analyses that correspond to the class.</param>
        public void CreateClassReport(Class @class, IEnumerable<FileAnalysis> fileAnalyses)
        {
        }

        /// <summary>
        /// Creates the summary report.
        /// </summary>
        /// <param name="summaryResult">The summary result.</param>
        public void CreateSummaryReport(SummaryResult summaryResult)
        {
        }
    }
}
