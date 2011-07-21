﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MultiCoreHighPerformanceTimer
{
    internal class Summarizer
    {
        private bool successful = true;

        private readonly List<double> synchronizedDifferences = new List<double>();
        private readonly List<double> unsynchronizedDifferences = new List<double>();

        public void Summarize(System.Windows.Forms.RichTextBox richTextBox)
        {
            richTextBox.Clear();
            richTextBox.AppendText("Overall status: ");

            if (this.successful)
            {
                richTextBox.SelectionColor = Color.Green;
                richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
                richTextBox.AppendText("OK" + Environment.NewLine);
            }
            else
            {
                richTextBox.SelectionColor = Color.Red;
                richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
                richTextBox.AppendText("BAD" + Environment.NewLine);
            }

            double syncAverage = this.synchronizedDifferences.Average();
            richTextBox.AppendText("Synchronized average: ");
            richTextBox.SelectionColor = syncAverage < 0.0001 ? Color.Green : Color.Red;
            richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
            richTextBox.AppendText(syncAverage.ToString("F10") + 's' + Environment.NewLine);

            double syncMax = this.synchronizedDifferences.Max();
            richTextBox.AppendText("Synchronized max: ");
            richTextBox.SelectionColor = syncMax < 0.0001 ? Color.Green : Color.Red;
            richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
            richTextBox.AppendText(syncMax.ToString("F10") + 's' + Environment.NewLine);

            double unsyncAverage = this.unsynchronizedDifferences.Average();
            richTextBox.AppendText("Unsynchronized average: ");
            richTextBox.SelectionColor = unsyncAverage < 0.001 ? Color.Green : Color.Red;
            richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
            richTextBox.AppendText(unsyncAverage.ToString("F10") + 's' + Environment.NewLine);

            double unsyncMax = this.unsynchronizedDifferences.Max();
            richTextBox.AppendText("Unsynchronized max: ");
            richTextBox.SelectionColor = unsyncMax < 0.001 ? Color.Green : Color.Red;
            richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
            richTextBox.AppendText(unsyncMax.ToString("F10") + 's' + Environment.NewLine);
        }

        public void AddSynchronizedMeasurements(TimeMeasurement[] measurements)
        {
            long minStopwatch = measurements.Min(m => m.stopwatchTimestamp);

            this.synchronizedDifferences.AddRange(measurements.Where(m => m.stopwatchTimestamp > minStopwatch).Select(m => (m.stopwatchTimestamp - minStopwatch)/(double)Stopwatch.Frequency));
        }

        public void AddUnsynchronizedMeasurements(TimeMeasurement[] measurements)
        {
            long minStopwatch = measurements.Min(m => m.stopwatchTimestamp);

            this.unsynchronizedDifferences.AddRange(measurements.Where(m => m.stopwatchTimestamp > minStopwatch).Select(m => (m.stopwatchTimestamp - minStopwatch)/(double)Stopwatch.Frequency));
        }
    }
}