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

        public void AddSerialMeasurements(TimeMeasurement[] measurements)
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
        }
    }
}
