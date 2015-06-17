﻿
namespace Palmmedia.ReportGeneratorTest
{
    using NUnit.Framework;
    using System.IO;
    using System.Linq;
    using Moq;
    using Palmmedia.ReportGenerator;
    using Palmmedia.ReportGenerator.Reporting;


    /// <summary>
    /// This is a test class for ReportConfigurationBuilder and is intended
    /// to contain all ReportConfigurationBuilder Unit Tests
    /// </summary>
    [TestFixture]
    public class ReportConfigurationBuilderTest
    {
        private static readonly string ReportPath = Path.Combine(FileManager.GetCSharpReportDirectory(), "OpenCover.xml");

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
            string[] legacyArguments = new string[] 
            { 
                ReportPath,
                "C:\\temp",
                "Latex"
            };

            this.configuration = this.reportConfigurationBuilder.Create(legacyArguments);

            Assert.IsTrue(this.configuration.ReportFiles.Contains(ReportPath), "ReportPath does not exist in ReportFiles.");
            Assert.AreEqual("C:\\temp", this.configuration.TargetDirectory, "Wrong target directory applied.");
            Assert.IsTrue(this.configuration.ReportTypes.Contains("Latex"), "Wrong report type applied.");
            Assert.IsFalse(this.configuration.SourceDirectories.Any(), "Source directories should be empty.");
            Assert.IsFalse(this.configuration.Filters.Any(), "Filters should be empty.");
        }

        [Test]
        public void InitWithNamedArguments_AllPropertiesApplied()
        {
            string[] namedArguments = new string[]
            { 
                "-reports:" + ReportPath,
                "-targetdir:C:\\temp",
                "-reporttype:Latex",
                "-sourcedirs:C:\\temp\\source;C:\\temp\\source2",
                "-filters:+Test;-Test",
                "-verbosity:" + VerbosityLevel.Info.ToString()
            };

            this.configuration = this.reportConfigurationBuilder.Create(namedArguments);

            Assert.IsTrue(this.configuration.ReportFiles.Contains(ReportPath), "ReportPath does not exist in ReportFiles.");
            Assert.AreEqual("C:\\temp", this.configuration.TargetDirectory, "Wrong target directory applied.");
            Assert.IsTrue(this.configuration.ReportTypes.Contains("Latex"), "Wrong report type applied.");
            Assert.IsTrue(this.configuration.SourceDirectories.Contains("C:\\temp\\source"), "Directory does not exist in Source directories.");
            Assert.IsTrue(this.configuration.SourceDirectories.Contains("C:\\temp\\source2"), "Directory does not exist in Source directories.");
            Assert.IsTrue(this.configuration.Filters.Contains("+Test"), "Filter does not exist in ReportFiles.");
            Assert.IsTrue(this.configuration.Filters.Contains("-Test"), "Filter does not exist in ReportFiles.");
            Assert.AreEqual(VerbosityLevel.Info, this.configuration.VerbosityLevel, "Wrong verbosity level applied.");
        }
    }
}
