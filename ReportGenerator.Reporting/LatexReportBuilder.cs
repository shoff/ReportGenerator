﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Palmmedia.ReportGenerator.Parser.Analysis;
using Palmmedia.ReportGenerator.Reporting.Rendering;

namespace Palmmedia.ReportGenerator.Reporting
{
    /// <summary>
    /// Creates report in Latex format.
    /// </summary>
    [Export(typeof(IReportBuilder))]
    public class LatexReportBuilder : ReportBuilderBase, IDisposable
    {
        /// <summary>
        /// The shared renderer.
        /// </summary>
        private LatexRenderer renderer = new LatexRenderer();

        /// <summary>
        /// Gets the type of the report.
        /// </summary>
        /// <value>
        /// The type of the report.
        /// </value>
        public override string ReportType
        {
            get { return "Latex"; }
        }

        /// <summary>
        /// Creates a class report.
        /// </summary>
        /// <param name="reportClass">The class.</param>
        /// <param name="fileAnalyses">The file analyses that correspond to the class.</param>
        public override void CreateClassReport(Class reportClass, IEnumerable<FileAnalysis> fileAnalyses)
        {
            this.CreateClassReport(this.renderer, reportClass, fileAnalyses);
        }

        /// <summary>
        /// Creates the summary report.
        /// </summary>
        /// <param name="summaryResult">The summary result.</param>
        public override void CreateSummaryReport(SummaryResult summaryResult)
        {
            this.CreateSummaryReport(this.renderer, summaryResult);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged ReportResources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged ReportResources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.renderer != null)
                {
                    this.renderer.Dispose();
                }
            }
        }
    }
}
