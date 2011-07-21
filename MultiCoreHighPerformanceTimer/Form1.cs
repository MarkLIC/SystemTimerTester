﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiCoreHighPerformanceTimer
{
    public partial class Form1 : Form
    {
        private TimeMeasurement[] measurements;
        private readonly object locker = new object();
        private readonly StringBuilder logBuilder= new StringBuilder();

        public Form1()
        {
            InitializeComponent();
        }

        private Action GenerateActionForCore(int i, Barrier barrier, bool runCps)
        {
            return () =>
                       {
                           using (ProcessorAffinity.BeginAffinity(i))
                           {
                               int? currentProc = TimerTester.IdentifyCurrentProcessor();
                               var measurement = new TimeMeasurement {core = currentProc ?? i, coreVerified = currentProc.HasValue};
                               if (barrier != null)
                               {
                                   barrier.SignalAndWait();
                               }

                               DateTime now = DateTime.Now;
                               measurement.dateTimeMillisecond = now.Millisecond;

                               if (barrier != null)
                               {
                                   barrier.SignalAndWait();
                               }

                               int envTickCount = Environment.TickCount;
                               measurement.envTickCount = envTickCount;

                               if (barrier != null)
                               {
                                   barrier.SignalAndWait();
                               }

                               long stopwatchTimestamp = Stopwatch.GetTimestamp();
                               measurement.stopwatchTimestamp = stopwatchTimestamp;

                               if (barrier != null)
                               {
                                   barrier.SignalAndWait();
                               }

                               uint mmTime = TimerTester.timeGetTime();
                               measurement.mmTime = mmTime;

                               if (barrier != null)
                               {
                                   barrier.SignalAndWait();
                               }

                               if (runCps)
                               {
                                   uint start = TimerTester.timeGetTime();
                                   uint waitFor = start + 1;
                                   uint stopAt = start + 2;

                                   while (TimerTester.timeGetTime() < waitFor)
                                   {
                                       // do nothing
                                   }

                                   ulong cps = 0;
                                   while (TimerTester.timeGetTime() < stopAt)
                                   {
                                       cps++;
                                   }

                                   measurement.checksPerMs = cps;
                               }

                               lock (this.locker)
                               {
                                   this.measurements[i] = measurement;
                               }
                           }
                       };
        }

        private void buttonRunTest_Click(object sender, EventArgs e)
        {
            var summarizer = new Summarizer();
            TimerTester.timeBeginPeriod(1);

            this.textBoxLog.Clear();

            int numProcessors = TimerTester.GetNumberOfProcessors();
            this.logBuilder.Append("Number of cores: " + numProcessors + Environment.NewLine);
            this.logBuilder.Append("Stopwatch frequency (Hz): " + Stopwatch.Frequency + Environment.NewLine);

            const int runMax = 5;
            for (int j = 0; j < runMax; j++)
            {
                this.measurements = new TimeMeasurement[numProcessors];

                this.logBuilder.Append(Environment.NewLine + "===Synchronized parallel task [" + (j + 1) + '/' + runMax + "]===" + Environment.NewLine);

                using (var barrier = new Barrier(numProcessors))
                {
                    Parallel.Invoke(Enumerable.Range(0, numProcessors).Select(i => this.GenerateActionForCore(i, barrier, true)).ToArray());
                }

                this.RecordMeasurements();
                summarizer.AddSynchronizedMeasurements(this.measurements);

                this.measurements = new TimeMeasurement[numProcessors];
                this.logBuilder.Append(Environment.NewLine + "===Unsynchronized parallel task [" + (j + 1) + '/' + runMax + "]===" + Environment.NewLine);
                Parallel.Invoke(Enumerable.Range(0, numProcessors).Select(i => this.GenerateActionForCore(i, null, true)).ToArray());

                this.RecordMeasurements();
                summarizer.AddUnsynchronizedMeasurements(this.measurements);

                this.measurements = new TimeMeasurement[numProcessors];
                this.logBuilder.Append(Environment.NewLine + "===Serial task [" + (j + 1) + '/' + runMax + "]===" + Environment.NewLine);
                for (int i = 0; i < numProcessors; i++)
                {
                    this.GenerateActionForCore(i, null, false)();
                }

                this.RecordMeasurements();
                summarizer.AddSerialMeasurements(this.measurements);
            }

            TimerTester.timeEndPeriod(1);

            this.textBoxLog.Text = this.logBuilder.ToString();

            summarizer.Summarize(this.richTextBoxSummary);
        }

        private void RecordMeasurements()
        {
            lock (this.locker)
            {
                this.logBuilder.Append(String.Join(Environment.NewLine, this.measurements.Select(m => m.ToString())) + Environment.NewLine);

                long minStopwatch = this.measurements.Min(m => m.stopwatchTimestamp);
                foreach (var measurement in this.measurements)
                {
                    long diff = measurement.stopwatchTimestamp - minStopwatch;
                    this.logBuilder.Append(String.Format("Core {0} swing: {1} ticks, {2:F10}s; on time: {3}" + Environment.NewLine, measurement.core, diff, (double)diff/Stopwatch.Frequency, TimeSpan.FromSeconds((double)measurement.stopwatchTimestamp/Stopwatch.Frequency)));
                }
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.richTextBoxSummary.Text + Environment.NewLine + "~~~~~~" + Environment.NewLine + this.textBoxLog.Text);
        }
    }
}
