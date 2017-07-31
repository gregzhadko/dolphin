using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace Timeline.Model
{
    public class Box : BaseBox
    {
        //each BPM corresponds to a certain color
        private static readonly List<Tuple<int, Color>> BoxColors = new List<Tuple<int, Color>>
        {
            new Tuple<int, Color>(20, Colors.LightCyan),
            new Tuple<int, Color>(30, Colors.Aquamarine),
            new Tuple<int, Color>(40, Colors.Cyan),
            new Tuple<int, Color>(50, Colors.CornflowerBlue),
            new Tuple<int, Color>(60, Colors.Azure),
            new Tuple<int, Color>(70, Colors.DodgerBlue),
            new Tuple<int, Color>(80, Colors.DarkTurquoise),
            new Tuple<int, Color>(90, Colors.Blue),
            new Tuple<int, Color>(100, Colors.CadetBlue),
            new Tuple<int, Color>(110, Colors.GreenYellow),
            new Tuple<int, Color>(120, Colors.LawnGreen),
            new Tuple<int, Color>(130, Colors.LightSeaGreen),
            new Tuple<int, Color>(140, Colors.LimeGreen),
            new Tuple<int, Color>(150, Colors.Green),
            new Tuple<int, Color>(160, Colors.MediumSeaGreen),
            new Tuple<int, Color>(170, Colors.ForestGreen),
            new Tuple<int, Color>(180, Colors.DarkGreen),
            new Tuple<int, Color>(190, Colors.DarkOliveGreen),
            new Tuple<int, Color>(200, Colors.Purple),
            new Tuple<int, Color>(210, Colors.DarkRed),
            new Tuple<int, Color>(220, Colors.IndianRed),
            new Tuple<int, Color>(230, Colors.MediumVioletRed),
            new Tuple<int, Color>(240, Colors.OrangeRed),
            new Tuple<int, Color>(250, Colors.Red)
        };

        private double _beatFrequency;

        private int _bpm;

        private Color _color;

        private int _duration;

        static Box()
        {
            var list = new List<int>();
            for (var i = 10; i < 260; i += 10)
            {
                list.Add(i);
            }

            PossibleValues = new ObservableCollection<int>(list);
        }

        public Box(string title, string folderName)
        {
            Title = title;
            FolderName = folderName;
        }

        /// <summary>
        ///     List of possible BPMs
        /// </summary>
        public static ObservableCollection<int> PossibleValues { get; set; }

        /// <summary>
        ///     Selected BPM value
        /// </summary>
        public int Bpm
        {
            get => _bpm;
            set
            {
                Set(ref _bpm, value);
                BeatFrequency = 60.0 / _bpm;
                UpdateColor();
            }
        }

        /// <summary>
        ///     Duration of the box in seconds
        /// </summary>
        public override int Duration
        {
            get => _duration;
            set
            {
                Set(ref _duration, value);
                UpdateBeatsNumberPerDuration();
            }
        }

        /// <summary>
        ///     Current color of the box
        /// </summary>
        public override Color Color
        {
            get => _color;
            set => Set(ref _color, value);
        }

        public string FolderName { get; set; }

        /// <summary>
        ///     Index of last max value in the points list
        /// </summary>
        public int MaxValueLastIndex { get; private set; }

        /// <summary>
        ///     Index of first max value in the points list
        /// </summary>
        public int MaxValueFirstIndex { get; private set; }

        /// <summary>
        ///     Max value from the points list
        /// </summary>
        public int MaxValue { get; private set; }

        /// <summary>
        ///     Show the frequency for a beat.
        /// </summary>
        public double BeatFrequency
        {
            get => _beatFrequency;
            private set
            {
                _beatFrequency = value;
                UpdateBeatsNumberPerDuration();
            }
        }

        public int BeatsNumberPerDuration { get; private set; }

        public sealed override string Title { get; set; }

        /// <summary>
        ///     Select color from the colors list according to selected BPM
        /// </summary>
        private void UpdateColor()
        {
            Color = BoxColors.First(t => t.Item1 == Bpm).Item2;
        }

        //grab entries from resource file according to BPM
        //Generate the entries needed for the export
        public void GeneratePoints()
        {
            var fileName = Directory.GetCurrentDirectory() + "\\resources\\" + FolderName + "\\" + Bpm + ".txt";
            if (!File.Exists(fileName))
            {
                Points = new List<string>();
            }

            Points = new List<string>();

            var lines = File.ReadAllLines(fileName).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();

            for (var i = 0; i < BeatsNumberPerDuration; i++)
            {
                Points.AddRange(lines);
            }
        }

        /// <summary>
        ///     Calculate max point value, last index of max point value, first index of max point values.
        ///     It is necessary for pause boxes points generation
        /// </summary>
        public void CalculateMaxValues()
        {
            var points = Points.Select(p => Convert.ToInt32(p)).ToList();
            MaxValue = points.Max();
            MaxValueFirstIndex = points.FindIndex(i => i == MaxValue);
            MaxValueLastIndex = points.FindLastIndex(i => i == MaxValue);
        }


        /// <summary>
        ///     Calculate Beats number per set duration
        /// </summary>
        private void UpdateBeatsNumberPerDuration()
        {
            BeatsNumberPerDuration = (int) Math.Ceiling(Duration / BeatFrequency);
        }
    }
}