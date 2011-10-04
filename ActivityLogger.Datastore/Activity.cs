using ActivityLogger.Tracing;

using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityLogger.Datalayer
{
    public class Activity
    {

        public Activity(string description) : this(description, DateTime.Now)
        { }

        public Activity(string description, DateTime timestamp)
        {
           ALT.TraceStartConstructor("Activity");

            this.Description = description;
            // We round the timestamp to the nearest second (we don't care about the miliseconds
            this.Timestamp = new DateTime(timestamp.Year, timestamp.Month, timestamp.Day, timestamp.Hour, timestamp.Minute, timestamp.Second);

            ALT.TraceStopConstructor("Activity");
        }

        public DateTime Timestamp { get; set; }

        public string Description { get; set; }

    }
}
