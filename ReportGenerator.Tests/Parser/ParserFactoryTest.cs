namespace ReportGenerator.Tests.Parser
{
    using System.IO;
    using NUnit.Framework;
    using Palmmedia.ReportGenerator.Parser;

    
    [TestFixture]
    public class ParserFactoryTest
    {
        
        [Test]
        public void CreateParser_SingleReportFileWithSingleReport_CorrectParserIsReturned()
        {
            string filePath = CommonNames.ReportDirectory + "Partcover2.3.xml";
            string parserName = ParserFactory.CreateParser(new[] { filePath }, new string[] { }).ToString();
            Assert.AreEqual("PartCover23Parser", parserName, "Wrong parser");

            filePath = CommonNames.ReportDirectory + "Partcover2.2.xml";
            parserName = ParserFactory.CreateParser(new[] { filePath }, new string[] { }).ToString();
            Assert.AreEqual("PartCover22Parser", parserName, "Wrong parser");

            filePath = CommonNames.ReportDirectory + "NCover1.5.8.xml";
            parserName = ParserFactory.CreateParser(new[] { filePath }, new string[] { }).ToString();
            Assert.AreEqual("NCoverParser", parserName, "Wrong parser");

            filePath = CommonNames.ReportDirectory + "OpenCover.xml";
            parserName = ParserFactory.CreateParser(new[] { filePath }, new string[] { }).ToString();
            Assert.AreEqual("OpenCoverParser", parserName, "Wrong parser");

            filePath = CommonNames.ReportDirectory + "VisualStudio2010.coveragexml";
            parserName = ParserFactory.CreateParser(new[] { filePath }, new string[] { }).ToString();
            Assert.AreEqual("VisualStudioParser", parserName, "Wrong parser");

            filePath = CommonNames.ReportDirectory + "DynamicCodeCoverage.xml";
            parserName = ParserFactory.CreateParser(new[] { filePath }, new string[] { }).ToString();
            Assert.AreEqual("DynamicCodeCoverageParser", parserName, "Wrong parser");
        }

        [Test, Ignore("We're not compiling that assembly will revisist with another one")]
        public void CreateParser_SingleReportFileWithSeveralReports_CorrectParserIsReturned()
        {
            string filePath = CommonNames.ReportDirectory + "MultiPartcover2.3.xml";
            string parserName = ParserFactory.CreateParser(new[] { filePath }, new string[] { }).ToString();
            Assert.AreEqual("MultiReportParser (2x PartCover23Parser)", parserName, "Wrong parser");

            filePath = CommonNames.ReportDirectory + "MultiPartcover2.2.xml";
            parserName = ParserFactory.CreateParser(new[] { filePath }, new string[] { }).ToString();
            Assert.AreEqual("MultiReportParser (2x PartCover22Parser)", parserName, "Wrong parser");

            filePath = CommonNames.ReportDirectory + "MultiNCover1.5.8.xml";
            parserName = ParserFactory.CreateParser(new[] { filePath }, new string[] { }).ToString();
            Assert.AreEqual("MultiReportParser (2x NCoverParser)", parserName, "Wrong parser");

            filePath = CommonNames.ReportDirectory + "MultiOpenCover.xml";
            parserName = ParserFactory.CreateParser(new[] { filePath }, new string[] { }).ToString();
            Assert.AreEqual("MultiReportParser (2x OpenCoverParser)", parserName, "Wrong parser");

            filePath = CommonNames.ReportDirectory + "MultiVisualStudio2010.coveragexml";
            parserName = ParserFactory.CreateParser(new[] { filePath }, new string[] { }).ToString();
            Assert.AreEqual("MultiReportParser (2x VisualStudioParser)", parserName, "Wrong parser");

            filePath = CommonNames.ReportDirectory + "MultiDynamicCodeCoverage.xml";
            parserName = ParserFactory.CreateParser(new[] { filePath }, new string[] { }).ToString();
            Assert.AreEqual("MultiReportParser (2x DynamicCodeCoverageParser)", parserName, "Wrong parser");
        }
        
        [Test]
        public void CreateParser_SeveralReportFilesWithSingleReport_CorrectParserIsReturned()
        {
            string filePath = CommonNames.ReportDirectory + "Partcover2.2.xml";
            string filePath2 = CommonNames.ReportDirectory + "Partcover2.3.xml";
            string parserName = ParserFactory.CreateParser(new[] { filePath, filePath2 }, new string[] { }).ToString();
            Assert.AreEqual("MultiReportParser (1x PartCover22Parser, 1x PartCover23Parser)", parserName, "Wrong parser");
        }
        
        [Test, Ignore("We're not compiling that assembly will revisist with another one")]
        public void CreateParser_SeveralReportFilesWithSeveralReports_CorrectParserIsReturned()
        {
            string filePath = CommonNames.ReportDirectory + "Partcover2.2.xml";
            string filePath2 = CommonNames.ReportDirectory + "MultiPartcover2.3.xml";

            string parserName = ParserFactory.CreateParser(new[] { filePath, filePath2 }, new string[] { }).ToString();

            Assert.AreEqual("MultiReportParser (1x PartCover22Parser, 2x PartCover23Parser)", parserName, "Wrong parser");
        }

        [Test]
        public void CreateParser_NoReports_CorrectParserIsReturned()
        {
            string parserName = ParserFactory.CreateParser(new[] { string.Empty }, new string[] { }).ToString();
            Assert.AreEqual(string.Empty, parserName, "Wrong parser");
        }
    }
}
