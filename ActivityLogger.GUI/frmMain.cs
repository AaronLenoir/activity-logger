﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace ActivityLogger.GUI
{
    public partial class frmMain : Form
    {
        Rectangle mScreenSize;
        InactivitySensor mInactivitySensor;
        private readonly TimeSpan _inactivityTime;
        private readonly string _inactivityMessage;
        private readonly string _inactivityEndMessage;

        public frmMain()
        {
            InitializeComponent();
            this.mScreenSize = Screen.PrimaryScreen.Bounds;

            _inactivityTime = new TimeSpan(0, 0, Properties.Settings.Default.IdleTriggerTime);

            _inactivityMessage = Properties.Settings.Default.InactivityMessage.Replace("{INACTIVITY_TIME}",
                                                                                        _inactivityTime.TotalMinutes.ToString());
            _inactivityEndMessage = Properties.Settings.Default.InactivityEndMessage.Replace("{INACTIVITY_TIME}",
                                                                                              _inactivityTime.TotalMinutes.ToString());
            logInactivityMenuItem.Text = Properties.Settings.Default.InactivityMenuMessage.Replace("{INACTIVITY_TIME}",
                                                                                             _inactivityTime.TotalMinutes.ToString());

        }

        private void CreateDefaultDataFile()
        {
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
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            LogStartup();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            LogShutdown();
        }

        private void SetPosition(Point baseLocation)
        {
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
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                this.Hide();

                DataManager.AddActivityAsync(this.textBox1.Text, DateTime.Now);

                this.textBox1.Text = String.Empty;
            }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            this.Hide();
            this.CreateDefaultDataFile();
            this.RefreshMenuItems();
        }

        private void aclIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.SetPosition(System.Windows.Forms.Cursor.Position);
                this.textBox1.Focus();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RefreshMenuItems()
        {
            SetAutoStartMenuItem();
            SetLogStartupShutdownMenuItem();
            SetLogInactivityMenuItem();
        }

        private void SetLogStartupShutdownMenuItem()
        {
            if (Properties.Settings.Default.LogStartupShutdown)
            {
                logStartupShutdownMenuItem.CheckState = CheckState.Checked;
            }
            else
            {
                logStartupShutdownMenuItem.CheckState = CheckState.Unchecked;
            }
        }

        private void SetLogInactivityMenuItem()
        {
            if (Properties.Settings.Default.LogInactivity)
            {
                logInactivityMenuItem.CheckState = CheckState.Checked;
                StartInactivityLogger();
            }
            else
            {
                logInactivityMenuItem.CheckState = CheckState.Unchecked;
                StopInactivityLogger();
            }

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
            if (fd.ShowDialog() == DialogResult.OK)
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

        private void logStartupShutdownMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.LogStartupShutdown = !Properties.Settings.Default.LogStartupShutdown;
            Properties.Settings.Default.Save();
            SetLogStartupShutdownMenuItem();
        }

        private void logInactivityMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.LogInactivity = !Properties.Settings.Default.LogInactivity;
            Properties.Settings.Default.Save();
            SetLogInactivityMenuItem();
        }

        private void LogStartup()
        {
            if (Properties.Settings.Default.LogStartupShutdown)
            {
                DataManager.AddActivityAsync("Activity Logger Started", DateTime.Now);
            }
        }

        private void LogShutdown()
        {
            if (Properties.Settings.Default.LogStartupShutdown)
            {
                DataManager.AddActivity("Activity Logger Stopped", DateTime.Now);
            }
        }

        private void StartInactivityLogger()
        {
            if (mInactivitySensor == null)
            {
                mInactivitySensor = new InactivitySensor(_inactivityTime,
                                                         LogInactivityStarted, LogInactivityEnded);
            }

            mInactivitySensor.Start();
        }

        private void StopInactivityLogger()
        {
            if (mInactivitySensor != null)
            {
                mInactivitySensor.Stop();
            }
        }

        private void LogInactivityStarted(object sender, InactivitySensor.InactivityStartedEventArgs e)
        {
            if (Properties.Settings.Default.LogInactivity)
            {
                DataManager.AddActivityAsync(_inactivityMessage, DateTime.Now);
            }
        }

        private void LogInactivityEnded(object sender, InactivitySensor.InactivityEndedEventArgs e)
        {
            if (Properties.Settings.Default.LogInactivity)
            {
                DataManager.AddActivityAsync(_inactivityEndMessage, DateTime.Now);
            }
        }
    }
}
