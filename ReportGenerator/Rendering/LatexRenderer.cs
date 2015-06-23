namespace Palmmedia.ReportGenerator.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Text;
    using Palmmedia.ReportGenerator.Parser.Analysis;
    using Palmmedia.ReportGenerator.Properties;
    using Palmmedia.ReportGenerator.Reporting;

    /// <summary>
    ///   Latex report renderer.
    /// </summary>
    internal class LatexRenderer : RendererBase, IReportRenderer, IDisposable
    {
        private TextWriter reportTextWriter;
        private Stream summaryReportStream;
        private Stream classReportStream;
        private TextWriter summaryReportTextWriter;
        private TextWriter classReportTextWriter;

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Begins the summary report.
        /// </summary>
        /// <param name="targetDirectory">The target directory.</param>
        /// <param name="title">The title.</param>
        public void BeginSummaryReport(string targetDirectory, string title)
        {
            this.summaryReportStream = new MemoryStream();
            this.reportTextWriter = this.summaryReportTextWriter = new StreamWriter(this.summaryReportStream);

            var start = string.Format(
                CultureInfo.InvariantCulture, 
                LatexStart, 
                ReportResources.CoverageReport, 
                typeof(IReportBuilder).Assembly.GetName().Name, 
                typeof(IReportBuilder).Assembly.GetName().Version);

            this.reportTextWriter.WriteLine(start);
        }

        /// <summary>Begins the class report.</summary>
        /// <param name="targetDirectory">The target directory.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="FileNotFoundException">The file cannot be found, such as when <paramref name="mode" />
        /// is FileMode.Truncate or FileMode.Open, and the file specified by <paramref name="path" /> does not exist.
        /// The file must already exist in these modes.</exception>
        /// <exception cref="IOException">An I/O error, such as specifying FileMode.CreateNew when the file specified by
        /// <paramref name="path" /> already exists, occurred.-or-The stream has been closed.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        public void BeginClassReport(string targetDirectory, string assemblyName, string className)
        {
            if (this.classReportTextWriter == null)
            {
                var targetPath = Path.Combine(targetDirectory, "tmp.tex");

                this.classReportStream = new FileStream(targetPath, FileMode.Create);
                this.classReportTextWriter = new StreamWriter(this.classReportStream);
            }

            this.reportTextWriter = this.classReportTextWriter;
            this.reportTextWriter.WriteLine(@"\newpage");

            className = string.Format(CultureInfo.InvariantCulture, @"\section{{{0}}}", EscapeLatexChars(className));
            this.reportTextWriter.WriteLine(className);
        }

        /// <summary>
        ///   Adds a header to the report.
        /// </summary>
        /// <param name="text">The text.</param>
        public void Header(string text)
        {
            text = string.Format(
                CultureInfo.InvariantCulture, 
                @"\{0}section{{{1}}}", 
                this.reportTextWriter == this.classReportTextWriter ? "sub" : string.Empty, 
                EscapeLatexChars(text));
            this.reportTextWriter.WriteLine(text);
        }

        /// <summary>
        ///   Adds the test methods to the report.
        /// </summary>
        /// <param name="testMethods">The test methods.</param>
        public void TestMethods(ICollection<TestMethod> testMethods)
        {
        }

        /// <summary>
        ///   Adds a file of a class to a report.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        public void File(string path)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path));

            if (path.Length > 78)
            {
                path = path.Substring(path.Length - 78);
            }

            path = string.Format(CultureInfo.InvariantCulture, @"\subsubsection{{{0}}}", EscapeLatexChars(path));
            this.reportTextWriter.WriteLine(path);
        }

        /// <summary>
        ///   Adds a paragraph to the report.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        public void Paragraph(string text)
        {
            this.reportTextWriter.WriteLine(EscapeLatexChars(text));
        }

        /// <summary>
        ///   Adds a table with two columns to the report.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed. </exception>
        public void BeginKeyValueTable()
        {
            this.reportTextWriter.WriteLine(@"\begin{longtable}[l]{ll}");
        }

        /// <summary>
        ///   Adds a summary table to the report.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed. </exception>
        public void BeginSummaryTable()
        {
            this.reportTextWriter.WriteLine(@"\begin{longtable}[l]{ll}");
        }

        /// <summary>
        ///   Adds custom summary elements to the report.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        public void CustomSummary(ICollection<Assembly> assemblies)
        {
        }

        /// <summary>
        ///   Adds a metrics table to the report.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <exception cref="ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed. </exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        public void BeginMetricsTable(ICollection<string> headers)
        {
            var columns = "|" + string.Join("|", headers.Select(h => "l")) + "|";

            this.reportTextWriter.WriteLine(@"\begin{longtable}[l]{" + columns + "}");
            this.reportTextWriter.WriteLine(@"\hline");
            this.reportTextWriter.Write(string.Join(" & ", headers.Select(h => @"\textbf{" + EscapeLatexChars(h) + "}")));
            this.reportTextWriter.WriteLine(@"\\");
            this.reportTextWriter.WriteLine(@"\hline");
        }

        /// <summary>
        ///   Adds a file analysis table to the report.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <exception cref="ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed. </exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        public void BeginLineAnalysisTable(ICollection<string> headers)
        {
            this.reportTextWriter.WriteLine(@"\begin{longtable}[l]{lrrll}");
            this.reportTextWriter.Write(string.Join(" & ", headers.Select(h => @"\textbf{" + EscapeLatexChars(h) + "}")));
            this.reportTextWriter.WriteLine(@"\\");
        }

        /// <summary>
        ///   Adds a table row with two cells to the report.
        /// </summary>
        /// <param name="key">The text of the first column.</param>
        /// <param name="value">The text of the second column.</param>
        /// <exception cref="ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed. </exception>
        public void KeyValueRow(string key, string value)
        {
            var row = string.Format(
                CultureInfo.InvariantCulture, 
                @"\textbf{{{0}}} & {1}\\", 
                ShortenString(key), 
                EscapeLatexChars(value));

            this.reportTextWriter.WriteLine(row);
        }

        /// <summary>
        ///   Adds a table row with two cells to the report.
        /// </summary>
        /// <param name="key">The text of the first column.</param>
        /// <param name="files">The files.</param>
        /// <exception cref="ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed. </exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        public void KeyValueRow(string key, ICollection<string> files)
        {
            files = (ICollection<string>)files.Select(f => f.Length > 78 ? f.Substring(f.Length - 78) : f);

            var row = string.Format(
                CultureInfo.InvariantCulture, 
                @"\textbf{{{0}}} & \begin{{minipage}}[t]{{12cm}}{{{1}}}\end{{minipage}} \\", 
                ShortenString(key), 
                string.Join(@"\\", files.Select(f => EscapeLatexChars(f))));

            this.reportTextWriter.WriteLine(row);
        }

        /// <summary>
        ///   Adds the given metric values to the report.
        /// </summary>
        /// <param name="metric">The metric.</param>
        public void MetricsRow(MethodMetric metric)
        {
            Contract.Requires<ArgumentNullException>(metric != null);

            var metrics = string.Join(" & ", metric.Metrics.Select(m => m.Value.ToString(CultureInfo.InvariantCulture)));

            var row = string.Format(
                CultureInfo.InvariantCulture, 
                @"\textbf{{{0}}} & {1}\\", 
                EscapeLatexChars(ShortenString(metric.ShortName, 20)), 
                metrics);

            this.reportTextWriter.WriteLine(row);
            this.reportTextWriter.WriteLine(@"\hline");
        }

        /// <summary>
        ///   Adds the coverage information of a single line of a file to the report.
        /// </summary>
        /// <param name="analysis">The line analysis.</param>
        public void LineAnalysis(LineAnalysis analysis)
        {
            Contract.Requires<ArgumentNullException>(analysis != null);

            var formattedLine = analysis.LineContent.Replace(((char)11).ToString(), "  ") // replace tab
                .Replace(((char)9).ToString(), "  ") // replace tab
                .Replace("~", " "); // replace '~' since this used for the \verb command

            formattedLine = ShortenString(formattedLine);
            formattedLine = ReplaceInvalidXmlChars(formattedLine);

            var lineVisitStatus = "gray";

            if (analysis.LineVisitStatus == LineVisitStatus.Covered)
            {
                lineVisitStatus = "green";
            }
            else if (analysis.LineVisitStatus == LineVisitStatus.NotCovered)
            {
                lineVisitStatus = "red";
            }

            var row = string.Format(
                CultureInfo.InvariantCulture, 
                @"\cellcolor{{{0}}} & {1} & \verb~{2}~ & & \verb~{3}~\\", 
                lineVisitStatus, 
                analysis.LineVisitStatus != LineVisitStatus.NotCoverable
                    ? analysis.LineVisits.ToString(CultureInfo.InvariantCulture)
                    : string.Empty, 
                analysis.LineNumber, 
                formattedLine);

            this.reportTextWriter.WriteLine(row);
        }

        /// <summary>
        ///   Finishes the current table.
        /// </summary>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        public void FinishTable()
        {
            this.reportTextWriter.WriteLine(@"\end{longtable}");
        }

        /// <summary>
        ///   Charts the specified historic coverages.
        /// </summary>
        /// <param name="historicCoverages">The historic coverages.</param>
        public void Chart(ICollection<HistoricCoverage> historicCoverages)
        {
        }

        /// <summary>
        ///   Adds the coverage information of an assembly to the report.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public void SummaryAssembly(Assembly assembly)
        {
            Contract.Requires<ArgumentNullException>(assembly != null);

            var row = string.Format(
                CultureInfo.InvariantCulture, 
                @"\textbf{{{0}}} & \textbf{{{1}}}\\", 
                EscapeLatexChars(assembly.Name), 
                assembly.CoverageQuota.HasValue
                    ? assembly.CoverageQuota.Value.ToString(CultureInfo.InvariantCulture) + @"\%"
                    : string.Empty);

            this.reportTextWriter.WriteLine(row);
        }

        /// <summary>
        ///   Adds the coverage information of a class to the report.
        /// </summary>
        /// <param name="summaryClass">The class.</param>
        public void SummaryClass(Class summaryClass)
        {
            Contract.Requires<ArgumentNullException>(summaryClass != null);

            var row = string.Format(
                CultureInfo.InvariantCulture, 
                @"{0} & {1}\\", 
                EscapeLatexChars(summaryClass.Name), 
                summaryClass.CoverageQuota.HasValue
                    ? summaryClass.CoverageQuota.Value.ToString(CultureInfo.InvariantCulture) + @"\%"
                    : string.Empty);

            this.reportTextWriter.WriteLine(row);
        }

        /// <summary>
        ///   Saves a summary report.
        /// </summary>
        /// <param name="targetDirectory">The target directory.</param>
        public void SaveSummaryReport(string targetDirectory)
        {
            var targetPath = Path.Combine(targetDirectory, "Summary.tex");

            Stream combinedReportStream = new FileStream(targetPath, FileMode.Create);

            this.summaryReportTextWriter.Flush();
            this.summaryReportStream.Position = 0;
            this.summaryReportStream.CopyTo(combinedReportStream);

            this.summaryReportTextWriter.Dispose();

            if (this.classReportTextWriter != null)
            {
                this.classReportTextWriter.Flush();
                this.classReportStream.Position = 0;
                this.classReportStream.CopyTo(combinedReportStream);

                this.classReportTextWriter.Dispose();

                System.IO.File.Delete(Path.Combine(targetDirectory, "tmp.tex"));
            }

            var latexEnd = Encoding.UTF8.GetBytes(LatexEnd);

            combinedReportStream.Write(latexEnd, 0, latexEnd.Length);

            combinedReportStream.Flush();
            combinedReportStream.Dispose();

            this.classReportTextWriter = null;
            this.summaryReportTextWriter = null;
        }

        /// <summary>
        ///   Saves a class report.
        /// </summary>
        /// <param name="targetDirectory">The target directory.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        public void SaveClassReport(string targetDirectory, string assemblyName, string className)
        {
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///   unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.summaryReportTextWriter != null)
                {
                    this.summaryReportTextWriter.Dispose();
                }

                if (this.classReportTextWriter != null)
                {
                    this.classReportTextWriter.Dispose();
                }

                if (this.summaryReportStream != null)
                {
                    this.summaryReportStream.Dispose();
                }

                if (this.classReportStream != null)
                {
                    this.classReportStream.Dispose();
                }
            }
        }

        /// <summary>
        ///   Shortens the given string.
        /// </summary>
        /// <param name="text">The text to shorten.</param>
        /// <returns>The shortened string.</returns>
        private static string ShortenString(string text)
        {
            return ShortenString(text, 78);
        }

        /// <summary>
        ///   Shortens the given string.
        /// </summary>
        /// <param name="text">The text to shorten.</param>
        /// <param name="maxLength">Maximum length.</param>
        /// <returns>The shortened string.</returns>
        private static string ShortenString(string text, int maxLength)
        {
            if (text.Length > maxLength)
            {
                return text.Substring(0, maxLength);
            }

            return text;
        }

        /// <summary>
        ///   Escapes the latex chars.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The text with escaped latex chars.</returns>
        private static string EscapeLatexChars(string text)
        {
            return text.Replace(@"\", @"\textbackslash ").Replace("%", @"\%").Replace("#", @"\#").Replace("_", @"\_");
        }

        #region Latex Snippets

        /// <summary>
        ///   The head of each generated Latex file.
        /// </summary>
        private const string LatexStart = @"\documentclass[a4paper,10pt]{{article}}
\usepackage[paper=a4paper,left=20mm,right=20mm,top=20mm,bottom=20mm]{{geometry}}
\usepackage{{longtable}}
\usepackage{{fancyhdr}}
\usepackage[pdftex]{{color}}
\usepackage{{colortbl}}
\definecolor{{green}}{{rgb}}{{0,1,0.12}}
\definecolor{{red}}{{rgb}}{{1,0,0}}
\definecolor{{gray}}{{rgb}}{{0.86,0.86,0.86}}

\usepackage[pdftex,
            colorlinks=true, linkcolor=red, urlcolor=green, citecolor=red,%
            raiselinks=true,%
            bookmarks=true,%
            bookmarksopenlevel=1,%
            bookmarksopen=true,%
            bookmarksnumbered=true,%
            hyperindex=true,% 
            plainpages=false,% correct hyperlinks
            pdfpagelabels=true%,% view TeX pagenumber in PDF reader
            %pdfborder={{0 0 0.5}}
            ]{{hyperref}}

\hypersetup{{pdftitle={{{0}}},
            pdfauthor={{{1} - {2}}}
           }}

\pagestyle{{fancy}}
\fancyhead[LE,LO]{{\leftmark}}
\fancyhead[R]{{\thepage}}
\fancyfoot[C]{{{1} - {2}}}

\begin{{document}}

\setcounter{{secnumdepth}}{{-1}}";

        private const string LatexEnd = @"\end{document}";

        #endregion
    }
}