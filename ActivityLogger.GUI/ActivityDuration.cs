using System;

namespace ActivityLogger.GUI
{
    // I wonder if this class belongs in the DataLayer together with Activity ... I think not, actually.
    class ActivityDuration
    {
        public TimeSpan Duration { get; set; }

        public long Ticks { get { return this.Duration.Ticks; } }

        public ActivityDuration(TimeSpan duration)
        {
            this.Duration = duration;
        }

        public void Add(ActivityDuration activityDuration)
        {
            this.Duration = this.Duration.Add(activityDuration.Duration);
        }

        public override string ToString()
        {
            string hours = this.Duration.Hours.ToString();
            string mins = this.Duration.Minutes.ToString();
            string secs = this.Duration.Seconds.ToString();

            if (this.Duration.Hours > 0)
            {
                return String.Format("{0}h {1}m {2}s", hours, mins, secs);
            }
            if (this.Duration.Minutes > 0)
            {
                return String.Format("{0}m {1}s", mins, secs);
            }
            return String.Format("{0}s", secs);
        }
    }
}
