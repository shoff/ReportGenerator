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
    /// This is a test class for ReportConfigurationBuilder and is intended
    /// to contain all ReportConfigurationBuilder Unit Tests
    /// </summary>
    [TestFixture]
    public class ReportConfigurationBuilderTest
    {
        private static readonly string ReportPath = CommonNames.ReportDirectory + "OpenCover.xml";

        private ReportConfigurationBuilder reportConfigurationBuilder;

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
            this.reportConfigurationBuilder = new ReportConfigurationBuilder(new Mock<IReportBuilderFactory>().Object);
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
        public void InitWithLegacyArguments_AllPropertiesApplied()
        {
            string[] legacyArguments = { ReportPath, CommonNames.TestFilesRoot, "Latex"
            };

            this.configuration = this.reportConfigurationBuilder.Create(legacyArguments);

            Assert.IsTrue(this.configuration.ReportFiles.Contains(ReportPath), "ReportPath does not exist in ReportFiles.");
            Assert.AreEqual(CommonNames.TestFilesRoot, this.configuration.TargetDirectory, "Wrong target directory applied.");
            Assert.IsTrue(this.configuration.ReportTypes.Contains("Latex"), "Wrong report type applied.");
            Assert.IsFalse(this.configuration.SourceDirectories.Any(), "Source directories should be empty.");
            Assert.IsFalse(this.configuration.Filters.Any(), "Filters should be empty.");
        }

        [Test]
        public void InitWithNamedArguments_AllPropertiesApplied()
        {
            string[] namedArguments = { 
                "-reports:" + ReportPath,
                "-targetdir:" + CommonNames.TestFilesRoot + "target",
                "-reporttype:Latex",
                "-sourcedirs:"+ CommonNames.TestFilesRoot + "source;" + CommonNames.TestFilesRoot + "temp\\source2",
                "-filters:+Test;-Test",
                "-verbosity:" + VerbosityLevel.Info
            };

            this.configuration = this.reportConfigurationBuilder.Create(namedArguments);

            Assert.IsTrue(this.configuration.ReportFiles.Contains(ReportPath), "ReportPath does not exist in ReportFiles.");
            Assert.AreEqual(CommonNames.TestFilesRoot + "target", this.configuration.TargetDirectory, "Wrong target directory applied.");
            Assert.IsTrue(this.configuration.ReportTypes.Contains("Latex"), "Wrong report type applied.");
            Assert.IsTrue(this.configuration.SourceDirectories.Contains(CommonNames.TestFilesRoot + "source"), "Directory does not exist in Source directories.");
            Assert.IsTrue(this.configuration.SourceDirectories.Contains(CommonNames.TestFilesRoot + "temp\\source2"), "Directory does not exist in Source directories.");
            Assert.IsTrue(this.configuration.Filters.Contains("+Test"), "Filter does not exist in ReportFiles.");
            Assert.IsTrue(this.configuration.Filters.Contains("-Test"), "Filter does not exist in ReportFiles.");
            Assert.AreEqual(VerbosityLevel.Info, this.configuration.VerbosityLevel, "Wrong verbosity level applied.");
        }
    }
}
