using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FunBlock
{
    public partial class MainForm : Form
    {
        bool blChanged;
        string strOldHosts;

        // Function: Read string from a file
        private string ReadFile(string file)
        {
            StreamReader reader = new StreamReader(file, Encoding.Default);
            string data = reader.ReadToEnd();
            reader.Close();

            return data;
        }

        // Function: Save string to a file
        private void SaveFile(string file, string data)
        {
            StreamWriter writer = new StreamWriter(file);
            writer.Write(data);
            writer.Close();
        }

        void ChangeStatus()
        {
            if (blChanged)
            {
                sttStatus.Text = "Active";
            }
            else
            {
                sttStatus.Text = "Not active";
            }

            notifyIcon1.Text = this.Text + " - " + sttStatus.Text;
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            blChanged = FunBlock.Properties.Settings.Default.Changed;
            txtNewHosts.Text = FunBlock.Properties.Settings.Default.NewHosts;

            if (blChanged == false)
                strOldHosts = ReadFile(@"C:\Windows\System32\drivers\etc\hosts");
            else
            {
                this.Hide();
                strOldHosts = FunBlock.Properties.Settings.Default.OldHosts;
            }

            notifyIcon1.Icon = this.Icon;

            ChangeStatus();
            timer1_Tick(null, null);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dtmNow = DateTime.Now;

            if (dtmNow.Hour >= 20 && dtmNow.Hour < 23 && blChanged == true)
            {
                SaveFile(@"C:\Windows\System32\drivers\etc\hosts", strOldHosts);
                blChanged = false;
                ChangeStatus();
            }
            else if ((dtmNow.Hour < 20 || dtmNow.Hour >= 23) && blChanged == false)
            {
                SaveFile(@"C:\Windows\System32\drivers\etc\hosts", strOldHosts + txtNewHosts.Text);
                blChanged = true;
                ChangeStatus();
            }

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                FunBlock.Properties.Settings.Default.Changed = blChanged;
                FunBlock.Properties.Settings.Default.OldHosts = strOldHosts;
                FunBlock.Properties.Settings.Default.NewHosts = txtNewHosts.Text;
                FunBlock.Properties.Settings.Default.Save();
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = !this.Visible;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void notifyIcon1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(Cursor.Position);
            }
        }
    }
}
