namespace ReportGenerator.Windows
{
    partial class Form1
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
            this.SourceDirectoriesListBox = new System.Windows.Forms.ListBox();
            this.SourceDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.AddSourceDirectoryButton = new System.Windows.Forms.Button();
            this.HistoryDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.HitoryDirectoryLabel = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.listView2 = new System.Windows.Forms.ListView();
            this.IgnoreButton = new System.Windows.Forms.Button();
            this.LogLevelComboBox = new System.Windows.Forms.ComboBox();
            this.LogLevelLabel = new System.Windows.Forms.Label();
            this.ReportTypeComboBox = new System.Windows.Forms.ComboBox();
            this.ReportTypesLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SourceDirectoriesListBox
            // 
            this.SourceDirectoriesListBox.FormattingEnabled = true;
            this.SourceDirectoriesListBox.Location = new System.Drawing.Point(11, 27);
            this.SourceDirectoriesListBox.Name = "SourceDirectoriesListBox";
            this.SourceDirectoriesListBox.Size = new System.Drawing.Size(335, 186);
            this.SourceDirectoriesListBox.TabIndex = 0;
            // 
            // SourceDirectoryTextBox
            // 
            this.SourceDirectoryTextBox.Location = new System.Drawing.Point(12, 215);
            this.SourceDirectoryTextBox.Name = "SourceDirectoryTextBox";
            this.SourceDirectoryTextBox.Size = new System.Drawing.Size(335, 20);
            this.SourceDirectoryTextBox.TabIndex = 2;
            // 
            // AddSourceDirectoryButton
            // 
            this.AddSourceDirectoryButton.Location = new System.Drawing.Point(11, 241);
            this.AddSourceDirectoryButton.Name = "AddSourceDirectoryButton";
            this.AddSourceDirectoryButton.Size = new System.Drawing.Size(75, 23);
            this.AddSourceDirectoryButton.TabIndex = 3;
            this.AddSourceDirectoryButton.Text = "Add";
            this.AddSourceDirectoryButton.UseVisualStyleBackColor = true;
            this.AddSourceDirectoryButton.Click += new System.EventHandler(this.AddSourceDirectoryButton_Click);
            // 
            // HistoryDirectoryTextBox
            // 
            this.HistoryDirectoryTextBox.Location = new System.Drawing.Point(354, 167);
            this.HistoryDirectoryTextBox.Name = "HistoryDirectoryTextBox";
            this.HistoryDirectoryTextBox.Size = new System.Drawing.Size(252, 20);
            this.HistoryDirectoryTextBox.TabIndex = 4;
            // 
            // HitoryDirectoryLabel
            // 
            this.HitoryDirectoryLabel.AutoSize = true;
            this.HitoryDirectoryLabel.Location = new System.Drawing.Point(357, 151);
            this.HitoryDirectoryLabel.Name = "HitoryDirectoryLabel";
            this.HitoryDirectoryLabel.Size = new System.Drawing.Size(126, 13);
            this.HitoryDirectoryLabel.TabIndex = 5;
            this.HitoryDirectoryLabel.Text = "history directory (optional)";
            // 
            // listView1
            // 
            this.listView1.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.listView1.Location = new System.Drawing.Point(12, 305);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(239, 214);
            this.listView1.TabIndex = 6;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            // 
            // listView2
            // 
            this.listView2.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.listView2.Location = new System.Drawing.Point(340, 305);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(257, 214);
            this.listView2.TabIndex = 7;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.List;
            // 
            // IgnoreButton
            // 
            this.IgnoreButton.Location = new System.Drawing.Point(257, 390);
            this.IgnoreButton.Name = "IgnoreButton";
            this.IgnoreButton.Size = new System.Drawing.Size(75, 23);
            this.IgnoreButton.TabIndex = 8;
            this.IgnoreButton.Text = "ignore";
            this.IgnoreButton.UseVisualStyleBackColor = true;
            this.IgnoreButton.Click += new System.EventHandler(this.IgnoreButton_Click);
            // 
            // LogLevelComboBox
            // 
            this.LogLevelComboBox.FormattingEnabled = true;
            this.LogLevelComboBox.Items.AddRange(new object[] {
            "Verbose",
            "Info",
            "Error"});
            this.LogLevelComboBox.Location = new System.Drawing.Point(354, 105);
            this.LogLevelComboBox.Name = "LogLevelComboBox";
            this.LogLevelComboBox.Size = new System.Drawing.Size(252, 21);
            this.LogLevelComboBox.TabIndex = 9;
            this.LogLevelComboBox.Text = "Error";
            // 
            // LogLevelLabel
            // 
            this.LogLevelLabel.AutoSize = true;
            this.LogLevelLabel.Location = new System.Drawing.Point(357, 89);
            this.LogLevelLabel.Name = "LogLevelLabel";
            this.LogLevelLabel.Size = new System.Drawing.Size(66, 13);
            this.LogLevelLabel.TabIndex = 10;
            this.LogLevelLabel.Text = "log verbosity";
            // 
            // ReportTypeComboBox
            // 
            this.ReportTypeComboBox.FormattingEnabled = true;
            this.ReportTypeComboBox.Items.AddRange(new object[] {
            "Html",
            "HtmlSummary",
            "Latex",
            "LatexSummary",
            "TextSummary",
            "Xml",
            "XmlSummary"});
            this.ReportTypeComboBox.Location = new System.Drawing.Point(354, 43);
            this.ReportTypeComboBox.Name = "ReportTypeComboBox";
            this.ReportTypeComboBox.Size = new System.Drawing.Size(252, 21);
            this.ReportTypeComboBox.TabIndex = 11;
            this.ReportTypeComboBox.Text = "Html";
            // 
            // ReportTypesLabel
            // 
            this.ReportTypesLabel.AutoSize = true;
            this.ReportTypesLabel.Location = new System.Drawing.Point(357, 27);
            this.ReportTypesLabel.Name = "ReportTypesLabel";
            this.ReportTypesLabel.Size = new System.Drawing.Size(62, 13);
            this.ReportTypesLabel.TabIndex = 12;
            this.ReportTypesLabel.Text = "report types";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 606);
            this.Controls.Add(this.ReportTypesLabel);
            this.Controls.Add(this.ReportTypeComboBox);
            this.Controls.Add(this.LogLevelLabel);
            this.Controls.Add(this.LogLevelComboBox);
            this.Controls.Add(this.IgnoreButton);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.HitoryDirectoryLabel);
            this.Controls.Add(this.HistoryDirectoryTextBox);
            this.Controls.Add(this.AddSourceDirectoryButton);
            this.Controls.Add(this.SourceDirectoryTextBox);
            this.Controls.Add(this.SourceDirectoriesListBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox SourceDirectoriesListBox;
        private System.Windows.Forms.TextBox SourceDirectoryTextBox;
        private System.Windows.Forms.Button AddSourceDirectoryButton;
        private System.Windows.Forms.TextBox HistoryDirectoryTextBox;
        private System.Windows.Forms.Label HitoryDirectoryLabel;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.Button IgnoreButton;
        private System.Windows.Forms.ComboBox LogLevelComboBox;
        private System.Windows.Forms.Label LogLevelLabel;
        private System.Windows.Forms.ComboBox ReportTypeComboBox;
        private System.Windows.Forms.Label ReportTypesLabel;
    }
}

