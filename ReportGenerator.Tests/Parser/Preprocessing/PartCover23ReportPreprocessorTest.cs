namespace ReportGenerator.Tests.Parser.Preprocessing
{
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using NUnit.Framework;
    using Palmmedia.ReportGenerator.Parser.Preprocessing;
    using Palmmedia.ReportGenerator.Parser.Preprocessing.FileSearch;

    /// <summary>
    /// This is a test class for PartCover23ReportPreprocessor and is intended
    /// to contain all PartCover23ReportPreprocessor Unit Tests
    /// </summary>
    [TestFixture]
    public class PartCover23ReportPreprocessorTest
    {
        private static readonly string FilePath = CommonNames.ReportDirectory + "Partcover2.3.0.35109.xml";

        /// <summary>
        /// A test for Execute
        /// </summary>
        [Test]
        public void Execute_SequencePointsOfAutoPropertiesAndCoverageDataOfUnexecutedMethodsAdded()
        {
            XDocument report = XDocument.Load(FilePath);

            var classSearcherFactory = new ClassSearcherFactory();
            new PartCover23ReportPreprocessor(report, classSearcherFactory, new ClassSearcher(string.Empty)).Execute();

            Assert.AreEqual(8, report.Root.Elements("File").Count(), "Wrong number of total files.");

            var gettersAndSetters = report.Root.Elements("Type")
                .Single(c => c.Attribute("name").Value == CommonNames.TestNamespace + "TestClass2")
                .Elements("Method")
                .Where(m => m.Attribute("name").Value.StartsWith("get_") || m.Attribute("name").Value.StartsWith("set_"))
                .Select(c => c.Element("pt"));

            Assert.IsTrue(gettersAndSetters.Any());

            foreach (var getterOrSetter in gettersAndSetters)
            {
                Assert.IsTrue(getterOrSetter.Attribute("fid") != null);
                Assert.IsTrue(getterOrSetter.Attribute("sl") != null);
            }

            var unexecutedMethod = report.Root.Elements("Type")
                .Single(c => c.Attribute("name").Value == CommonNames.TestNamespace + "TestClass2")
                .Elements("Method")
                .Single(m => m.Attribute("name").Value == "UnExecutedMethod");

            Assert.AreEqual(4, unexecutedMethod.Elements("pt").Count(), "Wrong number of sequence points.");

            foreach (var sequencePoint in unexecutedMethod.Elements("pt"))
            {
                Assert.IsTrue(sequencePoint.Attribute("fid") != null);
                Assert.IsTrue(sequencePoint.Attribute("sl") != null);
            }
        }
    }
}
