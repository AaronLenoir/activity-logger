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
                Activity act = activities[i];
                if (i < activities.Count - 1)
                {
                    actNext = activities[i + 1];
                }

                ListViewItem item = new ListViewItem(new string[] { act.Timestamp.ToString("HH:mm:ss"), actNext != null ? actNext.Timestamp.ToString("HH:mm:ss") : "- ? -", act.Description });
                listView1.Items.Add(item);
            }


            listView1.Columns[0].Width = -2;
            listView1.Columns[1].Width = -2;
            listView1.Columns[2].Width = -2;

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
    }
}
