namespace ReportGenerator.Windows
{
    partial class ReportGeneratorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportGeneratorForm));
            this.HistoryDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.HitoryDirectoryLabel = new System.Windows.Forms.Label();
            this.AvailableFilesView = new System.Windows.Forms.ListView();
            this.FilterListView = new System.Windows.Forms.ListView();
            this.IgnoreButton = new System.Windows.Forms.Button();
            this.LogLevelComboBox = new System.Windows.Forms.ComboBox();
            this.LogLevelLabel = new System.Windows.Forms.Label();
            this.ReportTypesLabel = new System.Windows.Forms.Label();
            this.RunReportButton = new System.Windows.Forms.Button();
            this.UnIgnoreButton = new System.Windows.Forms.Button();
            this.TargetDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.TargetDirectoryLabel = new System.Windows.Forms.Label();
            this.ReportTypesListBox = new System.Windows.Forms.ListBox();
            this.CoverageFilesGroupBox = new System.Windows.Forms.GroupBox();
            this.FilesToAnalyzeLabel = new System.Windows.Forms.Label();
            this.IgnoredLabel = new System.Windows.Forms.Label();
            this.CoverageDirectoryGroupBox = new System.Windows.Forms.GroupBox();
            this.SourceDirectoriesListBox = new System.Windows.Forms.ListBox();
            this.SourceDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.AddSourceDirectoryButton = new System.Windows.Forms.Button();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.OptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.NoFilesTextBox = new System.Windows.Forms.TextBox();
            this.TargetDirectoryToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.FileTypeListBox = new System.Windows.Forms.ListBox();
            this.FileTypesLabel = new System.Windows.Forms.Label();
            this.CoverageFilesGroupBox.SuspendLayout();
            this.CoverageDirectoryGroupBox.SuspendLayout();
            this.OptionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // HistoryDirectoryTextBox
            // 
            this.HistoryDirectoryTextBox.Location = new System.Drawing.Point(11, 123);
            this.HistoryDirectoryTextBox.Name = "HistoryDirectoryTextBox";
            this.HistoryDirectoryTextBox.Size = new System.Drawing.Size(252, 20);
            this.HistoryDirectoryTextBox.TabIndex = 4;
            this.TargetDirectoryToolTip.SetToolTip(this.HistoryDirectoryTextBox, "The location to store the history of reports ran.");
            // 
            // HitoryDirectoryLabel
            // 
            this.HitoryDirectoryLabel.AutoSize = true;
            this.HitoryDirectoryLabel.Location = new System.Drawing.Point(6, 107);
            this.HitoryDirectoryLabel.Name = "HitoryDirectoryLabel";
            this.HitoryDirectoryLabel.Size = new System.Drawing.Size(126, 13);
            this.HitoryDirectoryLabel.TabIndex = 5;
            this.HitoryDirectoryLabel.Text = "history directory (optional)";
            this.TargetDirectoryToolTip.SetToolTip(this.HitoryDirectoryLabel, "The location to store the history of reports ran.");
            // 
            // AvailableFilesView
            // 
            this.AvailableFilesView.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.AvailableFilesView.Location = new System.Drawing.Point(16, 40);
            this.AvailableFilesView.Name = "AvailableFilesView";
            this.AvailableFilesView.Size = new System.Drawing.Size(194, 136);
            this.AvailableFilesView.TabIndex = 6;
            this.TargetDirectoryToolTip.SetToolTip(this.AvailableFilesView, "These files will be used in the report \r\ngeneration. Select a file on the left an" +
        "d \r\nclick add to move it to the ignored list.");
            this.AvailableFilesView.UseCompatibleStateImageBehavior = false;
            this.AvailableFilesView.View = System.Windows.Forms.View.List;
            this.AvailableFilesView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.AvailableFilesViewItemSelectionChanged);
            // 
            // FilterListView
            // 
            this.FilterListView.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.FilterListView.AllowDrop = true;
            this.FilterListView.Location = new System.Drawing.Point(297, 40);
            this.FilterListView.Name = "FilterListView";
            this.FilterListView.Size = new System.Drawing.Size(194, 136);
            this.FilterListView.TabIndex = 7;
            this.FilterListView.UseCompatibleStateImageBehavior = false;
            this.FilterListView.View = System.Windows.Forms.View.List;
            this.FilterListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.FilterListViewItemSelectionChanged);
            // 
            // IgnoreButton
            // 
            this.IgnoreButton.Location = new System.Drawing.Point(216, 47);
            this.IgnoreButton.Name = "IgnoreButton";
            this.IgnoreButton.Size = new System.Drawing.Size(75, 23);
            this.IgnoreButton.TabIndex = 8;
            this.IgnoreButton.Text = "add ->";
            this.IgnoreButton.UseVisualStyleBackColor = true;
            this.IgnoreButton.Click += new System.EventHandler(this.IgnoreButtonClick);
            // 
            // LogLevelComboBox
            // 
            this.LogLevelComboBox.FormattingEnabled = true;
            this.LogLevelComboBox.Items.AddRange(new object[] {
            "Verbose",
            "Info",
            "Error"});
            this.LogLevelComboBox.Location = new System.Drawing.Point(11, 41);
            this.LogLevelComboBox.Name = "LogLevelComboBox";
            this.LogLevelComboBox.Size = new System.Drawing.Size(252, 21);
            this.LogLevelComboBox.TabIndex = 9;
            this.LogLevelComboBox.Text = "Error";
            this.TargetDirectoryToolTip.SetToolTip(this.LogLevelComboBox, "The verbosity level for logging\r\nbased upon the log4net log level.");
            // 
            // LogLevelLabel
            // 
            this.LogLevelLabel.AutoSize = true;
            this.LogLevelLabel.Location = new System.Drawing.Point(6, 25);
            this.LogLevelLabel.Name = "LogLevelLabel";
            this.LogLevelLabel.Size = new System.Drawing.Size(66, 13);
            this.LogLevelLabel.TabIndex = 10;
            this.LogLevelLabel.Text = "log verbosity";
            this.TargetDirectoryToolTip.SetToolTip(this.LogLevelLabel, "The verbosity level for logging\r\nbased upon the log4net log level.");
            // 
            // ReportTypesLabel
            // 
            this.ReportTypesLabel.AutoSize = true;
            this.ReportTypesLabel.Location = new System.Drawing.Point(148, 172);
            this.ReportTypesLabel.Name = "ReportTypesLabel";
            this.ReportTypesLabel.Size = new System.Drawing.Size(62, 13);
            this.ReportTypesLabel.TabIndex = 12;
            this.ReportTypesLabel.Text = "report types";
            // 
            // RunReportButton
            // 
            this.RunReportButton.Location = new System.Drawing.Point(345, 420);
            this.RunReportButton.Name = "RunReportButton";
            this.RunReportButton.Size = new System.Drawing.Size(128, 36);
            this.RunReportButton.TabIndex = 13;
            this.RunReportButton.Text = "Run Report";
            this.RunReportButton.UseVisualStyleBackColor = true;
            this.RunReportButton.EnabledChanged += new System.EventHandler(this.RunReportButtonEnabledChanged);
            this.RunReportButton.Click += new System.EventHandler(this.RunReportButtonClick);
            // 
            // UnIgnoreButton
            // 
            this.UnIgnoreButton.Location = new System.Drawing.Point(216, 76);
            this.UnIgnoreButton.Name = "UnIgnoreButton";
            this.UnIgnoreButton.Size = new System.Drawing.Size(75, 23);
            this.UnIgnoreButton.TabIndex = 14;
            this.UnIgnoreButton.Text = "<- remove";
            this.UnIgnoreButton.UseVisualStyleBackColor = true;
            this.UnIgnoreButton.Click += new System.EventHandler(this.UnIgnoreButtonClick);
            // 
            // TargetDirectoryTextBox
            // 
            this.TargetDirectoryTextBox.Location = new System.Drawing.Point(11, 84);
            this.TargetDirectoryTextBox.Name = "TargetDirectoryTextBox";
            this.TargetDirectoryTextBox.Size = new System.Drawing.Size(252, 20);
            this.TargetDirectoryTextBox.TabIndex = 16;
            this.TargetDirectoryToolTip.SetToolTip(this.TargetDirectoryTextBox, "The location to store the generated report.\r\n");
            // 
            // TargetDirectoryLabel
            // 
            this.TargetDirectoryLabel.AutoSize = true;
            this.TargetDirectoryLabel.Location = new System.Drawing.Point(6, 66);
            this.TargetDirectoryLabel.Name = "TargetDirectoryLabel";
            this.TargetDirectoryLabel.Size = new System.Drawing.Size(77, 13);
            this.TargetDirectoryLabel.TabIndex = 17;
            this.TargetDirectoryLabel.Text = "target directory";
            this.TargetDirectoryToolTip.SetToolTip(this.TargetDirectoryLabel, "The location to store the generated report.");
            // 
            // ReportTypesListBox
            // 
            this.ReportTypesListBox.FormattingEnabled = true;
            this.ReportTypesListBox.Items.AddRange(new object[] {
            "Html",
            "HtmlSummary",
            "Latex",
            "LatexSummary",
            "TextSummary",
            "Xml",
            "XmlSummary"});
            this.ReportTypesListBox.Location = new System.Drawing.Point(151, 188);
            this.ReportTypesListBox.Name = "ReportTypesListBox";
            this.ReportTypesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.ReportTypesListBox.Size = new System.Drawing.Size(112, 95);
            this.ReportTypesListBox.TabIndex = 18;
            // 
            // CoverageFilesGroupBox
            // 
            this.CoverageFilesGroupBox.Controls.Add(this.FilesToAnalyzeLabel);
            this.CoverageFilesGroupBox.Controls.Add(this.IgnoredLabel);
            this.CoverageFilesGroupBox.Controls.Add(this.FilterListView);
            this.CoverageFilesGroupBox.Controls.Add(this.AvailableFilesView);
            this.CoverageFilesGroupBox.Controls.Add(this.IgnoreButton);
            this.CoverageFilesGroupBox.Controls.Add(this.UnIgnoreButton);
            this.CoverageFilesGroupBox.Location = new System.Drawing.Point(12, 231);
            this.CoverageFilesGroupBox.Name = "CoverageFilesGroupBox";
            this.CoverageFilesGroupBox.Size = new System.Drawing.Size(516, 185);
            this.CoverageFilesGroupBox.TabIndex = 20;
            this.CoverageFilesGroupBox.TabStop = false;
            this.CoverageFilesGroupBox.Text = "coverage files";
            this.TargetDirectoryToolTip.SetToolTip(this.CoverageFilesGroupBox, "These files will be used in the report \r\ngeneration. Select a file on the left an" +
        "d \r\nclick add to move it to the ignored list.");
            // 
            // FilesToAnalyzeLabel
            // 
            this.FilesToAnalyzeLabel.AutoSize = true;
            this.FilesToAnalyzeLabel.Location = new System.Drawing.Point(28, 23);
            this.FilesToAnalyzeLabel.Name = "FilesToAnalyzeLabel";
            this.FilesToAnalyzeLabel.Size = new System.Drawing.Size(64, 13);
            this.FilesToAnalyzeLabel.TabIndex = 16;
            this.FilesToAnalyzeLabel.Text = "analyze files";
            // 
            // IgnoredLabel
            // 
            this.IgnoredLabel.AutoSize = true;
            this.IgnoredLabel.Location = new System.Drawing.Point(304, 23);
            this.IgnoredLabel.Name = "IgnoredLabel";
            this.IgnoredLabel.Size = new System.Drawing.Size(69, 13);
            this.IgnoredLabel.TabIndex = 15;
            this.IgnoredLabel.Text = "files to ignore";
            // 
            // CoverageDirectoryGroupBox
            // 
            this.CoverageDirectoryGroupBox.Controls.Add(this.SourceDirectoriesListBox);
            this.CoverageDirectoryGroupBox.Controls.Add(this.SourceDirectoryTextBox);
            this.CoverageDirectoryGroupBox.Controls.Add(this.AddSourceDirectoryButton);
            this.CoverageDirectoryGroupBox.Controls.Add(this.BrowseButton);
            this.CoverageDirectoryGroupBox.Location = new System.Drawing.Point(12, 27);
            this.CoverageDirectoryGroupBox.Name = "CoverageDirectoryGroupBox";
            this.CoverageDirectoryGroupBox.Size = new System.Drawing.Size(516, 198);
            this.CoverageDirectoryGroupBox.TabIndex = 21;
            this.CoverageDirectoryGroupBox.TabStop = false;
            this.CoverageDirectoryGroupBox.Text = "coverage directories";
            this.TargetDirectoryToolTip.SetToolTip(this.CoverageDirectoryGroupBox, "Directories contain the coveragexml files. \r\nThese are currently being  generated" +
        " through\r\nVisual Studio.");
            // 
            // SourceDirectoriesListBox
            // 
            this.SourceDirectoriesListBox.FormattingEnabled = true;
            this.SourceDirectoriesListBox.Location = new System.Drawing.Point(16, 25);
            this.SourceDirectoriesListBox.Name = "SourceDirectoriesListBox";
            this.SourceDirectoriesListBox.Size = new System.Drawing.Size(475, 108);
            this.SourceDirectoriesListBox.TabIndex = 0;
            // 
            // SourceDirectoryTextBox
            // 
            this.SourceDirectoryTextBox.Location = new System.Drawing.Point(16, 139);
            this.SourceDirectoryTextBox.Name = "SourceDirectoryTextBox";
            this.SourceDirectoryTextBox.Size = new System.Drawing.Size(475, 20);
            this.SourceDirectoryTextBox.TabIndex = 2;
            // 
            // AddSourceDirectoryButton
            // 
            this.AddSourceDirectoryButton.Location = new System.Drawing.Point(121, 169);
            this.AddSourceDirectoryButton.Name = "AddSourceDirectoryButton";
            this.AddSourceDirectoryButton.Size = new System.Drawing.Size(89, 23);
            this.AddSourceDirectoryButton.TabIndex = 3;
            this.AddSourceDirectoryButton.Text = "Add";
            this.AddSourceDirectoryButton.UseVisualStyleBackColor = true;
            this.AddSourceDirectoryButton.Click += new System.EventHandler(this.AddSourceDirectoryButtonClick);
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(297, 169);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(89, 23);
            this.BrowseButton.TabIndex = 15;
            this.BrowseButton.Text = "Browse";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButtonClick);
            // 
            // OptionsGroupBox
            // 
            this.OptionsGroupBox.Controls.Add(this.FileTypesLabel);
            this.OptionsGroupBox.Controls.Add(this.FileTypeListBox);
            this.OptionsGroupBox.Controls.Add(this.LogLevelLabel);
            this.OptionsGroupBox.Controls.Add(this.HistoryDirectoryTextBox);
            this.OptionsGroupBox.Controls.Add(this.HitoryDirectoryLabel);
            this.OptionsGroupBox.Controls.Add(this.ReportTypesListBox);
            this.OptionsGroupBox.Controls.Add(this.LogLevelComboBox);
            this.OptionsGroupBox.Controls.Add(this.TargetDirectoryLabel);
            this.OptionsGroupBox.Controls.Add(this.ReportTypesLabel);
            this.OptionsGroupBox.Controls.Add(this.TargetDirectoryTextBox);
            this.OptionsGroupBox.Location = new System.Drawing.Point(534, 27);
            this.OptionsGroupBox.Name = "OptionsGroupBox";
            this.OptionsGroupBox.Size = new System.Drawing.Size(281, 389);
            this.OptionsGroupBox.TabIndex = 22;
            this.OptionsGroupBox.TabStop = false;
            this.OptionsGroupBox.Text = "config options";
            // 
            // NoFilesTextBox
            // 
            this.NoFilesTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.NoFilesTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.NoFilesTextBox.CausesValidation = false;
            this.NoFilesTextBox.Location = new System.Drawing.Point(28, 422);
            this.NoFilesTextBox.Multiline = true;
            this.NoFilesTextBox.Name = "NoFilesTextBox";
            this.NoFilesTextBox.ReadOnly = true;
            this.NoFilesTextBox.Size = new System.Drawing.Size(218, 36);
            this.NoFilesTextBox.TabIndex = 24;
            this.NoFilesTextBox.Text = "NOTE: at least one file must be listed in the analyze files listbox to run a cove" +
    "rage report";
            this.NoFilesTextBox.Visible = false;
            // 
            // TargetDirectoryToolTip
            // 
            this.TargetDirectoryToolTip.AutoPopDelay = 5000;
            this.TargetDirectoryToolTip.InitialDelay = 250;
            this.TargetDirectoryToolTip.ReshowDelay = 100;
            // 
            // FileTypeListBox
            // 
            this.FileTypeListBox.FormattingEnabled = true;
            this.FileTypeListBox.Items.AddRange(new object[] {
            ".xml",
            ".coveragexml"});
            this.FileTypeListBox.Location = new System.Drawing.Point(11, 188);
            this.FileTypeListBox.Name = "FileTypeListBox";
            this.FileTypeListBox.Size = new System.Drawing.Size(112, 95);
            this.FileTypeListBox.TabIndex = 19;
            // 
            // FileTypesLabel
            // 
            this.FileTypesLabel.AutoSize = true;
            this.FileTypesLabel.Location = new System.Drawing.Point(11, 171);
            this.FileTypesLabel.Name = "FileTypesLabel";
            this.FileTypesLabel.Size = new System.Drawing.Size(48, 13);
            this.FileTypesLabel.TabIndex = 20;
            this.FileTypesLabel.Text = "file types";
            // 
            // ReportGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 476);
            this.Controls.Add(this.NoFilesTextBox);
            this.Controls.Add(this.OptionsGroupBox);
            this.Controls.Add(this.CoverageDirectoryGroupBox);
            this.Controls.Add(this.CoverageFilesGroupBox);
            this.Controls.Add(this.RunReportButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReportGeneratorForm";
            this.Text = "Code Coverage Report Generator";
            this.Load += new System.EventHandler(this.ReportGeneratorFormLoad);
            this.CoverageFilesGroupBox.ResumeLayout(false);
            this.CoverageFilesGroupBox.PerformLayout();
            this.CoverageDirectoryGroupBox.ResumeLayout(false);
            this.CoverageDirectoryGroupBox.PerformLayout();
            this.OptionsGroupBox.ResumeLayout(false);
            this.OptionsGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox HistoryDirectoryTextBox;
        private System.Windows.Forms.Label HitoryDirectoryLabel;
        private System.Windows.Forms.ListView AvailableFilesView;
        private System.Windows.Forms.ListView FilterListView;
        private System.Windows.Forms.Button IgnoreButton;
        private System.Windows.Forms.ComboBox LogLevelComboBox;
        private System.Windows.Forms.Label LogLevelLabel;
        private System.Windows.Forms.Label ReportTypesLabel;
        private System.Windows.Forms.Button RunReportButton;
        private System.Windows.Forms.Button UnIgnoreButton;
        private System.Windows.Forms.TextBox TargetDirectoryTextBox;
        private System.Windows.Forms.Label TargetDirectoryLabel;
        private System.Windows.Forms.ListBox ReportTypesListBox;
        private System.Windows.Forms.GroupBox CoverageFilesGroupBox;
        private System.Windows.Forms.GroupBox CoverageDirectoryGroupBox;
        private System.Windows.Forms.ListBox SourceDirectoriesListBox;
        private System.Windows.Forms.TextBox SourceDirectoryTextBox;
        private System.Windows.Forms.Button AddSourceDirectoryButton;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.GroupBox OptionsGroupBox;
        private System.Windows.Forms.Label FilesToAnalyzeLabel;
        private System.Windows.Forms.Label IgnoredLabel;
        private System.Windows.Forms.TextBox NoFilesTextBox;
        private System.Windows.Forms.ToolTip TargetDirectoryToolTip;
        private System.Windows.Forms.Label FileTypesLabel;
        private System.Windows.Forms.ListBox FileTypeListBox;
    }
}

