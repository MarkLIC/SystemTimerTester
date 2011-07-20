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
