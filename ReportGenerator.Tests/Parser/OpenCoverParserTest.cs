namespace ReportGenerator.Tests.Parser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Moq;
    using NUnit.Framework;
    using Palmmedia.ReportGenerator.Parser;
    using Palmmedia.ReportGenerator.Parser.Analysis;
    using Palmmedia.ReportGenerator.Parser.Preprocessing;
    using Palmmedia.ReportGenerator.Parser.Preprocessing.FileSearch;
    using ReportGenerator.Tests.TestHelpers;

    /// <summary>
    /// This is a test class for OpenCoverParser and is intended
    /// to contain all OpenCoverParser Unit Tests
    /// </summary>
    [TestFixture]
    public class OpenCoverParserTest
    {
        private readonly string filePath1 = CommonNames.ReportDirectory + "OpenCover.xml";
        private readonly string filePath2 = CommonNames.ReportDirectory + "OpenCoverWithTrackedMethods.xml";
        private ICollection<Assembly> assembliesWithoutPreprocessing;
        private ICollection<Assembly> assembliesWithPreprocessing;
        private ICollection<Assembly> assembliesWithTrackedMethods;
        private readonly string projectFolder = AppDomain.CurrentDomain.BaseDirectory + "\\TestFiles\\Project";

        private Mock<IClassSearcherFactory> classSearcherFactoryMock;
        private Mock<IClassSearcher> classSearcherMock;


        [SetUp]
        public void SetUp()
        {
            this.classSearcherFactoryMock = new Mock<IClassSearcherFactory>();
            this.classSearcherMock = new Mock<IClassSearcher>();

            assembliesWithoutPreprocessing = new OpenCoverParser(XDocument.Load(filePath1)).Assemblies;

            var report = XDocument.Load(filePath1);
            var classSearcherFactory = new ClassSearcherFactory();
            var globalClassSearcher = classSearcherFactory.CreateClassSearcher(projectFolder);
            
            // why are we doing this?
            var openCoverReportPreporcessor = new OpenCoverReportPreprocessor(report, classSearcherFactory, globalClassSearcher);
            openCoverReportPreporcessor.Execute();

            assembliesWithPreprocessing = new OpenCoverParser(report).Assemblies;

            report = XDocument.Load(filePath2);
            classSearcherFactory = new ClassSearcherFactory();
            globalClassSearcher = classSearcherFactory.CreateClassSearcher(projectFolder);
            new OpenCoverReportPreprocessor(report, classSearcherFactory, globalClassSearcher).Execute();
            assembliesWithTrackedMethods = new OpenCoverParser(report).Assemblies;
        }

        [Test]
        public void NumberOfLineVisitsTest_WithoutPreprocessing()
        {
            var fileAnalysis = FileAnalysisCreator.GetFileAnalysis(assembliesWithoutPreprocessing, "ReportGenerator.Tests.TestFiles.Project.TestClass",
                projectFolder + "\\TestClass.cs");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 9).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 10).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 11).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 12).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 19).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 23).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 31).LineVisits, "Wrong number of line visits");

            fileAnalysis = FileAnalysisCreator.GetFileAnalysis(assembliesWithoutPreprocessing, "ReportGenerator.Tests.TestFiles.Project.TestClass2",
                projectFolder + "\\TestClass2.cs");
            Assert.AreEqual(-1, fileAnalysis.Lines.Single(l => l.LineNumber == 13).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(-1, fileAnalysis.Lines.Single(l => l.LineNumber == 15).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 19).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(2, fileAnalysis.Lines.Single(l => l.LineNumber == 25).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 31).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 37).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(4, fileAnalysis.Lines.Single(l => l.LineNumber == 54).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 81).LineVisits, "Wrong number of line visits");
            Assert.IsFalse(fileAnalysis.Lines.Single(l => l.LineNumber == 44).CoveredBranches.HasValue, "No covered branches");
            Assert.IsFalse(fileAnalysis.Lines.Single(l => l.LineNumber == 44).TotalBranches.HasValue, "No total branches");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 45).CoveredBranches.Value, "Wrong number of covered branches");
            Assert.AreEqual(2, fileAnalysis.Lines.Single(l => l.LineNumber == 45).TotalBranches.Value, "Wrong number of total branches");

            fileAnalysis = FileAnalysisCreator.GetFileAnalysis(assembliesWithoutPreprocessing, "ReportGenerator.Tests.TestFiles.Project.PartialClass", 
                projectFolder + "\\PartialClass.cs");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 9).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 14).LineVisits, "Wrong number of line visits");

            fileAnalysis = FileAnalysisCreator.GetFileAnalysis(assembliesWithoutPreprocessing, "ReportGenerator.Tests.TestFiles.Project.PartialClass", 
                projectFolder + "\\PartialClass2.cs");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 9).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 14).LineVisits, "Wrong number of line visits");

            fileAnalysis = FileAnalysisCreator.GetFileAnalysis(assembliesWithoutPreprocessing, "ReportGenerator.Tests.TestFiles.Project.ClassWithExcludes",
                projectFolder + "\\ClassWithExcludes.cs");
            Assert.AreEqual(-1, fileAnalysis.Lines.Single(l => l.LineNumber == 9).LineVisits, "Wrong number of line visits (Property is excluded)");
            Assert.AreEqual(-1, fileAnalysis.Lines.Single(l => l.LineNumber == 19).LineVisits, "Wrong number of line visits (Method is excluded)");
        }

        [Test]
        public void NumberOfLineVisitsTest_WithPreprocessing()
        {
            var fileAnalysis = FileAnalysisCreator.GetFileAnalysis(assembliesWithPreprocessing, "ReportGenerator.Tests.TestFiles.Project.TestClass", projectFolder + "\\TestClass.cs");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 9).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 10).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 11).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 12).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 19).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 23).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 31).LineVisits, "Wrong number of line visits");

            fileAnalysis = FileAnalysisCreator.GetFileAnalysis(assembliesWithPreprocessing, "ReportGenerator.Tests.TestFiles.Project.TestClass2", projectFolder + "\\TestClass2.cs");
            Assert.AreEqual(3, fileAnalysis.Lines.Single(l => l.LineNumber == 13).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 15).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 19).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(2, fileAnalysis.Lines.Single(l => l.LineNumber == 25).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 31).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 37).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(4, fileAnalysis.Lines.Single(l => l.LineNumber == 54).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 81).LineVisits, "Wrong number of line visits");
            Assert.IsFalse(fileAnalysis.Lines.Single(l => l.LineNumber == 44).CoveredBranches.HasValue, "No covered branches");
            Assert.IsFalse(fileAnalysis.Lines.Single(l => l.LineNumber == 44).TotalBranches.HasValue, "No total branches");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 45).CoveredBranches.Value, "Wrong number of covered branches");
            Assert.AreEqual(2, fileAnalysis.Lines.Single(l => l.LineNumber == 45).TotalBranches.Value, "Wrong number of total branches");

            fileAnalysis = FileAnalysisCreator.GetFileAnalysis(assembliesWithPreprocessing, "ReportGenerator.Tests.TestFiles.Project.PartialClass", projectFolder + "\\PartialClass.cs");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 9).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 14).LineVisits, "Wrong number of line visits");

            fileAnalysis = FileAnalysisCreator.GetFileAnalysis(assembliesWithPreprocessing, "ReportGenerator.Tests.TestFiles.Project.PartialClass", projectFolder + "\\PartialClass2.cs");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 9).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 14).LineVisits, "Wrong number of line visits");

            fileAnalysis = FileAnalysisCreator.GetFileAnalysis(assembliesWithPreprocessing, "ReportGenerator.Tests.TestFiles.Project.ClassWithExcludes", projectFolder + "\\ClassWithExcludes.cs");
            Assert.AreEqual(-1, fileAnalysis.Lines.Single(l => l.LineNumber == 9).LineVisits, "Wrong number of line visits (Property is excluded)");
            Assert.AreEqual(-1, fileAnalysis.Lines.Single(l => l.LineNumber == 19).LineVisits, "Wrong number of line visits (Method is excluded)");
        }

        [Test]
        public void NumberOfLineVisitsTest_WithTrackedMethods()
        {
            var fileAnalysis = FileAnalysisCreator.GetFileAnalysis(assembliesWithTrackedMethods, "ReportGenerator.Tests.TestFiles.Project.PartialClass", projectFolder + "\\PartialClass.cs");

            var line = fileAnalysis.Lines.Single(l => l.LineNumber == 9);

            Assert.AreEqual(2, line.LineVisits, "Wrong number of line visits");

            Assert.AreEqual(2, line.LineCoverageByTestMethod.Count, "Wrong number of test methods");
            Assert.AreEqual(1, line.LineCoverageByTestMethod.First().Value.LineVisits, "Wrong number of test methods");
            Assert.AreEqual(1, line.LineCoverageByTestMethod.ElementAt(1).Value.LineVisits, "Wrong number of test methods");
        }

        [Test]
        public void NumberOfFilesTest()
        {
            Assert.AreEqual(11, assembliesWithoutPreprocessing.SelectMany(a => a.Classes).SelectMany(a => a.Files).Distinct().Count(), "Wrong number of files");
        }

        [Test]
        public void FilesOfClassTest()
        {
            Assert.AreEqual(1, assembliesWithoutPreprocessing.Single(a => a.Name == "ReportGenerator.Tests").Classes
                .Single(c => c.Name == "ReportGenerator.Tests.TestFiles.Project.TestClass").Files.Count(), "Wrong number of files");
            Assert.AreEqual(2, assembliesWithoutPreprocessing.Single(a => a.Name == "ReportGenerator.Tests").Classes
                .Single(c => c.Name == "ReportGenerator.Tests.TestFiles.Project.PartialClass").Files.Count(), "Wrong number of files");
        }

        [Test]
        public void ClassesInAssemblyTest()
        {
            Assert.AreEqual(16, assembliesWithoutPreprocessing.SelectMany(a => a.Classes).Count(), "Wrong number of classes");
        }

        [Test]
        public void AssembliesTest()
        {
            Assert.AreEqual(1, assembliesWithoutPreprocessing.Count(), "Wrong number of assemblies");
        }

        [Test]
        public void GetCoverableLinesOfClassTest()
        {
            Assert.AreEqual(4, assembliesWithoutPreprocessing.Single(a => a.Name == "ReportGenerator.Tests").Classes.Single(c => c.Name == "ReportGenerator.Tests.TestFiles.Project.AbstractClass").CoverableLines, "Wrong Coverable Lines");
        }

        [Test]
        public void GetCoverageQuotaOfClassTest()
        {
            Assert.AreEqual(50m, assembliesWithoutPreprocessing.Single(a => a.Name == "ReportGenerator.Tests").Classes.Single(c => c.Name == "ReportGenerator.Tests.TestFiles.Project.PartialClassWithAutoProperties").CoverageQuota, "Wrong coverage quota");
        }

        [Test]
        public void MethodMetricsTest()
        {
            var metrics = assembliesWithoutPreprocessing.Single(a => a.Name == "ReportGenerator.Tests").Classes.Single(c => c.Name == "ReportGenerator.Tests.TestFiles.Project.TestClass").MethodMetrics;

            Assert.AreEqual(2, metrics.Count(), "Wrong number of method metrics");
            Assert.AreEqual("System.Void Test.TestClass::SampleFunction()", metrics.First().Name, "Wrong name of method");
            Assert.AreEqual(3, metrics.First().Metrics.Count(), "Wrong number of metrics");

            Assert.AreEqual("Cyclomatic Complexity", metrics.First().Metrics.ElementAt(0).Name, "Wrong name of metric");
            Assert.AreEqual(3, metrics.First().Metrics.ElementAt(0).Value, "Wrong value of metric");
            Assert.AreEqual("Sequence Coverage", metrics.First().Metrics.ElementAt(1).Name, "Wrong name of metric");
            Assert.AreEqual(75M, metrics.First().Metrics.ElementAt(1).Value, "Wrong value of metric");
            Assert.AreEqual("Branch Coverage", metrics.First().Metrics.ElementAt(2).Name, "Wrong name of metric");
            Assert.AreEqual(60, metrics.First().Metrics.ElementAt(2).Value, "Wrong value of metric");
        }

    }
}
