namespace ReportGenerator.Tests.Parser
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using NUnit.Framework;
    using Palmmedia.ReportGenerator.Parser;
    using Palmmedia.ReportGenerator.Parser.Analysis;
    using ReportGenerator.Tests.TestHelpers;

    [TestFixture]
    public class DynamicCodeCoverageParserTest
    {
        [SetUp]
        public void SetUp()
        {
            var report = XDocument.Load(filePath);
            this.dynamicCodeCoverageParser = new DynamicCodeCoverageParser(report);
            this.assemblies = this.dynamicCodeCoverageParser.Assemblies;
        }

        private readonly string filePath = CommonNames.ReportDirectory + "DynamicCodeCoverage.xml";
        private ICollection<Assembly> assemblies;
        private DynamicCodeCoverageParser dynamicCodeCoverageParser;

        [Test]
        public void AssembliesTest()
        {
            Assert.AreEqual(1, assemblies.Count());
        }

        [Test]
        public void ClassesInAssemblyTest()
        {
            Assert.AreEqual(12, assemblies.SelectMany(a => a.Classes).Count(), "Wrong number of classes");
        }

        [Test]
        public void FilesOfClassTest()
        {
            Assert.AreEqual(
                1, 
                assemblies.Single(a => a.Name == "ReportGenerator.Tests").Classes.Single(c => c.Name == "TestClass").Files.Count(), 
                "Wrong number of files");
            Assert.AreEqual(
                2, 
                assemblies.Single(a => a.Name == "ReportGenerator.Tests").Classes.Single(c => c.Name == "PartialClass").Files.Count
                    (), 
                "Wrong number of files");
        }

        [Test]
        public void GetCoverableLinesOfClassTest()
        {
            Assert.AreEqual(
                4, 
                assemblies.Single(a => a.Name == "ReportGenerator.Tests").Classes.Single(c => c.Name == "AbstractClass")
                    .CoverableLines, 
                "Wrong Coverable Lines");
        }

        [Test]
        public void MethodMetricsTest()
        {
            var metrics =
                assemblies.Single(a => a.Name == "ReportGenerator.Tests").Classes.Single(c => c.Name == "TestClass").MethodMetrics;

            Assert.AreEqual(2, metrics.Count(), "Wrong number of method metrics");
            Assert.AreEqual("SampleFunction()", metrics.First().Name, "Wrong name of method");
            Assert.AreEqual(2, metrics.First().Metrics.Count(), "Wrong number of metrics");

            Assert.AreEqual("Blocks covered", metrics.First().Metrics.ElementAt(0).Name, "Wrong name of metric");
            Assert.AreEqual(9, metrics.First().Metrics.ElementAt(0).Value, "Wrong value of metric");
            Assert.AreEqual("Blocks not covered", metrics.First().Metrics.ElementAt(1).Name, "Wrong name of metric");
            Assert.AreEqual(4, metrics.First().Metrics.ElementAt(1).Value, "Wrong value of metric");
        }

        [Test]
        public void NumberOfFilesTest()
        {
            Assert.AreEqual(
                10, 
                assemblies.SelectMany(a => a.Classes).SelectMany(a => a.Files).Distinct().Count(), 
                "Wrong number of files");
        }

        [Test]
        public void NumberOfLineVisitsTest()
        {
            var fileAnalysis = FileAnalysisCreator.GetFileAnalysis(assemblies, "TestClass", CommonNames.CodeDirectory + "TestClass.cs");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 9).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 10).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 11).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 12).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 19).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 23).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 31).LineVisits, "Wrong number of line visits");

            fileAnalysis = FileAnalysisCreator.GetFileAnalysis(assemblies, "TestClass2", CommonNames.CodeDirectory + "TestClass2.cs");
            Assert.AreEqual(-1, fileAnalysis.Lines.Single(l => l.LineNumber == 13).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(-1, fileAnalysis.Lines.Single(l => l.LineNumber == 15).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 19).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 25).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 31).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 37).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 54).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 81).LineVisits, "Wrong number of line visits");

            fileAnalysis = FileAnalysisCreator.GetFileAnalysis(
                assemblies, 
                "PartialClass", 
                CommonNames.CodeDirectory + "PartialClass.cs");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 9).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 14).LineVisits, "Wrong number of line visits");

            fileAnalysis = FileAnalysisCreator.GetFileAnalysis(
                assemblies, 
                "PartialClass", 
                CommonNames.CodeDirectory + "PartialClass2.cs");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 9).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 14).LineVisits, "Wrong number of line visits");
        }
    }
}