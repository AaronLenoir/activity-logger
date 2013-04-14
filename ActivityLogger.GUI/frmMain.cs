using ActivityLogger.Tracing;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ActivityLogger.GUI
{
    public partial class frmMain : Form
    {

        Rectangle mScreenSize;

        public frmMain()
        {
            ALT.TraceStartConstructor("frmMain");

            InitializeComponent();
            this.mScreenSize = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

            ALT.TraceStopConstructor("frmMain");
        }

        private void CreateDefaultDataFile()
        {
            ALT.TraceStart("frmMain", "CreateDefaultDataFile");

            if (Properties.Settings.Default.DataFilePath == String.Empty)
            {
                // This is the first run
                string appDataFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Activity Logger");

                try
                {
                    if (!System.IO.Directory.Exists(appDataFolder))
                    {
                        System.IO.Directory.CreateDirectory(appDataFolder);
                    }
                    Properties.Settings.Default.DataFilePath = System.IO.Path.Combine(appDataFolder, Properties.Settings.Default.DefaultFileName);
                    Properties.Settings.Default.Save();
                }
                catch
                {
                    MessageBox.Show("Could not create the default activity storage file, please specify a location of this file.", "Could not create activity file.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ChangeDataFile();
                }
            }

            ALT.TraceStop("frmMain", "CreateDefaultDataFile");
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            ALT.TraceStart("frmMain", "frmMain");

            ALT.TraceStop("frmMain", "frmMain_Load");
        }

        private void SetPosition(Point baseLocation)
        {
            ALT.TraceStart("frmMain", "aclIcon_Click");

            int x = baseLocation.X;
            int y = baseLocation.Y;

            //x += this.Width;
            if ((x+this.Width) > this.mScreenSize.Right)
            {
                x -= this.Width;
            }

            y -= this.Height;
            if (y < 0)
            {
                y = 0;
            }

            this.Location = new Point(x, y);

            //this.Size = new Size(this.textBox1.Size.Width + this.label1.Width, this.textBox1.Size.Height);

            ALT.TraceStop("frmMain", "aclIcon_Click");
        }

        private void label1_Click(object sender, EventArgs e)
        {
            ALT.TraceStart("frmMain", "label1_Click");
            this.Hide();
            ALT.TraceStop("frmMain", "label1_Click");
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            ALT.TraceStart("frmMain", "textBox1_KeyUp");

            if (e.KeyCode == Keys.Return)
            {
                this.Hide();

                // Queue the activity save call
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(DataManager.AddActivity), 
                                                              new Object[] { this.textBox1.Text, DateTime.Now });

                this.textBox1.Text = String.Empty;
            }

            ALT.TraceStop("frmMain", "textBox1_KeyUp");
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            this.Hide();
            this.CreateDefaultDataFile();
            this.SetAutoStartMenuItem();
        }

        private void aclIcon_MouseClick(object sender, MouseEventArgs e)
        {
            ALT.TraceStart("frmMain", "aclIcon_MouseClick");

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.SetPosition(System.Windows.Forms.Cursor.Position);
                this.textBox1.Focus();
            }

            ALT.TraceStop("frmMain", "aclIcon_MouseClick");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetAutoStartMenuItem()
        {
            if (AutoStartHelper.HasAutoStartConfigured())
            {
                autoStartMenuItem.CheckState = CheckState.Checked;
            }
            else
            {
                autoStartMenuItem.CheckState = CheckState.Unchecked;
            }
        }

        private void autoStartMenuItem_Click(object sender, EventArgs e)
        {
            if (AutoStartHelper.HasAutoStartConfigured())
            {
                AutoStartHelper.RemoveAutoStart();
            }
            else
            {
                AutoStartHelper.SetAutoStart();
            }

            SetAutoStartMenuItem();
        }

        private void changeDataFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeDataFile();
        }

        private void ChangeDataFile()
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.AddExtension = true;
            fd.DefaultExt = "dat";
            fd.InitialDirectory = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(Properties.Settings.Default.DataFilePath));
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.DataFilePath = fd.FileName;
                Properties.Settings.Default.Save();
            }
        }

        private void viewActivitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmViewActivities form = new frmViewActivities();
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Show();
        }

    }
}
