using ActivityLogger.Datalayer;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ActivityLogger.GUI
{
    public partial class frmViewActivities : Form
    {
        public frmViewActivities()
        {
            InitializeComponent();

            FillActivities();
        }

        protected void FillActivities()
        {
            ActivityDataStore ads = ActivityDataStore.CreateInstance(Properties.Settings.Default.DataFilePath);

            DateTime time =  this.dateTimePicker1.Value;
            ActivityCollection activities = ads.GetActivities(time.Year, time.Month, time.Day);

            listView1.SuspendLayout();

            listView1.Items.Clear();
            for (int i = 0; i < activities.Count; i++)
            {
                Activity actNext = null;
                ActivityDuration duration;
                Activity act = activities[i];
                if (i < activities.Count - 1)
                {
                    actNext = activities[i + 1];
                    duration = new ActivityDuration(actNext.Timestamp.Subtract(activities[i].Timestamp));
                }
                else
                {
                    duration = new ActivityDuration(new TimeSpan(0));
                }

                ListViewItem item = new ListViewItem(new string[] { act.Timestamp.ToString("HH:mm:ss"),
                                                                    actNext != null ? actNext.Timestamp.ToString("HH:mm:ss") : "- ? -",
                                                                    duration.Ticks > 0 ? duration.ToString() : "- ? -", 
                                                                    act.Description });
                // not entirely happy of this duration solution - it seems a bit forced into the rest
                item.Tag = duration;
                listView1.Items.Add(item);
            }


            listView1.Columns[0].Width = -2;
            listView1.Columns[1].Width = -2;
            listView1.Columns[2].Width = -2;
            listView1.Columns[3].Width = -2;

            listView1.ResumeLayout();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            FillActivities();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.dateTimePicker1.Value = this.dateTimePicker1.Value.AddDays(-1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.dateTimePicker1.Value = this.dateTimePicker1.Value.AddDays(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FillActivities();
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ActivityDuration totalDuration = new ActivityDuration(new TimeSpan(0));
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    ActivityDuration actDuration = item.Tag as ActivityDuration;
                    if (actDuration != null)
                    {
                        totalDuration.Add(actDuration);
                    }
                }

                this.lblActivitiesTotalDuration.Text = totalDuration.Ticks > 0 ? totalDuration.ToString() : "?";
                this.lblSelectedActivities.Text = listView1.SelectedItems.Count.ToString();
                this.grpSummary.Enabled = true;
            }
            else
            {
                this.grpSummary.Enabled = false;
            }
        }
    }
}
