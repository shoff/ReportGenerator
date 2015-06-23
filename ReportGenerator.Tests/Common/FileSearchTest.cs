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
        public void GetFiles_Should_Throw_If_Pattern_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => FileSearch.GetFiles(null));
        }

        [Test]
        public void GetFiles_Should_Throw_If_Pattern_Is_Empty()
        {
            Assert.Throws<ArgumentNullException>(() => FileSearch.GetFiles(string.Empty));
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
        public void GetFiles_On_Empty_Directory_Returns_No_Files_Found()
        {
            if (!Directory.Exists("tmp"))
            {
                Directory.CreateDirectory("tmp");
            }

            var files = FileSearch.GetFiles(Path.Combine("tmp", "*")).ToArray();
            Assert.AreEqual(0, files.Length);

        }

        [Test]
        public void GetFiles_SingleDirectory_XmlFilesFound()
        {
            var cSharpDirectory = CommonNames.ReportDirectory;
            var files = FileSearch.GetFiles(Path.Combine(cSharpDirectory, "*.xml")).ToArray();
            Assert.AreEqual(13, files.Length);
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
