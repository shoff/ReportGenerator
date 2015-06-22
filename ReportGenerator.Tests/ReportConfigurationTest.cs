

namespace ReportGenerator.Tests
{
    using System.IO;
    using System.Linq;
    using Moq;
    using NUnit.Framework;
    using Palmmedia.ReportGenerator;
    using Palmmedia.ReportGenerator.Reporting;

    /// <summary>
    /// This is a test class for ReportConfiguration and is intended
    /// to contain all ReportConfiguration Unit Tests
    /// </summary>
    [TestFixture]
    public class ReportConfigurationTest
    {
        private static readonly string ReportPath = CommonNames.ReportDirectory + "OpenCover.xml";

        private Mock<IReportBuilderFactory> reportBuilderFactoryMock;

        private ReportConfiguration configuration;

        [SetUp]
        public void MyTestInitialize()
        {
            this.reportBuilderFactoryMock = new Mock<IReportBuilderFactory>();
        }

        

        [Test]
        public void Init_By_Constructor_All_Default_Values_Applied()
        {
            this.configuration = new ReportConfiguration(
                this.reportBuilderFactoryMock.Object,
                new[] { ReportPath }, CommonNames.TestFilesRoot,
                CommonNames.TestFilesRoot + "historic",
                new string[] { },
                new string[] { },
                new string[] { },
                string.Empty);

            Assert.IsTrue(this.configuration.ReportFiles.Contains(ReportPath), "ReportPath does not exist in ReportFiles.");
            Assert.AreEqual(CommonNames.TestFilesRoot, this.configuration.TargetDirectory, "Wrong target directory applied.");
            Assert.AreEqual(CommonNames.CodeDirectory + "historic", this.configuration.HistoryDirectory, "Wrong target directory applied.");
            Assert.IsTrue(this.configuration.ReportTypes.Contains("Html"), "Wrong report type applied.");
            Assert.AreEqual(0, this.configuration.SourceDirectories.Count(), "Wrong number of source directories.");
            Assert.AreEqual(0, this.configuration.Filters.Count(), "Wrong number of filters.");
            Assert.AreEqual(VerbosityLevel.Verbose, this.configuration.VerbosityLevel, "Wrong verbosity level applied.");
        }

        [Test]
        public void InitByConstructor_AllPropertiesApplied()
        {
            this.configuration = new ReportConfiguration(
                this.reportBuilderFactoryMock.Object,
                new[] { ReportPath },
                CommonNames.TestFilesRoot,
                null,
                new[] { "Latex", "Xml", "Html" },
                new[] { CommonNames.CodeDirectory },
                new[] { "+Test", "-Test" },
                VerbosityLevel.Info.ToString());

            Assert.IsTrue(this.configuration.ReportFiles.Contains(ReportPath), "ReportPath does not exist in ReportFiles.");
            Assert.AreEqual(CommonNames.TestFilesRoot, this.configuration.TargetDirectory, "Wrong target directory applied.");
            Assert.IsTrue(this.configuration.ReportTypes.Contains("Latex"), "Wrong report type applied.");
            Assert.IsTrue(this.configuration.ReportTypes.Contains("Xml"), "Wrong report type applied.");
            Assert.IsTrue(this.configuration.ReportTypes.Contains("Html"), "Wrong report type applied.");
            Assert.IsTrue(this.configuration.SourceDirectories.Contains(CommonNames.CodeDirectory), "Directory does not exist in Source directories.");
            Assert.IsTrue(this.configuration.Filters.Contains("+Test"), "Filter does not exist in ReportFiles.");
            Assert.IsTrue(this.configuration.Filters.Contains("-Test"), "Filter does not exist in ReportFiles.");
            Assert.AreEqual(VerbosityLevel.Info, this.configuration.VerbosityLevel, "Wrong verbosity level applied.");
        }

        [Test]
        public void Validate_AllPropertiesApplied_ValidationPasses()
        {
            this.reportBuilderFactoryMock
                .Setup(r => r.GetAvailableReportTypes())
                .Returns(new[] { "Latex", "Xml", "Html", "Something" });

            this.InitByConstructor_AllPropertiesApplied();

            Assert.IsTrue(this.configuration.Validate(), "Validation should pass.");
        }

