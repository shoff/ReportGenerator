﻿namespace ReportGenerator.Tests.Parser.Preprocessing
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
        private static readonly string CSharpFilePath = Path.Combine(FileManager.GetCSharpReportDirectory(), "OpenCover.xml");

        private static readonly string FSharpFilePath = Path.Combine(FileManager.GetFSharpReportDirectory(), "OpenCover.xml");

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
        public void Execute_SequencePointsOfAutoPropertiesAdded()
        {
            XDocument report = XDocument.Load(CSharpFilePath);

            var classSearcherFactory = new ClassSearcherFactory();
            new OpenCoverReportPreprocessor(report, classSearcherFactory, new ClassSearcher(string.Empty)).Execute();

            Assert.AreEqual(14, report.Descendants("File").Count(), "Wrong number of total files.");

            var gettersAndSetters = report.Descendants("Class")
                .Single(c => c.Element("FullName") != null && c.Element("FullName").Value == "Test.TestClass2")
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

        /// <summary>
        /// A test for Execute
        /// </summary>
        [Test]
        public void Execute_ClassNameAddedToStartupCodeElements()
        {
            XDocument report = XDocument.Load(FSharpFilePath);

            var startupCodeClasses = report.Root
                .Elements("Modules")
                .Elements("Module")
                .Elements("Classes")
                .Elements("Class")
                .Where(c => c.Element("FullName").Value.StartsWith("<StartupCode$"))
                .ToArray();

            Assert.AreEqual(17, startupCodeClasses.Length, "Wrong number of auto generated classes.");

            var classSearcherFactory = new ClassSearcherFactory();
            new OpenCoverReportPreprocessor(report, classSearcherFactory, new ClassSearcher(string.Empty)).Execute();

            var updatedStartupCodeClasses = report.Root
                .Elements("Modules")
                .Elements("Module")
                .Elements("Classes")
                .Elements("Class")
                .Where(c => c.Element("FullName").Value.StartsWith("<StartupCode$"))
                .ToArray();

            Assert.AreEqual(3, updatedStartupCodeClasses.Length, "Wrong number of auto generated classes.");

            for (int i = 2; i < 9; i++)
            {
                Assert.IsTrue(startupCodeClasses[i].Element("FullName").Value.StartsWith("ViewModels.MouseBehavior/"));
            }

            for (int i = 9; i < 16; i++)
            {
                Assert.IsTrue(startupCodeClasses[i].Element("FullName").Value.StartsWith("ViewModels.TestMouseBehavior/"));
            }
        }
    }
}
