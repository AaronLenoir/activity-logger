using ActivityLogger.Datalayer;
using ActivityLogger.Tracing;

using System;

namespace ActivityLogger.GUI
{
    class DataManager
    {

        protected const String cTimeRegex = "^[0-9]{1,2}:[0-9]{2} ?([PpAa][Mm])?( ){1}";

        public static void AddActivityAsync(string message, DateTime timestamp)
        {
            // Queue the activity save call
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(AddActivity),
                                                          new Object[] { message, timestamp });
        }

        public static void AddActivity(object stateInfo)
        {
            ALT.TraceStart("DateManager", "AddActivity");

            string description = (string)((Object[])stateInfo)[0];
            DateTime timestamp = (DateTime)((Object[])stateInfo)[1];

            ActivityDataStore ds = ActivityDataStore.CreateInstance(Properties.Settings.Default.DataFilePath);
            ds.AddActivity(new Activity(description, timestamp));

            ALT.TraceStop("DateManager", "AddActivity");
        }

    }
}
