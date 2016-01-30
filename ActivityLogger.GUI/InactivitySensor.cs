using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace ActivityLogger.GUI
{
    public class InactivitySensor
    {

        public class InactivityStartedEventArgs : EventArgs
        {
            public InactivityStartedEventArgs(TimeSpan triggerTime) { TriggerTime = triggerTime; }
            public TimeSpan TriggerTime { get; private set; }
        }

        public class InactivityEndedEventArgs : EventArgs
        {}

        public delegate void InactivityStartedHandler(object sender, InactivityStartedEventArgs e);
        private InactivityStartedHandler _startedHandler;

        public delegate void InactivityEndedHandler(object sender, InactivityEndedEventArgs e);
        private InactivityEndedHandler _endedHandler;

        private Timer _runningTimer;

        public InactivitySensor(TimeSpan triggerTime,
                                InactivityStartedHandler startedHandler,
                                InactivityEndedHandler endedHandler) 
        {
            _startedHandler = startedHandler;
            _endedHandler = endedHandler;
            TriggerTime = triggerTime;
        }

        public TimeSpan TriggerTime { get; private set; }

        public void Start()
        {
            if (!_started)
            {
                _started = true;
         
                ScheduleCheckOnce(TriggerTime);
            }
        }

        public void Stop()
        {
            if (_started)
            {
                _started = false;
            }
        }

        private bool _started = false;
        private bool _triggered = false;

        public void Tick(object state)
        {
            Log(string.Format("Tick."));

            if (!_started) {
                Log("Not started ...");
                return;
            }

            TimeSpan idleTime = RetrieveIdleTime();

            Log(string.Format("Idle Time: {0}.", idleTime));

            if (idleTime.TotalSeconds >= TriggerTime.TotalSeconds)
            {
                if (!_triggered)
                {
                    Log(string.Format("New idle detected."));
                    _triggered = true;
                    Log(string.Format("StartedHandler called."));
                    _startedHandler(this, new InactivityStartedEventArgs(TriggerTime));
                }
            } else {
                if (_triggered)
                {
                    Log(string.Format("Idle ended detected."));
                    _triggered = false;
                    Log(string.Format("EndedHandler called."));
                    _endedHandler(this, new InactivityEndedEventArgs());
                }
            }

            ScheduleCheckOnce(30);
        }

        private void ScheduleCheckOnce(int delayInSeconds)
        {
            Log(string.Format("Scheduling once (int): {0}", delayInSeconds));
            ScheduleCheckOnce(new TimeSpan(0, 0, delayInSeconds));
        }

        private void ScheduleCheckOnce(TimeSpan delay)
        {
            Log(string.Format("Scheduling once (TimeSpan): {0}", delay.TotalSeconds));

            if (_runningTimer != null) { _runningTimer.Dispose(); }

            _runningTimer = new Timer(Tick, null, delay, new TimeSpan(0, 0, 0, 0, -1));
        }

        private DateTime _testStartTime = DateTime.Now;

        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        private TimeSpan RetrieveIdleTime()
        {
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)LASTINPUTINFO.SizeOf;
            GetLastInputInfo(ref lastInputInfo);
            
            // Diff between current tickCount and tick count at last activity is
            // the elapsed idle time.
            int elapsedTicks = Environment.TickCount - (int)lastInputInfo.dwTime;

            // The "tick count" is the time in milliseconds since the system started.
            // So the elapsedTicks is expressed in milliseconds
            // Run a system for 24.9 days and you have trouble with this, since 
            // it'll wrap and be crazy.
            // Need to fix this!

            if (elapsedTicks != 0)
            {
                int i = 0;
                i = i + 1;
            }

            if (elapsedTicks > 0) { return new TimeSpan(0, 0, 0, 0, elapsedTicks); }
            else { return new TimeSpan(0); }
        }

        // TODO: Remove this.
        private void Log(string message)
        {
            System.IO.File.AppendAllText(@"C:\temp\act.log", string.Format("{0}: {1}\r\n", DateTime.Now.ToString(), message));
        }

    }
}
