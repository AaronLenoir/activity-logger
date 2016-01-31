using ActivityLogger.Datalayer;
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

        public static void AddActivity(string message, DateTime timestamp)
        {
            AddActivity(new Object[] { message, timestamp });
        }

        private static void AddActivity(object stateInfo)
        {
            string description = (string)((Object[])stateInfo)[0];
            DateTime timestamp = (DateTime)((Object[])stateInfo)[1];

            ActivityDataStore ds = ActivityDataStore.CreateInstance(Properties.Settings.Default.DataFilePath);
            ds.AddActivity(new Activity(description, timestamp));
        }
    }
}
