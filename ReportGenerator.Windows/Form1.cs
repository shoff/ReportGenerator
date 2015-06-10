using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReportGenerator.Windows
{
    using System.IO;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void AddSourceDirectoryButton_Click(object sender, EventArgs e)
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

        }

        private void PopulateListView()
        {

            DirectoryInfo dinfo = new DirectoryInfo(this.SourceDirectoryTextBox.Text);
            FileInfo[] Files = dinfo.GetFiles("*.dll");
            foreach (FileInfo file in Files)
            {
                this.listView1.Items.Add(file.Name);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void IgnoreButton_Click(object sender, EventArgs e)
        {
            foreach (var item in this.listView1.SelectedItems)
            {
                this.listView2.Items.Add(item.ToString());
            }

        }
    }
}
