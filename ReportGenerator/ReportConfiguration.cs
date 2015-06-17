using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using Palmmedia.ReportGenerator.Common;
using Palmmedia.ReportGenerator.Properties;
using Palmmedia.ReportGenerator.Reporting;

namespace Palmmedia.ReportGenerator
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides all parameters that are required for report generation.
    /// </summary>
    public class ReportConfiguration
    {
        /// <summary>
        /// The Logger.
        /// </summary>
        private static readonly ILog logger = LogManager.GetLogger(typeof(ReportConfiguration));

        /// <summary>
        /// The report files.
        /// </summary>
        private readonly List<string> reportFiles = new List<string>();

        /// <summary>
        /// The report file pattern that could not be parsed.
        /// </summary>
        private readonly List<string> failedReportFilePatterns = new List<string>();

        /// <summary>
        /// Determines whether the verbosity level was successfully parsed during initialization.
        /// </summary>
        private readonly bool verbosityLevelValid = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportConfiguration" /> class.
        /// </summary>
        /// <param name="reportBuilderFactory">The report builder factory.</param>
        /// <param name="reportFilePatterns">The report file patterns.</param>
        /// <param name="targetDirectory">The target directory.</param>
        /// <param name="historyDirectory">The history directory.</param>
        /// <param name="reportTypes">The report types.</param>
        /// <param name="sourceDirectories">The source directories.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="verbosityLevel">The verbosity level.</param>
        public ReportConfiguration(
            IReportBuilderFactory reportBuilderFactory,
            ICollection<string> reportFilePatterns,
            string targetDirectory,
            string historyDirectory,
            ICollection<string> reportTypes,
            ICollection<string> sourceDirectories,
            ICollection<string> filters,
            string verbosityLevel)
        {
            Contract.Requires<ArgumentNullException>(reportBuilderFactory != null);
            Contract.Requires<ArgumentNullException>(reportFilePatterns != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(targetDirectory));
            Contract.Requires<ArgumentNullException>(reportTypes != null);
            Contract.Requires<ArgumentNullException>(sourceDirectories != null);
            Contract.Requires<ArgumentNullException>(filters != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(verbosityLevel));

            this.ReportBuilderFactory = reportBuilderFactory;

            foreach (var reportFilePattern in reportFilePatterns)
            {
                try
                {
                    this.reportFiles.AddRange(FileSearch.GetFiles(reportFilePattern));
                }
                catch (Exception)
                {
                    this.failedReportFilePatterns.Add(reportFilePattern);
                }
            }

            this.TargetDirectory = targetDirectory;
            this.HistoryDirectory = historyDirectory;

            var reportsCollection = reportTypes as string[] ?? reportTypes.ToArray();
            if (reportsCollection.Any())
            {
                this.ReportTypes = reportsCollection;
            }
            else
            {
                this.ReportTypes = new[] { "Html" };
            }

            this.SourceDirectories = sourceDirectories;
            this.Filters = filters;
            VerbosityLevel parsedVerbosityLevel = VerbosityLevel.Verbose;
            this.verbosityLevelValid = Enum.TryParse<VerbosityLevel>(verbosityLevel, true, out parsedVerbosityLevel);
            this.VerbosityLevel = parsedVerbosityLevel;
        }

        /// <summary>
        /// Gets the report builder factory.
        /// </summary>
        public IReportBuilderFactory ReportBuilderFactory
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the report files.
        /// </summary>
        public ICollection<string> ReportFiles
        {
            get
            {
                return this.reportFiles;
            }
        }

        /// <summary>
        /// Gets the target directory.
        /// </summary>
        public string TargetDirectory
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the history directory.
        /// </summary>
        public string HistoryDirectory
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of the report.
        /// </summary>
        public ICollection<string> ReportTypes
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the source directories.
        /// </summary>
        public ICollection<string> SourceDirectories
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        public ICollection<string> Filters
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the verbosity level.
        /// </summary>
        public VerbosityLevel VerbosityLevel
        {
            get;
            private set;
        }

        /// <summary>
        /// Validates all parameters.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if all parameters are in a valid state; otherwise <c>false</c>.
        /// </returns>
        public bool Validate()
        {
            bool result = true;

            if (this.failedReportFilePatterns.Count > 0)
            {
                foreach (var failedReportFilePattern in this.failedReportFilePatterns)
                {
                    logger.ErrorFormat(Resources.FailedReportFilePattern, failedReportFilePattern);
                }

                result = false;
            }

            if (!this.ReportFiles.Any())
            {
                logger.Error(Resources.NoReportFiles);
                result = false;
            }
            else
            {
                foreach (var file in this.ReportFiles)
                {
                    if (!File.Exists(file))
                    {
                        logger.ErrorFormat(Resources.NotExistingReportFile, file);
                        result = false;
                    }
                }
            }

            if (string.IsNullOrEmpty(this.TargetDirectory))
            {
                logger.Error(Resources.NoTargetDirectory);
                result = false;
            }
            else if (!Directory.Exists(this.TargetDirectory))
            {
                try
                {
                    Directory.CreateDirectory(this.TargetDirectory);
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat(Resources.TargetDirectoryCouldNotBeCreated, this.TargetDirectory, ex.Message);
                    result = false;
                }
            }

            if (!string.IsNullOrEmpty(this.HistoryDirectory) && !Directory.Exists(this.HistoryDirectory))
            {
                try
                {
                    Directory.CreateDirectory(this.HistoryDirectory);
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat(Resources.HistoryDirectoryCouldNotBeCreated, this.HistoryDirectory, ex.Message);
                    result = false;
                }
            }

            var availableReportTypes = this.ReportBuilderFactory.GetAvailableReportTypes();

            foreach (var reportType in this.ReportTypes)
            {
                if (!availableReportTypes.Contains(reportType, StringComparer.OrdinalIgnoreCase))
                {
                    logger.ErrorFormat(Resources.UnknownReportType, reportType);
                    result = false;
                }
            }

            foreach (var directory in this.SourceDirectories)
            {
                if (!Directory.Exists(directory))
                {
                    logger.ErrorFormat(Resources.SourceDirectoryDoesNotExist, directory);
                    result = false;
                }
            }

            foreach (var filter in this.Filters)
            {
                if (string.IsNullOrEmpty(filter)
                    || (!filter.StartsWith("+", StringComparison.OrdinalIgnoreCase)
                        && !filter.StartsWith("-", StringComparison.OrdinalIgnoreCase)))
                {
                    logger.ErrorFormat(Resources.InvalidFilter, filter);
                    result = false;
                }
            }

            if (!this.verbosityLevelValid)
            {
                logger.Error(Resources.UnknownVerbosityLevel);
                result = false;
            }

            return result;
        }
    }
}
