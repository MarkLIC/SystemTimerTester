using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiCoreHighPerformanceTimer
{
    internal sealed class TimeMeasurement
    {
        public int core;
        public bool coreVerified;
        public int dateTimeMillisecond;
        public int envTickCount;
        public long stopwatchTimestamp;
        public uint mmTime;
        public ulong checksPerMs;
        
        public override string ToString()
        {
            return String.Format("Core: {3}{4}, DateTime.Millisecond: {0}, Environment.TickCount: {1}, Stopwatch.TimeStamp: {2}, timeGetTime: {5}, CPS: {6}",
                                 this.dateTimeMillisecond,
                                 this.envTickCount,
                                 this.stopwatchTimestamp,
                                 this.core,
                                 this.coreVerified ? String.Empty : "?",
                                 this.mmTime,
                                 this.checksPerMs);
        }
    }
}
