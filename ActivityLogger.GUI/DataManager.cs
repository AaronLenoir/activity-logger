using ActivityLogger.Datalayer;

using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityLogger.GUI
{
    class DataManager
    {

        public static void AddActivity(object stateInfo)
        {
            string description = (string)((Object[])stateInfo)[0];
            DateTime timestamp = (DateTime)((Object[])stateInfo)[1];

            ActivityDataStore ds = ActivityDataStore.CreateInstance(Properties.Settings.Default.DataFilePath);
            ds.AddActivity(new Activity(description, timestamp));
        }

    }
}
