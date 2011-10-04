using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ActivityLogger.Tracing
{
    public class ActivityLoggerTraceSource : TraceSource
    {
        private static ActivityLoggerTraceSource gInstance;

        private static int gStartEventId = 1000;
        private static int gStopEventId = 1001;

        protected ActivityLoggerTraceSource() : base("ActivityLogger", SourceLevels.All) { }

        protected static ActivityLoggerTraceSource Instance
        {
            get
            {
                if (ActivityLoggerTraceSource.gInstance == null)
                {
                    gInstance = new ActivityLoggerTraceSource();
                }
                return ActivityLoggerTraceSource.gInstance;
            }
        }

        [Conditional("TRACE")]
        public static void TraceStartConstructor(string iClassName)
        {
            ActivityLoggerTraceSource.Instance.TraceEvent(TraceEventType.Start, gStartEventId, "Constructor {0}", iClassName);
        }

        [Conditional("TRACE")]
        public static void TraceStopConstructor(string iClassName)
        {
            ActivityLoggerTraceSource.Instance.TraceEvent(TraceEventType.Stop, gStopEventId, "Constructor {0}", iClassName);
        }

        [Conditional("TRACE")]
        public static void TraceStart(string iClassName, string iMethodName) {
            ActivityLoggerTraceSource.Instance.TraceEvent(TraceEventType.Start, gStartEventId, "{0}.{1}", iClassName, iMethodName);
        }

        [Conditional("TRACE")]
        public static void TraceStop(string iClassName, string iMethodName)
        {
            ActivityLoggerTraceSource.Instance.TraceEvent(TraceEventType.Stop, gStopEventId, "{0}.{1}", iClassName, iMethodName);
        }
    }

    public class ALT : ActivityLoggerTraceSource
    {

    }
}
