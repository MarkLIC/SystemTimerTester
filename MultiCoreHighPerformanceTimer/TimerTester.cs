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
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace MultiCoreHighPerformanceTimer
{
    internal sealed class TimerTester
    {
        [DllImport("kernel32")]
        public static extern int NtGetCurrentProcessorNumber();

        [DllImport("kernel32")]
        public static extern int GetCurrentProcessorNumber();

        [DllImport("winmm")]
        public static extern uint timeBeginPeriod(uint milliseconds);

        [DllImport("winmm")]
        public static extern uint timeGetTime();

        [DllImport("winmm")]
        public static extern uint timeEndPeriod(uint milliseconds);

        public static int? IdentifyCurrentProcessor()
        {
            try
            {
                return GetCurrentProcessorNumber();
            }
            catch (EntryPointNotFoundException)
            {
                // ignore
            }

            try
            {
                return NtGetCurrentProcessorNumber();
            }
            catch (EntryPointNotFoundException)
            {
               // ignore 
            }

            return null;
        }

        public static int GetNumberOfProcessors()
        {
            return new ManagementObjectSearcher("Select * from Win32_Processor").Get().Cast<ManagementBaseObject>().Sum(item => int.Parse(item["NumberOfCores"].ToString()));
        }
    }
}
