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
        private ClassSearcher classSearcher;

        [SetUp]
        public void SetUp()
        {
            this.classSearcher = new ClassSearcher(CommonNames.CodeDirectory);
        }

        /// <summary>
        /// A test for GetFilesOfClass
        /// </summary>
        [Test]
        public void GetFilesOfClass_PartialClassWith2Files_2FilesFound()
        {
            var files = classSearcher.GetFilesOfClass(CommonNames.TestNamespace + "PartialClass");

            Assert.IsNotNull(files, "Files must not be null.");
            Assert.IsTrue(files.Contains(CommonNames.CodeDirectory + "PartialClass.cs"), "Files does not contain expected file");
            Assert.IsTrue(files.Contains(CommonNames.CodeDirectory + "PartialClass2.cs"), "Files does not contain expected file");
        }

        /// <summary>
        /// A test for GetFilesOfClass
        /// </summary>
        [Test]
        public void GetFilesOfClass_NestedClass_1FileFound()
        {
            var files = classSearcher.GetFilesOfClass(CommonNames.TestNamespace + "TestClassNestedClass");

            Assert.IsNotNull(files, "Files must not be null.");
            Assert.IsTrue(files.Contains(CommonNames.CodeDirectory + "TestClass.cs"), "Files does not contain expected file");
        }

        /// <summary>
        /// A test for GetFilesOfClass
        /// </summary>
        [Test]
        public void GetFilesOfClass_NotExistingClass_0FilesFound()
        {
            var files = classSearcher.GetFilesOfClass(CommonNames.TestNamespace + "Test123");

            Assert.IsNotNull(files, "Files must not be null.");
            Assert.IsFalse(files.Any());
        }
    }
}
