namespace ReportGenerator.Tests.Parser.Preprocessing.FileSearch
{
    using System.Linq;
    using NUnit.Framework;
    using Palmmedia.ReportGenerator.Parser.Preprocessing.FileSearch;

    /// <summary>
    /// This is a test class for ClassSearcher and is intended
    /// to contain all ClassSearcher Unit Tests
    /// </summary>
    [TestFixture]
    public class ClassSearcherTest
    {
        private static ClassSearcher classSearcher;

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        // Use ClassInitialize to run code before running the first test in the class
        [SetUp]
        public static void MyClassInitialize(TestContext testContext)
        {
            FileManager.CopyTestClasses();

            classSearcher = new ClassSearcher("C:\\temp");
        }

        // Use ClassCleanup to run code after all tests in a class have run
        [TearDown]
        public static void MyClassCleanup()
        {
            FileManager.DeleteTestClasses();
        }

        // Use TestInitialize to run code before running each test
        // [SetUp]
        // public void MyTestInitialize()
        // {
        // }

        // Use TestCleanup to run code after each test has run
        // [TearDown]
        // public void MyTestCleanup()
        // {
        // }
        #endregion

        /// <summary>
        /// A test for GetFilesOfClass
        /// </summary>
        [Test]
        public void GetFilesOfClass_PartialClassWith2Files_2FilesFound()
        {
            var files = classSearcher.GetFilesOfClass("Test.PartialClass");

            Assert.IsNotNull(files, "Files must not be null.");
            Assert.IsTrue(files.Contains("C:\\temp\\PartialClass.cs"), "Files does not contain expected file");
            Assert.IsTrue(files.Contains("C:\\temp\\PartialClass2.cs"), "Files does not contain expected file");
        }

        /// <summary>
        /// A test for GetFilesOfClass
        /// </summary>
        [Test]
        public void GetFilesOfClass_NestedClass_1FileFound()
        {
            var files = classSearcher.GetFilesOfClass("Test.TestClassNestedClass");

            Assert.IsNotNull(files, "Files must not be null.");
            Assert.IsTrue(files.Contains("C:\\temp\\TestClass.cs"), "Files does not contain expected file");
        }

        /// <summary>
        /// A test for GetFilesOfClass
        /// </summary>
        [Test]
        public void GetFilesOfClass_NotExistingClass_0FilesFound()
        {
            var files = classSearcher.GetFilesOfClass("Test.Test123");

            Assert.IsNotNull(files, "Files must not be null.");
            Assert.IsFalse(files.Any());
        }
    }
}
