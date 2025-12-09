using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace CSharpFileScanner
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start($"mailto:tsgiannis@gmail.com?subject=CSharp File Scanner&body=Hello John,");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to open email client: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("https://Solutions4It.guru");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to open website: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}