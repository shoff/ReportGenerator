namespace ReportGenerator.Tests.Parser.Preprocessing
{
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using NUnit.Framework;
    using Palmmedia.ReportGenerator.Parser.Preprocessing;
    using Palmmedia.ReportGenerator.Parser.Preprocessing.FileSearch;

    /// <summary>
    /// This is a test class for OpenCoverReportPreprocessor and is intended
    /// to contain all OpenCoverReportPreprocessor Unit Tests
    /// </summary>
    [TestFixture]
    public class OpenCoverReportPreprocessorTest
    {
        private static readonly string CSharpFilePath = CommonNames.ReportDirectory + "OpenCover.xml";
        
        /// <summary>
        /// A test for Execute
        /// </summary>
        [Test]
        public void Execute_SequencePointsOfAutoPropertiesAdded()
        {
            XDocument report = XDocument.Load(CSharpFilePath);

            var classSearcherFactory = new ClassSearcherFactory();
            new OpenCoverReportPreprocessor(report, classSearcherFactory, new ClassSearcher(string.Empty)).Execute();

            Assert.AreEqual(14, report.Descendants("File").Count(), "Wrong number of total files.");

            var gettersAndSetters = report.Descendants("Class")
                .Single(c => c.Element("FullName") != null && c.Element("FullName").Value == CommonNames.TestNamespace + "TestClass2")
                .Elements("Methods")
                .Elements("Method")
                .Where(m => m.Attribute("isGetter").Value == "true" || m.Attribute("isSetter").Value == "true");

            foreach (var getterOrSetter in gettersAndSetters)
            {
                Assert.IsTrue(getterOrSetter.Element("FileRef") != null);
                Assert.IsTrue(getterOrSetter.Element("SequencePoints") != null);

                var sequencePoints = getterOrSetter.Element("SequencePoints").Elements("SequencePoint");
                Assert.AreEqual(1, sequencePoints.Count(), "Wrong number of sequence points.");
                Assert.AreEqual(getterOrSetter.Element("MethodPoint").Attribute("vc").Value, sequencePoints.First().Attribute("vc").Value, "Getter or setter should have been visited.");
            }
        }

    }
}
