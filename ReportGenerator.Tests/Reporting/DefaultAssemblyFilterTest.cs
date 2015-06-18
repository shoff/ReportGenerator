
namespace ReportGenerator.Tests.Reporting
{
    using NUnit.Framework;
    using Palmmedia.ReportGenerator.Reporting;

    /// <summary>
    /// This is a test class for DefaultAssemblyFilter and is intended
    /// to contain all DefaultAssemblyFilter Unit Tests
    /// </summary>
    [TestFixture]
    public class DefaultAssemblyFilterTest
    {
        [Test]
        public void NoFilter_AnyAssembly_AssemblyIsAccepted()
        {
            IAssemblyFilter assemblyFilter = new DefaultAssemblyFilter(new string[] { });

            Assert.IsTrue(assemblyFilter.IsAssemblyIncludedInReport("Test"), "Assembly is expected to be included.");
        }

        [Test]
        public void OnlyIncludes_MatchingAssembly_AssemblyIsAccepted()
        {
            IAssemblyFilter assemblyFilter = new DefaultAssemblyFilter(new[] { "+Test" });

            Assert.IsTrue(assemblyFilter.IsAssemblyIncludedInReport("Test"), "Assembly is expected to be included.");
        }

        [Test]
        public void OnlyIncludes_NotMatchingAssembly_AssemblyIsNotAccepted()
        {
            IAssemblyFilter assemblyFilter = new DefaultAssemblyFilter(new[] { "+Test" });

            Assert.IsFalse(assemblyFilter.IsAssemblyIncludedInReport("Test123"), "Assembly is expected to be excluded.");
        }

        [Test]
        public void OnlyIncludesWithWildcards_MatchingAssembly_AssemblyIsAccepted()
        {
            IAssemblyFilter assemblyFilter = new DefaultAssemblyFilter(new[] { "+Test*" });

            Assert.IsTrue(assemblyFilter.IsAssemblyIncludedInReport("Test"), "Assembly is expected to be included.");
            Assert.IsTrue(assemblyFilter.IsAssemblyIncludedInReport("Test123"), "Assembly is expected to be included.");
        }

        [Test]
        public void OnlyIncludesWithWildcards_NotMatchingAssembly_AssemblyIsNotAccepted()
        {
            IAssemblyFilter assemblyFilter = new DefaultAssemblyFilter(new[] { "+Test*" });

            Assert.IsFalse(assemblyFilter.IsAssemblyIncludedInReport("PrefixTest"), "Assembly is expected to be included.");
            Assert.IsFalse(assemblyFilter.IsAssemblyIncludedInReport("PrefixTest123"), "Assembly is expected to be included.");
        }

        [Test]
        public void IncludesAndExcludes_MatchingAssembly_AssemblyIsAccepted()
        {
            IAssemblyFilter assemblyFilter = new DefaultAssemblyFilter(new[] { "+Test", "-SomeExclude" });

            Assert.IsTrue(assemblyFilter.IsAssemblyIncludedInReport("Test"), "Assembly is expected to be included.");
        }

        [Test]
        public void IncludesAndExcludes_NotMatchingAssembly_AssemblyIsNotAccepted()
        {
            IAssemblyFilter assemblyFilter = new DefaultAssemblyFilter(new[] { "+Test", "-Test" });

            Assert.IsFalse(assemblyFilter.IsAssemblyIncludedInReport("Test"), "Assembly is expected to be excluded.");
        }

        [Test]
        public void IncludesAndExcludesWithWildcards_MatchingAssembly_AssemblyIsAccepted()
        {
            IAssemblyFilter assemblyFilter = new DefaultAssemblyFilter(new[] { "+Test*", "-SomeExclude*" });

            Assert.IsTrue(assemblyFilter.IsAssemblyIncludedInReport("Test"), "Assembly is expected to be included.");
            Assert.IsTrue(assemblyFilter.IsAssemblyIncludedInReport("Test123"), "Assembly is expected to be included.");
        }

        [Test]
        public void IncludesAndExcludesWithWildcards_NotMatchingAssembly_AssemblyIsNotAccepted()
        {
            IAssemblyFilter assemblyFilter = new DefaultAssemblyFilter(new[] { "+Test*", "-Tes*" });

            Assert.IsFalse(assemblyFilter.IsAssemblyIncludedInReport("Test"), "Assembly is expected to be included.");
            Assert.IsFalse(assemblyFilter.IsAssemblyIncludedInReport("PrefixTest123"), "Assembly is expected to be included.");
        }
    }
}
