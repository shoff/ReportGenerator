
namespace ReportGenerator.Tests.Parser.Preprocessing.FileSearch
{
    using NUnit.Framework;
    using Palmmedia.ReportGenerator.Parser.Preprocessing.FileSearch;

    /// <summary>
    /// This is a test class for ClassSearcherFactory and is intended
    /// to contain all ClassSearcherFactory Unit Tests
    /// </summary>
    [TestFixture]
    public class ClassSearcherFactoryTests
    {
        private ClassSearcherFactory classSearcherFactory;

        [SetUp]
        public void SetUp()
        {
            this.classSearcherFactory = new ClassSearcherFactory();
        }

        [Test]
        public void CreateClassSearcher_Pass_Null_ClassSearcher_With_Null_Directory_Is_Returned()
        {
            var classSearcher = this.classSearcherFactory.CreateClassSearcher((string)null);
            Assert.IsNotNull(classSearcher, "ClassSearcher must not be null");
            Assert.IsNull(classSearcher.Directory, "ClassSearcher directory must be null");
        }

        [Test]
        public void CreateClassSearcher_Pass_Subdirectory_Cached_Instance_Is_Returned()
        {

            var classSearcher1 = this.classSearcherFactory.CreateClassSearcher("C:\\temp");
            var classSearcher2 = this.classSearcherFactory.CreateClassSearcher("C:\\temp\\sub");
            Assert.AreSame(classSearcher1, classSearcher2, "ClassSearchers are not the same instance.");
        }

        [Test]
        public void CreateClassSearcher_Pass_Parent_Directory_New_Instance_Is_Returned()
        {
            var classSearcher1 = this.classSearcherFactory.CreateClassSearcher("C:\\temp");
            var classSearcher2 = this.classSearcherFactory.CreateClassSearcher("C:\\");
            Assert.AreNotSame(classSearcher1, classSearcher2, "ClassSearchers are the same instance.");
        }
    }
}
