﻿/*
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
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;

namespace MultiCoreHighPerformanceTimer
{
    // Code taken from http://stackoverflow.com/questions/2510593/how-can-i-set-processor-affinity-in-net/2510708#2510708

    /// <summary>
    /// Gets and sets the processor affinity of the current thread.
    /// </summary>
    public static class ProcessorAffinity
    {
        static class Win32Native
        {
            //GetCurrentThread() returns only a pseudo handle. No need for a SafeHandle here.
            [DllImport("kernel32.dll")]
            public static extern IntPtr GetCurrentThread();

            [HostProtection(SelfAffectingThreading = true)]
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern UIntPtr SetThreadAffinityMask(IntPtr handle, UIntPtr mask);
        }

        public struct ProcessorAffinityHelper : IDisposable
        {
            UIntPtr lastaffinity;

            internal ProcessorAffinityHelper(UIntPtr lastaffinity)
            {
                this.lastaffinity = lastaffinity;
            }

            #region IDisposable Members

            public void Dispose()
            {
                if (lastaffinity != UIntPtr.Zero)
                {
                    Win32Native.SetThreadAffinityMask(Win32Native.GetCurrentThread(), lastaffinity);
                    lastaffinity = UIntPtr.Zero;
                }
            }

            #endregion
        }

        static ulong maskfromids(params int[] ids)
        {
            ulong mask = 0;
            foreach (int id in ids)
            {
                if (id < 0 || id >= Environment.ProcessorCount)
                    throw new ArgumentOutOfRangeException("CPUId", id.ToString());
                mask |= 1UL << id;
            }
            return mask;
        }

        /// <summary>
        /// Sets a processor affinity mask for the current thread.
        /// </summary>
        /// <param name="mask">A thread affinity mask where each bit set to 1 specifies a logical processor on which this thread is allowed to run. 
        /// <remarks>Note: a thread cannot specify a broader set of CPUs than those specified in the process affinity mask.</remarks> 
        /// </param>
        /// <returns>The previous affinity mask for the current thread.</returns>
        public static UIntPtr SetThreadAffinityMask(UIntPtr mask)
        {
            UIntPtr lastaffinity = Win32Native.SetThreadAffinityMask(Win32Native.GetCurrentThread(), mask);
            if (lastaffinity == UIntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return lastaffinity;
        }

        /// <summary>
        /// Sets the logical CPUs that the current thread is allowed to execute on.
        /// </summary>
        /// <param name="CPUIds">One or more logical processor identifier(s) the current thread is allowed to run on.<remarks>Note: numbering starts from 0.</remarks></param>
        /// <returns>The previous affinity mask for the current thread.</returns>
        public static UIntPtr SetThreadAffinity(params int[] CPUIds)
        {
            return SetThreadAffinityMask(((UIntPtr)maskfromids(CPUIds)));
        }

        /// <summary>
        /// Restrict a code block to run on the specified logical CPUs in conjuction with 
        /// the <code>using</code> statement.
        /// </summary>
        /// <param name="CPUIds">One or more logical processor identifier(s) the current thread is allowed to run on.<remarks>Note: numbering starts from 0.</remarks></param>
        /// <returns>A helper structure that will reset the affinity when its Dispose() method is called at the end of the using block.</returns>
        public static ProcessorAffinityHelper BeginAffinity(params int[] CPUIds)
        {
            return new ProcessorAffinityHelper(SetThreadAffinityMask(((UIntPtr)maskfromids(CPUIds))));
        }
    }
}
