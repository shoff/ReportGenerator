namespace ReportGenerator.Windows
{
    using System.Collections.Generic;

    public class ReportConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportConfig"/> class.
        /// </summary>
        public ReportConfig()
        {
            this.ReportTypes = new List<string>();
            this.SourceDirectories = new List<string>();
            this.Filters = new List<string>();
            this.ReportFilePatterns = new List<string>();
        }

        /// <summary>
        /// Gets or sets the reports.
        /// </summary>
        /// <value>
        /// The reports.
        /// </value>
        public string Reports { get; set; }
        /// <summary>
        /// Gets or sets the target directory.
        /// </summary>
        /// <value>
        /// The target directory.
        /// </value>
        public string TargetDirectory { get; set;}
        /// <summary>
        /// Gets or sets the history directory.
        /// </summary>
        /// <value>
        /// The history directory.
        /// </value>
        public string HistoryDirectory { get; set; }
        /// <summary>
        /// Gets or sets the report types.
        /// </summary>
        /// <value>
        /// The report types.
        /// </value>
        public ICollection<string> ReportTypes { get; set; }
        /// <summary>
        /// Gets or sets the source directories.
        /// </summary>
        /// <value>
        /// The source directories.
        /// </value>
        public ICollection<string> SourceDirectories { get; set; }
        /// <summary>
        /// Gets or sets the filters.
        /// </summary>
        /// <value>
        /// The filters.
        /// </value>
        public ICollection<string> Filters { get; set; }
        /// <summary>
        /// Gets or sets the report file patterns.
        /// </summary>
        /// <value>
        /// The report file patterns.
        /// </value>
        public ICollection<string> ReportFilePatterns { get; set; }
        /// <summary>
        /// Gets or sets the verbosity level.
        /// </summary>
        /// <value>
        /// The verbosity level.
        /// </value>
        public string VerbosityLevel { get; set; }

    }
}