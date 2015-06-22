namespace ReportGenerator.Tests.Parser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using NUnit.Framework;
    using Palmmedia.ReportGenerator.Parser;
    using Palmmedia.ReportGenerator.Parser.Analysis;
    using ReportGenerator.Tests.TestHelpers;

    /// <summary>
    /// This is a test class for NCoverParser and is intended
    /// to contain all NCoverParser Unit Tests
    /// </summary>
    [TestFixture]
    public class NCoverParserTest
    {
        private static readonly string filePath = CommonNames.ReportDirectory + "NCover1.5.8.xml";
        private static ICollection<Assembly> assemblies;
        private readonly string projectFile = AppDomain.CurrentDomain.BaseDirectory + "\\TestFiles\\Project";

        [SetUp]
        public void SetUp()
        {
            var report = XDocument.Load(filePath);
            assemblies = new NCoverParser(report).Assemblies;
        }

        [Test]
        public void NumberOfLineVisitsTest()
        {
            var fileAnalysis = FileAnalysisCreator.GetFileAnalysis(assemblies, "ReportGenerator.Tests.TestFiles.Project.TestClass", projectFile + "\\TestClass.cs");

            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 14).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 18).LineVisits, "Wrong number of line visits");

            fileAnalysis = FileAnalysisCreator.GetFileAnalysis(assemblies, "ReportGenerator.Tests.TestFiles.Project.TestClass2", projectFile + "\\TestClass2.cs");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 19).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(2, fileAnalysis.Lines.Single(l => l.LineNumber == 25).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 31).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 37).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(4, fileAnalysis.Lines.Single(l => l.LineNumber == 54).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 81).LineVisits, "Wrong number of line visits");

            fileAnalysis = FileAnalysisCreator.GetFileAnalysis(assemblies, "ReportGenerator.Tests.TestFiles.Project.PartialClass", projectFile + "\\PartialClass.cs");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 9).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 14).LineVisits, "Wrong number of line visits");

            fileAnalysis = FileAnalysisCreator.GetFileAnalysis(assemblies, "ReportGenerator.Tests.TestFiles.Project.PartialClass", projectFile + "\\PartialClass2.cs");
            Assert.AreEqual(1, fileAnalysis.Lines.Single(l => l.LineNumber == 9).LineVisits, "Wrong number of line visits");
            Assert.AreEqual(0, fileAnalysis.Lines.Single(l => l.LineNumber == 14).LineVisits, "Wrong number of line visits");
        }

        [Test]
        public void NumberOfFilesTest()
        {
            Assert.AreEqual(5, assemblies.SelectMany(a => a.Classes).SelectMany(a => a.Files).Distinct().Count(), "Wrong number of files");
        }

        [Test]
        public void FilesOfClassTest()
        {
            Assert.AreEqual(1, assemblies.Single(a => a.Name == "ReportGenerator.Tests").Classes.Single(c => c.Name == "ReportGenerator.Tests.TestFiles.Project.TestClass").Files.Count(), "Wrong number of files");

            Assert.AreEqual(2, assemblies.Single(a => a.Name == "ReportGenerator.Tests").Classes.Single(c => c.Name == "ReportGenerator.Tests.TestFiles.Project.PartialClass").Files.Count(), "Wrong number of files");
        }

        [Test]
        public void ClassesInAssemblyTest()
        {
            Assert.AreEqual(4, assemblies.SelectMany(a => a.Classes).Count(), "Wrong number of classes");
        }

        [Test]
        public void AssembliesTest()
        {
            Assert.AreEqual(1, assemblies.Count(), "Wrong number of assemblies");
        }

        [Test]
        public void MethodMetricsTest()
        {
            Assert.AreEqual(0, assemblies.Single(a => a.Name == "ReportGenerator.Tests").Classes.Single(c => c.Name == "ReportGenerator.Tests.TestFiles.Project.TestClass").MethodMetrics.Count(), "Wrong number of metrics");
        }
    }
}
