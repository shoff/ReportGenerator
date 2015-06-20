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

    /// <summary>
    /// This is a test class for MultiReportParser and is intended
    /// to contain all MultiReportParser Unit Tests
    /// </summary>
    [TestFixture, Ignore]
    public class MultiReportParserTest
    {
        private static readonly string filePath1 = Path.Combine(FileManager.GetCSharpReportDirectory(), "Partcover2.2.xml");
        private static readonly string filePath2 = Path.Combine(FileManager.GetCSharpReportDirectory(), "Partcover2.3.xml");
        private static List<Assembly> assembliesWithoutPreprocessing;
        private static List<Assembly> assembliesWithPreprocessing;
        private static readonly string testClassDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\TestFiles\\Project";

        private Mock<IParser> parser22Mock;
        private Mock<IParser> parser23Mock;

        private Mock<IClassSearcherFactory> classSearcherFactoryMock;
        private Mock<IClassSearcher> classSearcherMock;

        [SetUp]
        public void SetUp()
        {
            Assert.IsTrue(File.Exists(filePath1));
            Assert.IsTrue(File.Exists(filePath2));

            this.parser22Mock = new Mock<IParser>();
            this.parser23Mock = new Mock<IParser>();

            this.classSearcherFactoryMock = new Mock<IClassSearcherFactory>();
            this.classSearcherMock = new Mock<IClassSearcher>();

            // TODO this REALLY needs to be changed to actually test in isolation.
            var multiReportParser = new MultiReportParser();
            SetupMultiReportParser(multiReportParser);

            // var classSearcherFactory = new ClassSearcherFactory();
            // var globalClassSearcher = classSearcherFactory.CreateClassSearcher(testClassDirectory);

            multiReportParser = new MultiReportParser();

            var report = XDocument.Load(filePath1);

            // TODO this needs to be changed, too much going on in one single test!

            var partCover22ReportProcessor = new PartCover22ReportPreprocessor( report,
                this.classSearcherFactoryMock.Object, this.classSearcherMock.Object); 
            //classSearcherFactory, globalClassSearcher);
            // partCover22ReportProcessor.Execute();


            multiReportParser.AddParser(new PartCover22Parser(report));
            report = XDocument.Load(filePath2);

            // TODO this needs to be changed, too much going on in one single test!           
            var partCover23ReportProcessor = new PartCover23ReportPreprocessor(report,
                this.classSearcherFactoryMock.Object, this.classSearcherMock.Object);

            partCover23ReportProcessor.Execute();

            multiReportParser.AddParser(new PartCover23Parser(report));
            
            assembliesWithPreprocessing = (List<Assembly>)multiReportParser.Assemblies;
        }

        private void SetupMultiReportParser(MultiReportParser multiReportParser)
        {
            // TODO mock the parser.
            multiReportParser.AddParser(new PartCover22Parser(XDocument.Load(filePath1)));

            multiReportParser.AddParser(new PartCover22Parser(XDocument.Load(filePath1)));

            assembliesWithoutPreprocessing = (List<Assembly>)multiReportParser.Assemblies;
        }

        [Test]
        public void Number_Of_Line_Visits_Test_Without_Preprocessing()
        {
            var fileAnalysis = GetFileAnalysis(assembliesWithoutPreprocessing, "ReportGenerator.Tests.TestFiles.Project.TestClass", testClassDirectory + "\\TestClass.cs");
            Assert.AreEqual(2, fileAnalysis.Lines.Single(l => l.LineNumber == 14).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 18).LineVisits, "Wrong number of line visits");

            fileAnalysis = GetFileAnalysis(assembliesWithoutPreprocessing, "Test.TestClass2", @"C:\Projects\CodeCoverage\ReportGenerator\ReportGenerator.Tests\bin\Debug\TestFiles\Project\TestClass2.cs");
            Assert.AreEqual(-1, fileAnalysis.Lines.Single(l => l.LineNumber == 13).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 19).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(4, fileAnalysis.Lines.Single(l => l.LineNumber == 25).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(2, fileAnalysis.Lines.Single(l => l.LineNumber == 31).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 37).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(8, fileAnalysis.Lines.Single(l => l.LineNumber == 54).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 81).LineVisits, "Wrong number of line visits");

            fileAnalysis = GetFileAnalysis(assembliesWithoutPreprocessing, "ReportGenerator.Tests.TestFiles.Project.PartialClass", testClassDirectory + "\\PartialClass.cs");
            Assert.AreEqual(2, fileAnalysis.Lines.Single(l => l.LineNumber == 9).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 14).LineVisits, "Wrong number of line visits");

            fileAnalysis = GetFileAnalysis(assembliesWithoutPreprocessing, "ReportGenerator.Tests.TestFiles.Project.PartialClass", testClassDirectory + "\\PartialClass2.cs");
            Assert.AreEqual(2, fileAnalysis.Lines.Single(l => l.LineNumber == 9).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 14).LineVisits, "Wrong number of line visits");
        }

        [Test]
        public void Number_Of_Line_Visits_Test_With_Preprocessing()
        {
            var fileAnalysis = GetFileAnalysis(assembliesWithPreprocessing, "ReportGenerator.Tests.TestFiles.Project.TestClass", testClassDirectory + "\\TestClass.cs");
            Assert.AreEqual(2, fileAnalysis.Lines.Single(l => l.LineNumber == 14).LineVisits, "Wrong number of line visits");
            //Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 18).LineVisits, "Wrong number of line visits");

            //fileAnalysis = GetFileAnalysis(assembliesWithPreprocessing, "ReportGenerator.Tests.TestFiles.Project.TestClass2", "C:\Projects\CodeCoverage\ReportGenerator\ReportGenerator.Tests\bin\Debug\TestFiles\Project\\\TestClass2.cs");
            //Assert.AreEqual(6, fileAnalysis.Lines.Single(l => l.LineNumber == 13).LineVisits, "Wrong number of line visits");
            //Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 19).LineVisits, "Wrong number of line visits");
            //Assert.AreEqual(4, fileAnalysis.Lines.Single(l => l.LineNumber == 25).LineVisits, "Wrong number of line visits");
            //Assert.AreEqual(2, fileAnalysis.Lines.Single(l => l.LineNumber == 31).LineVisits, "Wrong number of line visits");
            //Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 37).LineVisits, "Wrong number of line visits");
            //Assert.AreEqual(8, fileAnalysis.Lines.Single(l => l.LineNumber == 54).LineVisits, "Wrong number of line visits");
            //Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 81).LineVisits, "Wrong number of line visits");

            //fileAnalysis = GetFileAnalysis(assembliesWithPreprocessing, "ReportGenerator.Tests.TestFiles.Project.PartialClass", "C:\Projects\CodeCoverage\ReportGenerator\ReportGenerator.Tests\bin\Debug\TestFiles\Project\\\PartialClass.cs");
            //Assert.AreEqual(2, fileAnalysis.Lines.Single(l => l.LineNumber == 9).LineVisits, "Wrong number of line visits");
            //Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 14).LineVisits, "Wrong number of line visits");

            //fileAnalysis = GetFileAnalysis(assembliesWithPreprocessing, "ReportGenerator.Tests.TestFiles.Project.PartialClass", "C:\Projects\CodeCoverage\ReportGenerator\ReportGenerator.Tests\bin\Debug\TestFiles\Project\\\PartialClass2.cs");
            //Assert.AreEqual(2, fileAnalysis.Lines.Single(l => l.LineNumber == 9).LineVisits, "Wrong number of line visits");
            //Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 14).LineVisits, "Wrong number of line visits");
        }

        [Test]
        public void Number_Of_Files_Test()
        {
            Assert.AreEqual(5, assembliesWithoutPreprocessing.SelectMany(a => a.Classes).SelectMany(a => a.Files).Distinct().Count(), "Wrong number of files");
        }

        [Test]
        public void GetFileAnalysis_On_TestClass_Returns()
        {
            var actual = assembliesWithoutPreprocessing.Single(a => a.Name == "Test").Classes.Single(
                    c => c.Name == "ReportGenerator.Tests.TestFiles.Project.TestClass").Files.Count();

            Assert.AreEqual(1, actual);
        }

        [Test]
        public void GetFileAnalysis_On_PartialClass_Returns_2()
        {
            var actual = assembliesWithoutPreprocessing.Single(a => a.Name == "Test").Classes.Single(
                    c => c.Name == "ReportGenerator.Tests.TestFiles.Project.PartialClass").Files.Count();

            Assert.AreEqual(2, actual);
        }

        [Test]
        public void ClassesInAssemblyTest()
        {
            Assert.AreEqual(7, assembliesWithoutPreprocessing.SelectMany(a => a.Classes).Count(), "Wrong number of classes");
        }

        [Test]
        public void AssembliesTest()
        {
            Assert.AreEqual(1, assembliesWithoutPreprocessing.Count(), "Wrong number of assemblies");
        }

        [Test]
        public void MethodMetricsTest()
        {
            Assert.AreEqual(0, assembliesWithoutPreprocessing.Single(a => a.Name == "Test").Classes.Single
                (c => c.Name == "ReportGenerator.Tests.TestFiles.Project.TestClass").MethodMetrics.Count(), "Wrong number of metrics");
        }

        [Test]
        public void OpenCoverMethodMetricsTest()
        {
            string filePath = Path.Combine(FileManager.GetCSharpReportDirectory(), "MultiOpenCover.xml");
            var multiReportParser = ParserFactory.CreateParser(new string[] { filePath }, new string[] { });
            //Assert.IsInstanceOfType(multiReportParser, typeof(MultiReportParser), "Wrong type");

            var metrics = multiReportParser.Assemblies.Single(a => a.Name == "Test").Classes.Single(c => c.Name == "ReportGenerator.Tests.TestFiles.Project.TestClass").MethodMetrics;

            Assert.AreEqual(2, metrics.Count(), "Wrong number of method metrics");
            Assert.AreEqual("System.Void Test.TestClass::SampleFunction()", metrics.First().Name, "Wrong name of method");
            Assert.AreEqual(3, metrics.First().Metrics.Count(), "Wrong number of metrics");

            Assert.AreEqual("Cyclomatic Complexity", metrics.First().Metrics.ElementAt(0).Name, "Wrong name of metric");
            Assert.AreEqual(111, metrics.First().Metrics.ElementAt(0).Value, "Wrong value of metric");
            Assert.AreEqual("Sequence Coverage", metrics.First().Metrics.ElementAt(1).Name, "Wrong name of metric");
            Assert.AreEqual(222, metrics.First().Metrics.ElementAt(1).Value, "Wrong value of metric");
            Assert.AreEqual("Branch Coverage", metrics.First().Metrics.ElementAt(2).Name, "Wrong name of metric");
            Assert.AreEqual(333, metrics.First().Metrics.ElementAt(2).Value, "Wrong value of metric");
        }

        [Test]
        public void OpenCoverBranchesTest()
        {
            string filePath = Path.Combine(FileManager.GetCSharpReportDirectory(), "MultiOpenCover.xml");
            var multiReportParser = ParserFactory.CreateParser(new string[] { filePath }, new string[] { });
            //Assert.IsInstanceOfType(multiReportParser, typeof(MultiReportParser), "Wrong type");

            var fileAnalysis = GetFileAnalysis(multiReportParser.Assemblies, "ReportGenerator.Tests.TestFiles.Project.TestClass2", testClassDirectory + "\\TestClass2.cs");

            Assert.IsFalse(fileAnalysis.Lines.Single(l => l.LineNumber == 44).CoveredBranches.HasValue, "No covered branches");
            Assert.IsFalse(fileAnalysis.Lines.Single(l => l.LineNumber == 44).TotalBranches.HasValue, "No total branches");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 45).CoveredBranches.Value, "Wrong number of covered branches");
            Assert.AreEqual(2, fileAnalysis.Lines.Single(l => l.LineNumber == 45).TotalBranches.Value, "Wrong number of total branches");
        }

        private static FileAnalysis GetFileAnalysis(IEnumerable<Assembly> assemblies, string className, string fileName)
        {
            var analysis = assemblies
                .Single(a => a.Name == "Test").Classes
                .Single(c => c.Name == className).Files
                .Single(f => f.Path == fileName)
                .AnalyzeFile();

            return analysis;
        }
    }
}
