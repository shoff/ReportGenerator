﻿namespace Palmmedia.ReportGenerator.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Palmmedia.ReportGenerator.Parser.Analysis;

    /// <summary>
    /// XML report renderer.
    /// </summary>
    internal class XmlRenderer : RendererBase, IReportRenderer
    {
        /// <summary>
        /// Indicates that the current node representing an assembly has to be closed before adding further elements.
        /// </summary>
        private bool closeAssemblyNode;

        /// <summary>
        /// The report builder.
        /// </summary>
        private XmlWriter reportTextWriter;

        /// <summary>
        /// Begins the summary report.
        /// </summary>
        /// <param name="targetDirectory">The target directory.</param>
        /// <param name="title">The title.</param>
        public void BeginSummaryReport(string targetDirectory, string title)
        {
            string targetPath = Path.Combine(targetDirectory, "Summary.xml");
            this.CreateXmlWriter(targetPath);

            this.reportTextWriter.WriteStartElement("CoverageReport");
            this.reportTextWriter.WriteAttributeString("scope", title);
        }

        /// <summary>
        /// Adds custom summary elements to the report.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        public void CustomSummary(ICollection<Assembly> assemblies)
        {
        }

        /// <summary>
        /// Begins the class report.
        /// </summary>
        /// <param name="targetDirectory">The target directory.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        public void BeginClassReport(string targetDirectory, string assemblyName, string className)
        {
            string targetPath = Path.Combine(targetDirectory, ReplaceInvalidPathChars(assemblyName + "_" + className) + ".xml");
            this.CreateXmlWriter(targetPath);

            this.reportTextWriter.WriteStartElement("CoverageReport");
            this.reportTextWriter.WriteAttributeString("scope", className);
        }

        /// <summary>
        /// Adds a header to the report.
        /// </summary>
        /// <param name="text">The text.</param>
        public void Header(string text)
        {
            this.reportTextWriter.WriteStartElement(ReplaceNonLetterChars(text));
        }

        /// <summary>
        /// Adds the test methods to the report.
        /// </summary>
        /// <param name="testMethods">The test methods.</param>
        public void TestMethods(ICollection<TestMethod> testMethods)
        {
        }

        /// <summary>
        /// Adds a file of a class to a report.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        public void File(string path)
        {
            this.reportTextWriter.WriteStartElement("File");
            this.reportTextWriter.WriteAttributeString("name", path);
        }

        /// <summary>
        /// Adds a paragraph to the report.
        /// </summary>
        /// <param name="text">The text.</param>
        public void Paragraph(string text)
        {
        }

        /// <summary>
        /// Adds a table with two columns to the report.
        /// </summary>
        public void BeginKeyValueTable()
        {
        }

        /// <summary>
        /// Adds a summary table to the report.
        /// </summary>
        public void BeginSummaryTable()
        {
        }

        /// <summary>
        /// Adds a metrics table to the report.
        /// </summary>
        /// <param name="headers">The headers.</param>
        public void BeginMetricsTable(ICollection<string> headers)
        {
        }

        /// <summary>
        /// Adds a file analysis table to the report.
        /// </summary>
        /// <param name="headers">The headers.</param>
        public void BeginLineAnalysisTable(ICollection<string> headers)
        {
        }

        /// <summary>
        /// Adds a table row with two cells to the report.
        /// </summary>
        /// <param name="key">The text of the first column.</param>
        /// <param name="value">The text of the second column.</param>
        public void KeyValueRow(string key, string value)
        {
            this.reportTextWriter.WriteStartElement(ReplaceNonLetterChars(key));
            this.reportTextWriter.WriteValue(value);
            this.reportTextWriter.WriteEndElement();
        }

        /// <summary>
        /// Adds a table row with two cells to the report.
        /// </summary>
        /// <param name="key">The text of the first column.</param>
        /// <param name="files">The files.</param>
        public void KeyValueRow(string key, ICollection<string> files)
        {
            Contract.Requires<ArgumentNullException>(files != null);

            this.reportTextWriter.WriteStartElement(ReplaceNonLetterChars(key));

            foreach (var file in files)
            {
                this.reportTextWriter.WriteStartElement("File");
                this.reportTextWriter.WriteValue(file);
                this.reportTextWriter.WriteEndElement();
            }

            this.reportTextWriter.WriteEndElement();
        }

        /// <summary>
        /// Adds the given metric values to the report.
        /// </summary>
        /// <param name="metric">The metric.</param>
        public void MetricsRow(MethodMetric metric)
        {
            Contract.Requires<ArgumentNullException>(metric != null);

            this.reportTextWriter.WriteStartElement(ReplaceNonLetterChars(metric.ShortName));

            foreach (var m in metric.Metrics)
            {
                this.reportTextWriter.WriteStartElement(ReplaceNonLetterChars(m.Name));
                this.reportTextWriter.WriteValue(m.Value.ToString(CultureInfo.InvariantCulture));
                this.reportTextWriter.WriteEndElement();
            }

            this.reportTextWriter.WriteEndElement();
        }

        /// <summary>
        /// Adds the coverage information of a single line of a file to the report.
        /// </summary>
        /// <param name="analysis">The line analysis.</param>
        public void LineAnalysis(LineAnalysis analysis)
        {
            Contract.Requires<ArgumentNullException>(analysis != null);

            this.reportTextWriter.WriteStartElement("LineAnalysis");
            this.reportTextWriter.WriteAttributeString("line", analysis.LineNumber.ToString(CultureInfo.InvariantCulture));
            this.reportTextWriter.WriteAttributeString("visits", analysis.LineVisits.ToString(CultureInfo.InvariantCulture));
            this.reportTextWriter.WriteAttributeString("coverage", analysis.LineVisitStatus.ToString());
            this.reportTextWriter.WriteAttributeString("content", XmlRenderer.ReplaceInvalidXmlChars(analysis.LineContent));
            this.reportTextWriter.WriteEndElement();
        }

        /// <summary>
        /// Finishes the current table.
        /// </summary>
        public void FinishTable()
        {
            if (this.closeAssemblyNode)
            {
                this.reportTextWriter.WriteEndElement();
                this.closeAssemblyNode = false;
            }

            this.reportTextWriter.WriteEndElement();
        }

        /// <summary>
        /// Charts the specified historic coverages.
        /// </summary>
        /// <param name="historicCoverages">The historic coverages.</param>
        public void Chart(ICollection<HistoricCoverage> historicCoverages)
        {
        }

        /// <summary>
        /// Adds the coverage information of an assembly to the report.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public void SummaryAssembly(Assembly assembly)
        {
            Contract.Requires<ArgumentNullException>(assembly != null);
            if (this.closeAssemblyNode)
            {
                this.reportTextWriter.WriteEndElement();
            }

            this.reportTextWriter.WriteStartElement("Assembly");
            this.reportTextWriter.WriteAttributeString("name", assembly.Name);
            this.reportTextWriter.WriteAttributeString("classes", assembly.Classes.Count().ToString(CultureInfo.InvariantCulture));
            this.reportTextWriter.WriteAttributeString("coverage", assembly.CoverageQuota.HasValue ? assembly.CoverageQuota.Value.ToString(CultureInfo.InvariantCulture) : string.Empty);
            this.reportTextWriter.WriteAttributeString("coveredlines", assembly.CoveredLines.ToString(CultureInfo.InvariantCulture));
            this.reportTextWriter.WriteAttributeString("coverablelines", assembly.CoverableLines.ToString(CultureInfo.InvariantCulture));
            this.reportTextWriter.WriteAttributeString("totallines", assembly.TotalLines.HasValue ? assembly.TotalLines.Value.ToString(CultureInfo.InvariantCulture) : string.Empty);

            this.closeAssemblyNode = true;
        }

        /// <summary>
        /// Adds the coverage information of a class to the report.
        /// </summary>
        /// <param name="summaryClass">The class.</param>
        public void SummaryClass(Class summaryClass)
        {
            Contract.Requires<ArgumentNullException>(summaryClass != null);
            this.reportTextWriter.WriteStartElement("Class");
            this.reportTextWriter.WriteAttributeString("name", summaryClass.Name);
            this.reportTextWriter.WriteAttributeString("coverage", summaryClass.CoverageQuota.HasValue ? summaryClass.CoverageQuota.Value.ToString(CultureInfo.InvariantCulture) : string.Empty);
            this.reportTextWriter.WriteAttributeString("coveredlines", summaryClass.CoveredLines.ToString(CultureInfo.InvariantCulture));
            this.reportTextWriter.WriteAttributeString("coverablelines", summaryClass.CoverableLines.ToString(CultureInfo.InvariantCulture));
            this.reportTextWriter.WriteAttributeString("totallines", summaryClass.TotalLines.HasValue ? summaryClass.TotalLines.Value.ToString(CultureInfo.InvariantCulture) : string.Empty);
            this.reportTextWriter.WriteEndElement();
        }

        /// <summary>
        /// Saves a summary report.
        /// </summary>
        /// <param name="targetDirectory">The target directory.</param>
        public void SaveSummaryReport(string targetDirectory)
        {
            this.SaveReport();
        }

        /// <summary>
        /// Saves a class report.
        /// </summary>
        /// <param name="targetDirectory">The target directory.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        public void SaveClassReport(string targetDirectory, string assemblyName, string className)
        {
            this.SaveReport();
        }

        /// <summary>
        /// Initializes the xml writer.
        /// </summary>
        /// <param name="targetPath">The target path.</param>
        private void CreateXmlWriter(string targetPath)
        {
            var xmlWriterSettings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = false,
                CloseOutput = true
            };

            this.reportTextWriter = XmlWriter.Create(new FileStream(targetPath, FileMode.Create), xmlWriterSettings);
        }

        /// <summary>
        /// Saves the report.
        /// </summary>
        private void SaveReport()
        {
            this.reportTextWriter.Flush();
            this.reportTextWriter.Close();

            this.reportTextWriter = null;
        }
    }
}