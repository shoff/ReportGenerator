namespace ReportGenerator.Tests.Common
{
    using System;
    using System.IO;
    using System.Linq;
    using NUnit.Framework;
    using Palmmedia.ReportGenerator.Common;

    /// <summary>
    /// This is a test class for FileSearch and is intended
    /// to contain all FileSearch Unit Tests
    /// </summary>
    [TestFixture]
    public class FileSearchTest
    {
        [Test]
        public void GetFiles_FilePatternNull_ArgumentException()
        {

            Assert.Throws<ArgumentException>(() => FileSearch.GetFiles(null).ToArray());
        }

        [Test]
        public void GetFiles_FilePatternEmtpy_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => FileSearch.GetFiles(string.Empty).ToArray());
        }

        [Test]
        public void GetFiles_FilePatternInvalid_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => FileSearch.GetFiles("\"").ToArray());
        }

        [Test]
        public void GetFiles_OnlyDriveWithoutFilePattern_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => FileSearch.GetFiles("C:\\").ToArray());
        }

        [Test]
        public void GetFiles_OnlyUNCPathWithoutFilePattern_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => FileSearch.GetFiles("\\test").ToArray());
        }

        [Test]
        public void GetFiles_EmptyDirectory_NoFilesFound()
        {
            Directory.CreateDirectory("tmp");

            var files = FileSearch.GetFiles(Path.Combine("tmp", "*")).ToArray();
            Assert.AreEqual(0, files.Length);

            Directory.Delete("tmp");
        }

        [Test]
        public void GetFiles_SingleDirectory_XmlFilesFound()
        {
            var cSharpDirectory = FileManager.GetCSharpReportDirectory();
            var files = FileSearch.GetFiles(Path.Combine(cSharpDirectory, "*.xml")).ToArray();
            Assert.AreEqual(13, files.Length);
        }

        [Test, Ignore]
        public void GetFiles_MultiDirectory_AllFilesFound()
        {
            var files = FileSearch.GetFiles(Path.Combine(FileManager.GetFilesDirectory(), "*", "*", "*")).ToArray();
            Assert.IsTrue(files.Length >= 39);
        }

        [Test, Ignore]
        public void GetFiles_MultiDirectory_MatchingFilesFound()
        {
            var files = FileSearch.GetFiles(Path.Combine(FileManager.GetFilesDirectory(), "CSharp", "*roject*", "*lyzer*.cs")).ToArray();
            Assert.AreEqual(1, files.Length);
        }

        [Test]
        public void GetFiles_RelativePath_DllFound()
        {
            var files = FileSearch.GetFiles("..\\*\\*.dll").ToArray();
            Assert.IsTrue(files.Any(f => f.EndsWith(this.GetType().Assembly.GetName().Name + ".dll", StringComparison.OrdinalIgnoreCase)));
        }

        [Test]
        public void GetFiles_UncPath_NoFilesFound()
        {
            var files = FileSearch.GetFiles(@"\\DoesNotExist\*.xml").ToArray();
            Assert.AreEqual(0, files.Length);
        }
    }
}
