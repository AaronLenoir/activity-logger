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
            // Clear our old timer, we'll start a new one anyway.
            _runningTimer.Dispose();
            _runningTimer = null;

            if (!_started) {
                return;
            }

            TimeSpan idleTime = RetrieveIdleTime();

            if (idleTime.TotalSeconds >= TriggerTime.TotalSeconds)
            {
                if (!_triggered)
                {
                    _triggered = true;
                    _startedHandler(this, new InactivityStartedEventArgs(TriggerTime));
                }
            } else {
                if (_triggered)
                {
                    _triggered = false;
                    _endedHandler(this, new InactivityEndedEventArgs());
                }
            }

            ScheduleCheckOnce(30);
        }

        private void ScheduleCheckOnce(int delayInSeconds)
        {
            ScheduleCheckOnce(new TimeSpan(0, 0, delayInSeconds));
        }

        private void ScheduleCheckOnce(TimeSpan delay)
        {
            // Only schedule a run if no other timer is running.
            if (_runningTimer == null) {
                _runningTimer = new Timer(Tick, null, delay, new TimeSpan(0, 0, 0, 0, -1));
            }
        }

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

    }
}
