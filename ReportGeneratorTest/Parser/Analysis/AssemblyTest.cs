using System.Linq;

using Palmmedia.ReportGenerator.Parser.Analysis;

namespace Palmmedia.ReportGeneratorTest.Parser.Analysis
{
    using NUnit.Framework;

    /// <summary>
    /// This is a test class for Assembly and is intended
    /// to contain all Assembly Unit Tests
    /// </summary>
    [TestFixture]
    public class AssemblyTest
    {
        /// <summary>
        /// A test for the Constructor
        /// </summary>
        [Test]
        public void Constructor()
        {
            string assemblyName = "C:\\test\\TestAssembly.dll";

            var sut = new Assembly(assemblyName);

            Assert.AreEqual(assemblyName, sut.Name, "Not equal");
            Assert.AreEqual("TestAssembly.dll", sut.ShortName, "Not equal");
        }

        /// <summary>
        /// A test for AddClass
        /// </summary>
        [Test]
        public void AddClass_AddSingleClass_ClassIsStored()
        {
            var sut = new Assembly("C:\\test\\TestAssembly.dll");
            var @class = new Class("Test", sut);

            sut.AddClass(@class);

            Assert.AreEqual(@class, sut.Classes.First(), "Not equal");
            Assert.AreEqual(1, sut.Classes.Count(), "Wrong number of classes");
        }

        /// <summary>
        /// A test for Merge
        /// </summary>
        [Test]
        public void Merge_MergeAssemblyWithOneClass_ClassIsStored()
        {
            var sut = new Assembly("C:\\test\\TestAssembly.dll");
            var assemblyToMerge = new Assembly("C:\\test\\TestAssembly.dll");
            var @class = new Class("Test", sut);
            assemblyToMerge.AddClass(@class);

            sut.Merge(assemblyToMerge);

            Assert.AreEqual(@class, sut.Classes.First(), "Not equal");
            Assert.AreEqual(1, sut.Classes.Count(), "Wrong number of classes");
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [Test]
        public void Equals()
        {
            string assemblyName = "C:\\test\\TestAssembly.dll";

            var target1 = new Assembly(assemblyName);
            var target2 = new Assembly(assemblyName);
            var target3 = new Assembly("Test.dll");

            Assert.IsTrue(target1.Equals(target2), "Objects are not equal");
            Assert.IsFalse(target1.Equals(target3), "Objects are equal");
            Assert.IsFalse(target1.Equals(null), "Objects are equal");
            Assert.IsFalse(target1.Equals(new object()), "Objects are equal");
        }
    }
}
