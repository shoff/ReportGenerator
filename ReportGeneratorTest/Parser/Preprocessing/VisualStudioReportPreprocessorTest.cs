﻿using System.IO;
using System.Linq;
using System.Xml.Linq;

using Palmmedia.ReportGenerator.Parser.Preprocessing;

namespace Palmmedia.ReportGeneratorTest.Parser.Preprocessing
{
    using NUnit.Framework;

    /// <summary>
    /// This is a test class for VisualStudioReportPreprocessor and is intended
    /// to contain all VisualStudioReportPreprocessor Unit Tests
    /// </summary>
    [TestFixture]
    public class VisualStudioReportPreprocessorTest
    {
        private static readonly string FSharpFilePath = Path.Combine(FileManager.GetFSharpReportDirectory(), "VisualStudio2010.coveragexml");

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
        public void Execute_ClassNameAddedToStartupCodeElements()
        {
            XDocument report = XDocument.Load(FSharpFilePath);

            var startupCodeClasses = report.Root
                .Elements("Module")
                .Elements("NamespaceTable")
                .Where(c => c.Element("NamespaceName").Value.StartsWith("<StartupCode$"))
                .Elements("Class")
                .ToArray();

            Assert.AreEqual(15, startupCodeClasses.Length, "Wrong number of auto generated classes.");

            new VisualStudioReportPreprocessor(report).Execute();

            var updatedStartupCodeClasses = report.Root
                .Elements("Module")
                .Elements("NamespaceTable")
                .Where(c => c.Element("NamespaceName").Value.StartsWith("<StartupCode$"))
                .Elements("Class")
                .ToArray();

            Assert.AreEqual(0, updatedStartupCodeClasses.Length, "Wrong number of auto generated classes.");

            Assert.IsTrue(startupCodeClasses[0].Element("ClassName").Value.Equals("$Module1"));

            foreach (int index in new[] { 5, 6, 8, 9, 10, 11, 13 })
            {
                Assert.IsTrue(startupCodeClasses[index].Element("ClassName").Value.Equals("MouseBehavior"));
                Assert.IsTrue(startupCodeClasses[index].Parent.Element("NamespaceName").Value.Equals("ViewModels"));
            }

            foreach (int index in new[] { 1, 2, 3, 4, 7, 12, 14 })
            {
                Assert.IsTrue(startupCodeClasses[index].Element("ClassName").Value.Equals("TestMouseBehavior"));
                Assert.IsTrue(startupCodeClasses[index].Parent.Element("NamespaceName").Value.Equals("ViewModels"));
            }
        }
    }
}
