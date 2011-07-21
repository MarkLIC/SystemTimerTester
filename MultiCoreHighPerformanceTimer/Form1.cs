using System;
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

        public Form1()
        {
            InitializeComponent();
        }

        private Action GenerateActionForCore(int i, Barrier barrier)
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
            this.measurements = new TimeMeasurement[numProcessors];

            this.textBoxLog.AppendText("Number of cores: " + numProcessors + Environment.NewLine);
            this.textBoxLog.AppendText("Stopwatch frequency (Hz): " + Stopwatch.Frequency + Environment.NewLine);
            this.textBoxLog.AppendText(Environment.NewLine + "===Synchronized parallel task===" + Environment.NewLine);

            using (var barrier = new Barrier(numProcessors))
            {
                Parallel.Invoke(Enumerable.Range(0, numProcessors).Select(i => this.GenerateActionForCore(i, barrier)).ToArray());
            }

            this.RecordMeasurements();
            summarizer.AddSynchronizedMeasurements(this.measurements);

            this.measurements = new TimeMeasurement[numProcessors];
            this.textBoxLog.AppendText(Environment.NewLine + "===Unsynchronized parallel task===" + Environment.NewLine);
            Parallel.Invoke(Enumerable.Range(0, numProcessors).Select(i => this.GenerateActionForCore(i, null)).ToArray());

            this.RecordMeasurements();
            summarizer.AddUnsynchronizedMeasurements(this.measurements);

            this.measurements = new TimeMeasurement[numProcessors];
            this.textBoxLog.AppendText(Environment.NewLine + "===Serial task===" + Environment.NewLine);
            for (int i = 0; i < numProcessors; i++)
            {
                this.GenerateActionForCore(i, null)();
            }

            this.RecordMeasurements();

            TimerTester.timeEndPeriod(1);

            summarizer.Summarize(this.richTextBoxSummary);
        }

        private void RecordMeasurements()
        {
            lock (this.locker)
            {
                this.textBoxLog.AppendText(String.Join(Environment.NewLine, this.measurements.Select(m => m.ToString())) + Environment.NewLine);

                long minStopwatch = this.measurements.Min(m => m.stopwatchTimestamp);
                foreach (var measurement in this.measurements)
                {
                    long diff = measurement.stopwatchTimestamp - minStopwatch;
                    this.textBoxLog.AppendText(String.Format("Core {0} swing: {1} ticks, {2:F10}s; on time: {3}" + Environment.NewLine, measurement.core, diff, (double)diff/Stopwatch.Frequency, TimeSpan.FromSeconds((double)measurement.stopwatchTimestamp/Stopwatch.Frequency)));
                }
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.richTextBoxSummary.Text + Environment.NewLine + "~~~~~~" + Environment.NewLine + this.textBoxLog.Text);
        }
    }
}
