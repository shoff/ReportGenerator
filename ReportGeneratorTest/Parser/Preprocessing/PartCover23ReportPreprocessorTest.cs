﻿using System.IO;
using System.Linq;
using System.Xml.Linq;

using Palmmedia.ReportGenerator.Parser.Preprocessing;
using Palmmedia.ReportGenerator.Parser.Preprocessing.FileSearch;

namespace Palmmedia.ReportGeneratorTest.Parser.Preprocessing
{
    using NUnit.Framework;

    /// <summary>
    /// This is a test class for PartCover23ReportPreprocessor and is intended
    /// to contain all PartCover23ReportPreprocessor Unit Tests
    /// </summary>
    [TestFixture]
    public class PartCover23ReportPreprocessorTest
    {
        private static readonly string FilePath = Path.Combine(FileManager.GetCSharpReportDirectory(), "Partcover2.3.0.35109.xml");

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        // Use ClassInitialize to run code before running the first test in the class
        [SetUp]
        public static void MyClassInitialize(TestContext testContext)
        {
            FileManager.CopyTestClasses();
        }

        // Use ClassCleanup to run code after all tests in a class have run
        [TearDown]
        public static void MyClassCleanup()
        {
            FileManager.DeleteTestClasses();
        }

        #endregion

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
                .Single(c => c.Attribute("name").Value == "Test.TestClass2")
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
                .Single(c => c.Attribute("name").Value == "Test.TestClass2")
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
