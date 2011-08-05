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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MultiCoreHighPerformanceTimer
{
    internal class Summarizer
    {
        private bool successful = true;
        private bool serialTasksAlwaysAscending = true;

        private readonly List<double> synchronizedDifferences = new List<double>();
        private readonly List<double> unsynchronizedDifferences = new List<double>();
        private readonly List<double> variationCoefficients = new List<double>();

        public void Summarize(System.Windows.Forms.RichTextBox richTextBox)
        {
            richTextBox.Clear();

            if (this.synchronizedDifferences.Any())
            {
                const double syncAllowableAverage = 0.0003;
                double syncAverage = this.synchronizedDifferences.Average();
                this.successful = this.successful && syncAverage < syncAllowableAverage;
                richTextBox.AppendText("Synchronized average: ");
                richTextBox.SelectionColor = syncAverage < syncAllowableAverage ? Color.Green : Color.Red;
                richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
                richTextBox.AppendText(syncAverage.ToString("F10") + 's' + Environment.NewLine);

                const double syncAllowableMax = 0.0003;
                double syncMax = this.synchronizedDifferences.Max();
                richTextBox.AppendText("Synchronized max: ");
                richTextBox.SelectionColor = syncMax < syncAllowableMax ? Color.Green : Color.Goldenrod;
                richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
                richTextBox.AppendText(syncMax.ToString("F10") + 's' + Environment.NewLine);
            }
            else
            {
                richTextBox.AppendText("Synchronized average: N/A" + Environment.NewLine);
                richTextBox.AppendText("Synchronized max: N/A" + Environment.NewLine);
            }

            if (this.unsynchronizedDifferences.Any())
            {
                const double unsyncAllowableAverage = 0.001;
                double unsyncAverage = this.unsynchronizedDifferences.Average();
                this.successful = this.successful && unsyncAverage < unsyncAllowableAverage;
                richTextBox.AppendText("Unsynchronized average: ");
                richTextBox.SelectionColor = unsyncAverage < unsyncAllowableAverage ? Color.Green : Color.Red;
                richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
                richTextBox.AppendText(unsyncAverage.ToString("F10") + 's' + Environment.NewLine);

                const double unsyncAllowableMax = 0.001;
                double unsyncMax = this.unsynchronizedDifferences.Max();
                richTextBox.AppendText("Unsynchronized max: ");
                richTextBox.SelectionColor = unsyncMax < unsyncAllowableMax ? Color.Green : Color.Goldenrod;
                richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
                richTextBox.AppendText(unsyncMax.ToString("F10") + 's' + Environment.NewLine);
            }
            else
            {
                richTextBox.AppendText("Unsynchronized average: N/A" + Environment.NewLine);
                richTextBox.AppendText("Unsynchronized max: N/A" + Environment.NewLine);
            }

            if (this.variationCoefficients.Any())
            {
                // 0.15 semi-arbitrarily chosen to be reasonable boundary, based on repeated trials and human observation
                // But bumped it up to 0.25 to avoid false positives
                const double vcAllowableAverage = 0.25;
                double vcAverage = this.variationCoefficients.Average();
                this.successful = this.successful && vcAverage < vcAllowableAverage;
                richTextBox.AppendText("Variation coefficient average: ");
                richTextBox.SelectionColor = vcAverage < vcAllowableAverage ? Color.Green : Color.Red;
                richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
                richTextBox.AppendText(vcAverage.ToString("F3") + Environment.NewLine);

                const double vcAllowableMax = 0.5;
                double vcMax = this.variationCoefficients.Max();
                richTextBox.AppendText("Variation coefficient max: ");
                richTextBox.SelectionColor = vcMax < vcAllowableMax ? Color.Green : Color.Goldenrod;
                richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
                richTextBox.AppendText(vcMax.ToString("F3") + Environment.NewLine);
            }
            else
            {
                richTextBox.AppendText("Variation coefficient average: N/A" + Environment.NewLine);
                richTextBox.AppendText("Variation coefficient max: N/A" + Environment.NewLine);
            }

            richTextBox.AppendText("Counter always increasing in serial task: ");
            richTextBox.SelectionColor = this.serialTasksAlwaysAscending ? Color.Green : Color.Red;
            richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
            richTextBox.AppendText((this.serialTasksAlwaysAscending ? "YES" : "NO") + Environment.NewLine);

            this.successful = this.successful && this.serialTasksAlwaysAscending;

            // insert ok/bad at beginning
            richTextBox.Select(0, 0);
            if (this.successful)
            {
                richTextBox.SelectionColor = Color.Green;
                richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
                richTextBox.SelectedText= "OK" + Environment.NewLine;
            }
            else
            {
                richTextBox.SelectionColor = Color.Red;
                richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
                richTextBox.SelectedText = "BAD" + Environment.NewLine;
            }

            // insert message before ok/bad
            richTextBox.Select(0, 0);
            richTextBox.SelectedText = "Overall status: ";

            if (!this.successful)
            {
                string message = "The program has detected some poor timing results on your computer. " +
                                 "It is normal for this to happen once in a while." + Environment.NewLine + Environment.NewLine +
                                 "However, if you run the tests a few more times and are consistently receiving poor timing results, there may be applications running on your PC that consume too many resources and cause false positives.  " +
                                 "First, try stopping other programs that are running.  " +
                                 "If the poor timing results still persist, you may have an issue with your computer hardware or your BIOS.  " +
                                 "Click the 'Copy results to clipboard' button to your right, and paste everything in an email to software@lafayetteinstrument.com with subject 'Timing results' for more help determining how to fix the issue.";

                System.Windows.Forms.MessageBox.Show(message, "Poor timing results");
            }
        }

        public void AddSynchronizedMeasurements(IEnumerable<TimeMeasurement> measurements)
        {
            long minStopwatch = measurements.Min(m => m.stopwatchTimestamp);

            this.synchronizedDifferences.AddRange(measurements.Where(m => m.stopwatchTimestamp > minStopwatch).Select(m => (m.stopwatchTimestamp - minStopwatch)/(double)Stopwatch.Frequency));
        }

        public void AddUnsynchronizedMeasurements(IEnumerable<TimeMeasurement> measurements)
        {
            long minStopwatch = measurements.Min(m => m.stopwatchTimestamp);

            this.unsynchronizedDifferences.AddRange(measurements.Where(m => m.stopwatchTimestamp > minStopwatch).Select(m => (m.stopwatchTimestamp - minStopwatch)/(double)Stopwatch.Frequency));
        }

        public void AddSerialMeasurements(IEnumerable<TimeMeasurement> measurements)
        {
            var differences = new List<long>();

            var prevStopwatch = measurements.First().stopwatchTimestamp;
            foreach (var curStopwatch in measurements.Skip(1).Select(m => m.stopwatchTimestamp))
            {
                differences.Add(curStopwatch - prevStopwatch);
                prevStopwatch = curStopwatch;
            }

            if (differences.Count > 1)
            {
                var average = differences.Average();
                var standardDeviation = Math.Sqrt(differences.Sum(d => (d - average) * (d - average)) / (differences.Count - 1));
                var cv = standardDeviation / average;

                this.variationCoefficients.Add(cv);
            }

            this.serialTasksAlwaysAscending = this.serialTasksAlwaysAscending && differences.All(d => d > 0);
        }
    }
}
