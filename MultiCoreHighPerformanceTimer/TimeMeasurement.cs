/*
 *     Copyright 2011 Mark Rushakoff, Lafayette Instrument Company
 *
 *     This file is part of SystemTimerTester.
 *
 *     SystemTimerTester is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 *     SystemTimerTester is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 *
 *     You should have received a copy of the GNU General Public License along with Foobar. If not, see http://www.gnu.org/licenses/.
 *
 */

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
            return String.Format("Core: {3}{4}, DateTime.Millisecond: {0}, Environment.TickCount: {1}, Stopwatch.TimeStamp: {2}, timeGetTime: {5}, CPmS: {6}",
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
