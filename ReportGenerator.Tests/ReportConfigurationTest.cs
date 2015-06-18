﻿

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
        private static readonly string ReportPath = Path.Combine(FileManager.GetCSharpReportDirectory(), "OpenCover.xml");

        private Mock<IReportBuilderFactory> reportBuilderFactoryMock;

        private ReportConfiguration configuration;

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        // Use ClassInitialize to run code before running the first test in the class
        // [SetUp]
        // public static void MyClassInitialize(TestContext testContext)
        // {
        // }

        // Use ClassCleanup to run code after all tests in a class have run
        // [TearDown]
        // public static void MyClassCleanup()
        // {
        // }

        // Use TestInitialize to run code before running each test
        [SetUp]
        public void MyTestInitialize()
        {
            this.reportBuilderFactoryMock = new Mock<IReportBuilderFactory>();
        }

        // Use TestCleanup to run code after each test has run
        [TearDown]
        public void MyTestCleanup()
        {
            Assert.IsNotNull(this.configuration.ReportFiles);
            Assert.IsNotNull(this.configuration.SourceDirectories);
            Assert.IsNotNull(this.configuration.Filters);
        }

        #endregion

        [Test]
        public void InitByConstructor_AllDefaultValuesApplied()
        {
            this.configuration = new ReportConfiguration(
                this.reportBuilderFactoryMock.Object,
                new[] { ReportPath },
                "C:\\temp",
                "C:\\temp\\historic",
                new string[] { },
                new string[] { },
                new string[] { },
                string.Empty);

            Assert.IsTrue(this.configuration.ReportFiles.Contains(ReportPath), "ReportPath does not exist in ReportFiles.");
            Assert.AreEqual("C:\\temp", this.configuration.TargetDirectory, "Wrong target directory applied.");
            Assert.AreEqual("C:\\temp\\historic", this.configuration.HistoryDirectory, "Wrong target directory applied.");
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
                "C:\\temp",
                null,
                new[] { "Latex", "Xml", "Html" },
                new[] { FileManager.GetCSharpCodeDirectory() },
                new[] { "+Test", "-Test" },
                VerbosityLevel.Info.ToString());

            Assert.IsTrue(this.configuration.ReportFiles.Contains(ReportPath), "ReportPath does not exist in ReportFiles.");
            Assert.AreEqual("C:\\temp", this.configuration.TargetDirectory, "Wrong target directory applied.");
            Assert.IsTrue(this.configuration.ReportTypes.Contains("Latex"), "Wrong report type applied.");
            Assert.IsTrue(this.configuration.ReportTypes.Contains("Xml"), "Wrong report type applied.");
            Assert.IsTrue(this.configuration.ReportTypes.Contains("Html"), "Wrong report type applied.");
            Assert.IsTrue(this.configuration.SourceDirectories.Contains(FileManager.GetCSharpCodeDirectory()), "Directory does not exist in Source directories.");
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
                "C:\\temp",
                null,
                new[] { "Latex" },
                new[] { FileManager.GetCSharpCodeDirectory() },
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
                "C:\\temp",
                null,
                new[] { "Latex" },
                new[] { FileManager.GetCSharpCodeDirectory() },
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
                new[] { FileManager.GetCSharpCodeDirectory() },
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
                new[] { FileManager.GetCSharpCodeDirectory() },
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
                "C:\\temp",
                "C:\\temp:?$",
                new[] { "Latex" },
                new[] { FileManager.GetCSharpCodeDirectory() },
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
                "C:\\temp",
                null,
                new[] { "DoesNotExist" },
                new[] { FileManager.GetCSharpCodeDirectory() },
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
                "C:\\temp",
                null,
                new[] { "Latex" },
                new[] { Path.Combine(FileManager.GetCSharpCodeDirectory(), "123456") },
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
                @"C:\\temp",
                null,
                new[] { "Latex" },
                new[] { FileManager.GetCSharpCodeDirectory() },
                new[] { "Test" },
                VerbosityLevel.Info.ToString());

            Assert.IsFalse(this.configuration.Validate(), "Validation should fail.");
        }
    }
}
