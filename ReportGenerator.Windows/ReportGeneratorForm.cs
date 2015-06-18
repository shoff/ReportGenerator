
namespace ReportGenerator.Windows
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using System.Xml.Linq;
    using log4net;
    using log4net.Appender;
    using Palmmedia.ReportGenerator;
    using Palmmedia.ReportGenerator.Properties;
    using Palmmedia.ReportGenerator.Reporting;

    /// <summary>
    /// </summary>
    public partial class ReportGeneratorForm : Form
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ReportGeneratorForm));

        /// <summary>
        /// </summary>
        public ReportGeneratorForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void AddSourceDirectoryButtonClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.SourceDirectoryTextBox.Text))
            {
                return;
            }

            if (!Directory.Exists(this.SourceDirectoryTextBox.Text))
            {
                return;
            }
            
            if (this.SourceDirectoriesListBox.Items.Count == 0)
            {
                this.SourceDirectoriesListBox.Items.Add(this.SourceDirectoryTextBox.Text + "\r\n");
            }
            else
            {
                this.SourceDirectoriesListBox.Items.Add(";" + this.SourceDirectoryTextBox.Text + "\r\n");
            }

            PopulateListView();

            // TODO
            var reportConfigurationBuilder = new ReportConfigurationBuilder(new MefReportBuilderFactory());

            //if (args.Length < 2)
            //{
            //    reportConfigurationBuilder.ShowHelp();
            //    return 1;
            //}

            //args = args.Select(a => a.EndsWith("\"", StringComparison.OrdinalIgnoreCase) ? a.TrimEnd('\"') + "\\" : a).ToArray();

            //ReportConfiguration configuration = reportConfigurationBuilder.Create(args);

            //return Execute(configuration) ? 0 : 1;

        }

        /// <summary>
        /// </summary>
        private void PopulateListView()
        {

            DirectoryInfo dinfo = new DirectoryInfo(this.SourceDirectoryTextBox.Text);
            FileInfo[] files = dinfo.GetFiles("*.coveragexml");

            foreach (FileInfo file in files)
            {
                this.AvailableFilesView.Items.Add(file.Name);
            }

        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void ReportGeneratorFormLoad(object sender, EventArgs e)
        {
            this.ReportTypesListBox.SelectedItems.Add(this.ReportTypesListBox.Items[0]);
            this.FileTypeListBox.SelectedItems.Add(this.FileTypeListBox.Items[0]);
            this.FileTypeListBox.SelectedItems.Add(this.FileTypeListBox.Items[1]);

            this.UnIgnoreButton.Enabled = this.FilterListView.Items.Count > 0;
            this.IgnoreButton.Enabled = this.AvailableFilesView.Items.Count > 0 && this.AvailableFilesView.SelectedItems.Count > 0;
            LoadConfigFile();
        }

        private void SetupReportConfigurationBuilder()
        {
            ReportConfig config = new ReportConfig();
            config.Filters = new string[this.FilterListView.Items.Count];
            
            for (int i = 0; i < this.FilterListView.Items.Count; i++)
            {
                config.Filters.Add(this.FilterListView.Items[i].ToString());
            }

            foreach (var reportPattern in this.FileTypeListBox.Items)
            {
                config.ReportFilePatterns.Add(reportPattern.ToString());
            }

            foreach (var reportPattern in this.FileTypeListBox.Items)
            {
                config.ReportFilePatterns.Add(reportPattern.ToString());
            }

            if (!string.IsNullOrWhiteSpace(this.HistoryDirectoryTextBox.Text))
            {
                config.HistoryDirectory = this.HistoryDirectoryTextBox.Text;
            }
            
            if (!string.IsNullOrWhiteSpace(this.TargetDirectoryTextBox.Text))
            {
                config.TargetDirectory = this.TargetDirectoryTextBox.Text;
            }

            config.VerbosityLevel = this.LogLevelComboBox.SelectedItem.ToString();

            //ReportConfiguration configuration = reportConfigurationBuilder.Create();
        }

        private void LoadConfigFile()
        {
            XDocument xdoc = XDocument.Load("configuration.xml");
            //Run query
            var sourceDirectories = xdoc.Descendants("sourcedir").Elements("dir").Map(x => x.Value).ToList();
            foreach (var directory in sourceDirectories)
            {
                this.SourceDirectoryTextBox.Text = directory;
                this.AddSourceDirectoryButtonClick(this, EventArgs.Empty);
            }
            this.SourceDirectoryTextBox.Text = "";
            try
            {
                this.TargetDirectoryTextBox.Text = xdoc.Descendants("targetdir").FirstOrDefault().Value;
                this.HistoryDirectoryTextBox.Text = xdoc.Descendants("historydir").FirstOrDefault().Value;
            }
            catch (NullReferenceException)
            {
                // ignore it.
            }

        }

        /// <summary>
        /// Executes the report generation.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <returns>
        /// <c>true</c> if report was generated successfully; otherwise <c>false</c>.
        /// </returns>
        internal static bool Execute(ReportConfiguration configuration)
        {
            Contract.Requires<ArgumentNullException>(configuration != null);
            
            var appender = new RollingFileAppender()
            {
                Layout = new log4net.Layout.PatternLayout("%message%newline")
            };

            appender.ActivateOptions();
            log4net.Config.BasicConfigurator.Configure(appender);
            if (configuration.VerbosityLevel == VerbosityLevel.Info)
            {
                 appender.Threshold = log4net.Core.Level.Info;
            }
            else if (configuration.VerbosityLevel == VerbosityLevel.Error)
            {
                 appender.Threshold = log4net.Core.Level.Error;
            }
            if (!configuration.Validate())
            {
                return false;
            }
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            DateTime executionTime = DateTime.Now;

            var parser = Palmmedia.ReportGenerator.Parser.ParserFactory.CreateParser(configuration.ReportFiles, configuration.SourceDirectories);

            if (configuration.HistoryDirectory != null)
            {
                new HistoryParser(parser.Assemblies, configuration.HistoryDirectory).ApplyHistoricCoverage();
            }

            new ReportGenerator(parser, new DefaultAssemblyFilter(configuration.Filters), 
                configuration.ReportBuilderFactory.GetReportBuilders(configuration.TargetDirectory, configuration.ReportTypes))
                    .CreateReport(configuration.HistoryDirectory != null, executionTime);

            if (configuration.HistoryDirectory != null)
            {
                new HistoryReportGenerator( parser, configuration.HistoryDirectory).CreateReport(executionTime);
            }

            stopWatch.Stop();
            logger.InfoFormat(Resources.ReportGenerationTook, stopWatch.ElapsedMilliseconds / 1000d);

            return true;
        }

        private void RunReportButtonClick(object sender, EventArgs e)
        {
            SetupReportConfigurationBuilder();
        }

        private void IgnoreButtonClick(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.AvailableFilesView.SelectedItems)
            {
                this.FilterListView.Items.Add(item.Text);
                this.AvailableFilesView.Items.Remove(item);
            }
            this.RunReportButton.Enabled = this.AvailableFilesView.Items.Count > 0;
            this.NoFilesTextBox.Visible = this.AvailableFilesView.Items.Count == 0;
            this.IgnoreButton.Enabled = false;
        }

        private void UnIgnoreButtonClick(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.FilterListView.SelectedItems)
            {
                this.AvailableFilesView.Items.Add(item.Text);
                this.FilterListView.Items.Remove(item);
            }
            this.UnIgnoreButton.Enabled = false;
            this.RunReportButton.Enabled = this.AvailableFilesView.Items.Count > 0;
            this.NoFilesTextBox.Visible = this.AvailableFilesView.Items.Count == 0;
        }

        private void BrowseButtonClick(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
        }

        private void AvailableFilesViewItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (this.AvailableFilesView.SelectedItems.Count > 0)
            {
                this.IgnoreButton.Enabled = true;
            }
        }

        private void FilterListViewItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (this.FilterListView.SelectedItems.Count > 0)
            {
                this.UnIgnoreButton.Enabled = true;
            }
        }

        private void RunReportButtonEnabledChanged(object sender, EventArgs e)
        {

        }

    }
}
