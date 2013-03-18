using ActivityLogger.Tracing;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ActivityLogger.Datalayer
{
    public class Activity
    {

        protected const String cTimeRegex = "^[0-9]{1,2}:[0-9]{2} ?([PpAa][Mm])?( ){1}";

        public Activity(string description)
        {
            ALT.TraceStartConstructor("Activity");

            // Find out if the description holds a timestamp of its own
            DateTime timestamp = DateTime.Now;
            OverwriteDateTime(ref description, ref timestamp);

            this.Description = description;
            this.Timestamp = RoundDateTime(timestamp);

            ALT.TraceStopConstructor("Activity");
        }

        public Activity(string description, DateTime timestamp)
        {
           ALT.TraceStartConstructor("Activity");

           this.OverwriteDateTime(ref description, ref timestamp);

            this.Description = description;
            this.Timestamp = RoundDateTime(timestamp);

            ALT.TraceStopConstructor("Activity");
        }

        public DateTime Timestamp { get; set; }

        public string Description { get; set; }

        private DateTime RoundDateTime(DateTime timestamp)
        {
            return timestamp.AddMilliseconds(-1 * timestamp.Millisecond);
        }

        public void OverwriteDateTime(ref string description, ref DateTime timestamp)
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
                else
                {
                    // No timestamp specified in the description, so assuming the timestamp is "now".
                    timestamp = DateTime.Now;
                }
            }

            ALT.TraceStop("DateManager", "OverwriteDateTime");
        }

    }
}
