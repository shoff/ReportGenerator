namespace ReportGenerator.Tests.Parser.Analysis
{
    using NUnit.Framework;
    using Palmmedia.ReportGenerator.Parser.Analysis;

    /// <summary>
    /// This is a test class for Branch and is intended
    /// to contain all Branch Unit Tests
    /// </summary>
    [TestFixture]
    public class BranchTest
    {
        /// <summary>
        /// A test for the Constructor
        /// </summary>
        [Test]
        public void Constructor()
        {
            var sut = new Branch(10, "Test");

            Assert.AreEqual(10, sut.BranchVisits, "Not equal");
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [Test]
        public void Equals()
        {
            var target1 = new Branch(10, "Test");
            var target2 = new Branch(11, "Test");
            var target3 = new Branch(10, "Test123");

            Assert.IsTrue(target1.Equals(target2), "Objects are not equal");
            Assert.IsFalse(target1.Equals(target3), "Objects are equal");
            Assert.IsFalse(target1.Equals(null), "Objects are equal");
            Assert.IsFalse(target1.Equals(new object()), "Objects are equal");
        }
    }
}
