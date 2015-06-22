namespace ReportGenerator.Tests.Parser.Analysis
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Palmmedia.ReportGenerator.Parser.Analysis;

    /// <summary>
    /// This is a test class for CodeFile and is intended
    /// to contain all CodeFile Unit Tests
    /// </summary>
    [TestFixture]
    public class CodeFileTest
    {

        /// <summary>
        /// A test for the Constructor
        /// </summary>
        [Test]
        public void Constructor()
        {
            var sut = new CodeFile(CommonNames.CodeDirectory + "Program.cs", new int[] { -1, 0, 2 });

            Assert.AreEqual(2, sut.CoverableLines, "Not equal");
            Assert.AreEqual(1, sut.CoveredLines, "Not equal");

            Assert.IsNull(sut.CoveredBranches, "Not null");
            Assert.IsNull(sut.TotalBranches, "Not null");
        }

        /// <summary>
        /// A test for the Constructor
        /// </summary>
        [Test]
        public void Constructor_WithBranches()
        {
            var branches = new Dictionary<int, List<Branch>>()
            {
                { 1, new List<Branch>() { new Branch(1, "1"), new Branch(0, "2") } },
                { 2, new List<Branch>() { new Branch(0, "3"), new Branch(2, "4") } }
            };

            var sut = new CodeFile(CommonNames.CodeDirectory + "Program.cs", new int[] { -1, -1, -1, 0, 0, 0, 1, 1, 1 }, branches);

            Assert.AreEqual(2, sut.CoveredBranches, "Not equal");
            Assert.AreEqual(4, sut.TotalBranches, "Not equal");
        }

        /// <summary>
        /// A test for Merge
        /// </summary>
        [Test]
        public void Merge_CodeFileToMergeHasNoBranches_BranchCoverageInformationIsUpdated()
        {
            var branches = new Dictionary<int, List<Branch>>()
            {
                { 1, new List<Branch>() { new Branch(1, "1"), new Branch(0, "2") } },
                { 2, new List<Branch>() { new Branch(0, "3"), new Branch(2, "4") } }
            };
            var sut = new CodeFile(CommonNames.CodeDirectory + "Program.cs", new int[] { -1, -1, -1, 0, 0, 0, 1, 1, 1 }, branches);

            var codeFileToMerge = new CodeFile(CommonNames.CodeDirectory + "Program.cs", new int[] { -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1 });

            sut.Merge(codeFileToMerge);

            Assert.AreEqual(2, sut.CoveredBranches, "Not equal");
            Assert.AreEqual(4, sut.TotalBranches, "Not equal");
        }

        /// <summary>
        /// A test for Merge
        /// </summary>
        [Test]
        public void Merge_TargetCodeFileHasNoBranches_BranchCoverageInformationIsUpdated()
        {
            var branches = new Dictionary<int, List<Branch>>()
            {
                { 1, new List<Branch>() { new Branch(1, "1"), new Branch(0, "2") } },
                { 2, new List<Branch>() { new Branch(0, "3"), new Branch(2, "4") } }
            };
            var sut = new CodeFile(CommonNames.CodeDirectory + "Program.cs", new int[] { -1, -1, -1, 0, 0, 0, 1, 1, 1 });

            var codeFileToMerge = new CodeFile(CommonNames.CodeDirectory + "Program.cs", new int[] { -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1 }, branches);

            sut.Merge(codeFileToMerge);

            Assert.AreEqual(2, sut.CoveredBranches, "Not equal");
            Assert.AreEqual(4, sut.TotalBranches, "Not equal");
        }

        /// <summary>
        /// A test for Merge
        /// </summary>
        [Test]
        public void Merge_MergeCodeFileWithEqualLengthCoverageArray_CoverageInformationIsUpdated()
        {
            var sut = new CodeFile(CommonNames.CodeDirectory + "Program.cs", new int[] { -1, -1, -1, 0, 0, 0, 1, 1, 1 });
            var codeFileToMerge = new CodeFile(CommonNames.CodeDirectory + "Program.cs", new int[] { -1, 0, 1, -1, 0, 1, -1, 0, 1 });
            var testMethod = new TestMethod("TestFull", "Test");
            codeFileToMerge.AddCoverageByTestMethod(testMethod, new int[] { -1, -1, -1, 0, 0, 0, 1, 1, 1 });

            sut.Merge(codeFileToMerge);

            Assert.AreEqual(8, sut.CoverableLines, "Not equal");
            Assert.AreEqual(5, sut.CoveredLines, "Not equal");

            Assert.IsNull(sut.CoveredBranches, "Not null");
            Assert.IsNull(sut.TotalBranches, "Not null");

            Assert.IsTrue(sut.TestMethods.Contains(testMethod));
        }

        /// <summary>
        /// A test for Merge
        /// </summary>
        [Test]
        public void Merge_MergeCodeFileWithLongerCoverageArray_CoverageInformationIsUpdated()
        {
            var branches = new Dictionary<int, List<Branch>>()
            {
                { 1, new List<Branch>() { new Branch(1, "1"), new Branch(0, "2") } },
                { 2, new List<Branch>() { new Branch(0, "3"), new Branch(2, "4") } }
            };
            var sut = new CodeFile(CommonNames.CodeDirectory + "Program.cs", new int[] { -1, -1, -1, 0, 0, 0, 1, 1, 1 }, branches);

            var branches2 = new Dictionary<int, List<Branch>>()
            {
                { 1, new List<Branch>() { new Branch(4, "1"), new Branch(3, "5") } },
                { 3, new List<Branch>() { new Branch(0, "3"), new Branch(2, "4") } }
            };
            var codeFileToMerge = new CodeFile(CommonNames.CodeDirectory + "Program.cs", new int[] { -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1 }, branches2);

            sut.Merge(codeFileToMerge);

            Assert.AreEqual(10, sut.CoverableLines, "Not equal");
            Assert.AreEqual(6, sut.CoveredLines, "Not equal");

            Assert.AreEqual(4, sut.CoveredBranches, "Not equal");
            Assert.AreEqual(7, sut.TotalBranches, "Not equal");
        }

        /// <summary>
        /// A test for AnalyzeFile
        /// </summary>
        [Test]
        public void AnalyzeFile_ExistingFile_AnalysisIsReturned()
        {
            var sut = new CodeFile(CommonNames.CodeDirectory + "Program.cs", new int[] { -2, -1, 0, 1 });

            Assert.IsNull(sut.TotalLines);

            var fileAnalysis = sut.AnalyzeFile();

            Assert.IsNotNull(fileAnalysis);
            Assert.IsNull(fileAnalysis.Error);
            Assert.AreEqual(fileAnalysis.Path, fileAnalysis.Path);
            Assert.AreEqual(87, sut.TotalLines);
            Assert.AreEqual(87, fileAnalysis.Lines.Count());

            Assert.AreEqual(1, fileAnalysis.Lines.ElementAt(0).LineNumber);
            Assert.AreEqual(-1, fileAnalysis.Lines.ElementAt(0).LineVisits);
            Assert.AreEqual(LineVisitStatus.NotCoverable, fileAnalysis.Lines.ElementAt(0).LineVisitStatus);

            Assert.AreEqual(2, fileAnalysis.Lines.ElementAt(1).LineNumber);
            Assert.AreEqual(0, fileAnalysis.Lines.ElementAt(1).LineVisits);
            Assert.AreEqual(LineVisitStatus.NotCovered, fileAnalysis.Lines.ElementAt(1).LineVisitStatus);

            Assert.AreEqual(3, fileAnalysis.Lines.ElementAt(2).LineNumber);
            Assert.AreEqual(1, fileAnalysis.Lines.ElementAt(2).LineVisits);
            Assert.AreEqual(LineVisitStatus.Covered, fileAnalysis.Lines.ElementAt(2).LineVisitStatus);
        }

        /// <summary>
        /// A test for AnalyzeFile
        /// </summary>
        [Test]
        public void AnalyzeFile_ExistingFileWithTrackedMethods_AnalysisIsReturned()
        {
            var sut = new CodeFile(CommonNames.CodeDirectory + "Program.cs", new int[] { -2, -1, 0, 1 });
            var testMethod = new TestMethod("TestFull", "Test");
            sut.AddCoverageByTestMethod(testMethod, new int[] { -2, 1, -1, 0 });

            var fileAnalysis = sut.AnalyzeFile();

            Assert.AreEqual(1, fileAnalysis.Lines.First().LineCoverageByTestMethod[testMethod].LineVisits);
            Assert.AreEqual(LineVisitStatus.Covered, fileAnalysis.Lines.First().LineCoverageByTestMethod[testMethod].LineVisitStatus);
        }

        /// <summary>
        /// A test for AnalyzeFile
        /// </summary>
        [Test]
        public void AnalyzeFile_NonExistingFile_AnalysisIsReturned()
        {
            var sut = new CodeFile(CommonNames.CodeDirectory + "Other.cs", new int[] { -2, -1, 0, 1 });

            Assert.IsNull(sut.TotalLines);

            var fileAnalysis = sut.AnalyzeFile();

            Assert.IsNotNull(fileAnalysis);
            Assert.IsNotNull(fileAnalysis.Error);
            Assert.AreEqual(fileAnalysis.Path, fileAnalysis.Path);
            Assert.IsNull(sut.TotalLines);
            Assert.AreEqual(0, fileAnalysis.Lines.Count());
        }

        /// <summary>
        /// A test for Equals
        /// </summary>
        [Test]
        public void Equals()
        {
            var target1 = new CodeFile(CommonNames.CodeDirectory + "Program.cs", new int[0]);
            var target2 = new CodeFile(CommonNames.CodeDirectory + "Program.cs", new int[0]);
            var target3 = new CodeFile(CommonNames.CodeDirectory + "Other.cs", new int[0]);

            Assert.IsTrue(target1.Equals(target2), "Objects are not equal");
            Assert.IsFalse(target1.Equals(target3), "Objects are equal");
            Assert.IsFalse(target1.Equals(null), "Objects are equal");
            Assert.IsFalse(target1.Equals(new object()), "Objects are equal");
        }
    }
}
