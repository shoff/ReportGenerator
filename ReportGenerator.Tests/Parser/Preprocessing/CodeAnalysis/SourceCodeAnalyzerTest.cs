﻿

namespace ReportGenerator.Tests.Parser.Preprocessing.CodeAnalysis
{
    using System.IO;
    using NUnit.Framework;
    using Palmmedia.ReportGenerator.Parser.Preprocessing.CodeAnalysis;

    [TestFixture]
    public class SourceCodeAnalyzerTest
    {
        private static readonly string elementClassFile = CommonNames.CodeDirectory + "AnalyzerTestClass.cs";

        [Test]
        public void GetClassesInFile_All_Classes_Are_Returned()
        {
            var tfc = CommonNames.CodeDirectory + "TestClass.cs";
            Assert.True(File.Exists(tfc));

            var classes = SourceCodeAnalyzer.GetClassesInFile(tfc);
            Assert.IsNotNull(classes, "Classes must not be null.");

            Assert.IsTrue(classes.Contains("ReportGenerator.Tests.TestFiles.Project.TestClass"), "Classes does not contain root class");
            Assert.IsTrue(classes.Contains("ReportGenerator.Tests.TestFiles.Project.TestClassNestedClass"), "Classes does not contain nested class");
        }

        [Test]
        public void FindMethod_SearchExistingMethod_PositionMustNotBeNullAndSupplyCorrectLinenumber()
        {
            PartCoverMethodElement partCoverMethodElement = new PartCoverMethodElement(
                "ReportGenerator.Tests.TestFiles.Project.AnalyzerTestClass", 
                "DoSomething", 
                "string  (string, string[], System.Guid, string, string, System.Decimal, int, long, stringint, ref int, float, double, bool, unsigned byte, char, object, byte, short, unsigned int, unsigned long, unsigned short, ICSharpCode.NRefactory.Ast.INode)");

            var methodPosition = SourceCodeAnalyzer.FindSourceElement(elementClassFile, partCoverMethodElement);

            Assert.IsNotNull(methodPosition, "MethodPosition must not be null.");

            Assert.AreEqual(37, methodPosition.Start, "Start line number does not match.");
            Assert.AreEqual(40, methodPosition.End, "End line number does not match.");
        }

        [Test]
        public void FindMethod_SearchExistingConstructor_PositionMustNotBeNullAndSupplyCorrectLinenumber()
        {
            PartCoverMethodElement partCoverMethodElement = new PartCoverMethodElement("ReportGenerator.Tests.TestFiles.Project.AnalyzerTestClass", ".ctor", "void  ()");

            var methodPosition = SourceCodeAnalyzer.FindSourceElement(elementClassFile, partCoverMethodElement);

            Assert.IsNotNull(methodPosition, "MethodPosition must not be null.");

            Assert.AreEqual(10, methodPosition.Start, "Start line number does not match.");
            Assert.AreEqual(12, methodPosition.End, "End line number does not match.");
        }

        [Test]
        public void FindMethod_SearchNonExistingGenericMethod_PositionIsNull()
        {
            PartCoverMethodElement partCoverMethodElement = new PartCoverMethodElement(
                "TestNamespace.AnalyzerTestClass", 
                "GenericMethod", 
                "void  (int)");

            var methodPosition = SourceCodeAnalyzer.FindSourceElement(elementClassFile, partCoverMethodElement);

            Assert.IsNull(methodPosition, "MethodPosition is not null.");
        }

        [Test]
        public void FindProperty_SearchExistingProperty_PositionMustNotBeNullAndSupplyCorrectLinenumber()
        {
            PropertyElement propertyElement = new PropertyElement("ReportGenerator.Tests.TestFiles.Project.AnalyzerTestClass", "get_AutoProperty");

            var propertyPosition = SourceCodeAnalyzer.FindSourceElement(elementClassFile, propertyElement);

            Assert.IsNotNull(propertyPosition, "PropertyPosition must not be null.");

            Assert.AreEqual(46, propertyPosition.Start, "Start line number does not match.");
            Assert.AreEqual(46, propertyPosition.End, "End line number does not match.");
        }

        [Test]
        public void FindProperty_SearchNonExistingProperty_PositionIsNull()
        {
            PropertyElement propertyElement = new PropertyElement(CommonNames.TestNamespace + "AnalyzerTestClass", "get_DoesNotExist");

            var propertyPosition = SourceCodeAnalyzer.FindSourceElement(elementClassFile, propertyElement);

            Assert.IsNull(propertyPosition, "PropertyPosition is not null.");
        }
    }
}