        [Test]
        public void Validate_NoReport_ValidationFails()
        {
            this.configuration = new ReportConfiguration(
                this.reportBuilderFactoryMock.Object,
                new string[] { },
                CommonNames.TestFilesRoot,
                null,
                new[] { "Latex" },
                new[] { CommonNames.CodeDirectory },
                new[] { "+Test", "-Test" },
                VerbosityLevel.Info.ToString());

            Assert.IsFalse(this.configuration.Validate(), "Validation should fail.");
        }

        [Test]
        public void Validate_NonExistingReport_ValidationFails()
        {
            this.configuration = new ReportConfiguration(
                this.reportBuilderFactoryMock.Object,
                new[] { "123.xml" },
                CommonNames.TestFilesRoot,
                null,
                new[] { "Latex" },
                new[] { CommonNames.CodeDirectory },
                new[] { "+Test", "-Test" },
                VerbosityLevel.Info.ToString());

            Assert.IsFalse(this.configuration.Validate(), "Validation should fail.");
        }

        [Test]
        public void Validate_NoTargetDirectory_ValidationFails()
        {
            this.configuration = new ReportConfiguration(
                this.reportBuilderFactoryMock.Object,
                new[] { ReportPath },
                string.Empty,
                null,
                new[] { "Latex" },
                new[] { CommonNames.CodeDirectory },
                new[] { "+Test", "-Test" },
                VerbosityLevel.Info.ToString());

            Assert.IsFalse(this.configuration.Validate(), "Validation should fail.");
        }

        [Test]
        public void Validate_InvalidTargetDirectory_ValidationFails()
        {
            this.configuration = new ReportConfiguration(
                this.reportBuilderFactoryMock.Object,
                new[] { ReportPath },
                "C:\\temp:?$",
                null,
                new[] { "Latex" },
                new[] { CommonNames.CodeDirectory },
                new[] { "+Test", "-Test" },
                VerbosityLevel.Info.ToString());

            Assert.IsFalse(this.configuration.Validate(), "Validation should fail.");
        }

        [Test]
        public void Validate_InvalidHistoryDirectory_ValidationFails()
        {
            this.configuration = new ReportConfiguration(
                this.reportBuilderFactoryMock.Object,
                new[] { ReportPath },
                CommonNames.TestFilesRoot,
                "C:\\temp:?$",
                new[] { "Latex" },
                new[] { CommonNames.CodeDirectory },
                new[] { "+Test", "-Test" },
                VerbosityLevel.Info.ToString());

            Assert.IsFalse(this.configuration.Validate(), "Validation should fail.");
        }

        [Test]
        public void Validate_InvalidReportType_ValidationFails()
        {
            this.configuration = new ReportConfiguration(
                this.reportBuilderFactoryMock.Object,
                new[] { ReportPath },
                CommonNames.TestFilesRoot,
                null,
                new[] { "DoesNotExist" },
                new[] { CommonNames.CodeDirectory },
                new[] { "+Test", "-Test" },
                VerbosityLevel.Info.ToString());

            Assert.IsFalse(this.configuration.Validate(), "Validation should fail.");
        }

        [Test]
        public void Validate_NotExistingSourceDirectory_ValidationFails()
        {
            this.configuration = new ReportConfiguration(
                this.reportBuilderFactoryMock.Object,
                new[] { ReportPath },
                CommonNames.TestFilesRoot,
                null,
                new[] { "Latex" },
                new[] { Path.Combine(CommonNames.CodeDirectory, "123456") },
                new[] { "+Test", "-Test" },
                VerbosityLevel.Info.ToString());

            Assert.IsFalse(this.configuration.Validate(), "Validation should fail.");
        }

        [Test]
        public void Validate_InvalidFilter_ValidationFails()
        {
            this.configuration = new ReportConfiguration(
                this.reportBuilderFactoryMock.Object,
                new[] { ReportPath },
                @CommonNames.TestFilesRoot,
                null,
                new[] { "Latex" },
                new[] { CommonNames.CodeDirectory },
                new[] { "Test" },
                VerbosityLevel.Info.ToString());

            Assert.IsFalse(this.configuration.Validate(), "Validation should fail.");
        }
    }
}
