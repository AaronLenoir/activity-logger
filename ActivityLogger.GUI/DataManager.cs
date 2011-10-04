using ActivityLogger.Datalayer;
using ActivityLogger.Tracing;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ActivityLogger.GUI
{
    class DataManager
    {

        protected const String cTimeRegex = "^[0-9]{1,2}:[0-9]{2} ?([PpAa][Mm])?( ){1}";

        public static void AddActivity(object stateInfo)
        {
            ALT.TraceStart("DateManager", "AddActivity");

            string description = (string)((Object[])stateInfo)[0];
            DateTime timestamp = (DateTime)((Object[])stateInfo)[1];

            // Maybe we need to overwrite the datetime with the time as set in the message
            OverwriteDateTime(ref description, ref timestamp);

            ActivityDataStore ds = ActivityDataStore.CreateInstance(Properties.Settings.Default.DataFilePath);
            ds.AddActivity(new Activity(description, timestamp));

            ALT.TraceStop("DateManager", "AddActivity");
        }

        public static void OverwriteDateTime(ref string description, ref DateTime timestamp)
        {
            ALT.TraceStart("DateManager", "OverwriteDateTime");

            Regex reg = new Regex(cTimeRegex);
            Match m = reg.Match(description);
            if (m.Success)
            {
                DateTime time;
                if (DateTime.TryParse(m.Value, out time))
                {
                    // In this case, the user wants to overwrite the current time of day with the time of day he starts the message with
                    // We don't care about seconds
                    timestamp = new DateTime(timestamp.Year, timestamp.Month, timestamp.Day, time.Hour, time.Minute, 0);
                    // So in that case, we also need to remove that time prefix from the actual message ... so remove the regex match
                    description = reg.Replace(description, String.Empty);
                }
            }

            ALT.TraceStop("DateManager", "OverwriteDateTime");
        }

    }
}
