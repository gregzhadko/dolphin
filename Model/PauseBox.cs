using System.Collections.Generic;
using System.Windows.Media;

namespace Timeline.Model
{
    public sealed class PauseBox : BaseBox
    {
        private Color _color = Colors.White;

        private int _duration;

        //PauseBox has a default color of White (for differentiation) 
        public PauseBox()
        {
            Title = "Pause";
        }

        public override Color Color
        {
            get => _color;
            set => Set(ref _color, value);
        }

        public override int Duration
        {
            get => _duration;
            set => Set(ref _duration, value);
        }

        public int Bpm => 833;

        //This method will generate the number of 833 values needed in between two sinus beats
        //Takes the total amount of entries (833 flatline values) needed (256*number of seconds) and then subtracts the numberBefore and the numberAfter
        //numberBefore = The last peak value from the sinus before the pause
        //numberAfter = The first peak from the sinus after the pause
        public void GeneratePoints(Box boxBefore, Box boxAfter)
        {
            Points = new List<string>();

            var numberBefore = boxBefore != null ? (boxBefore.Points.Count - boxBefore.MaxValueLastIndex + 1) : 0;
            var numberAfter = boxAfter != null ? boxAfter.MaxValueFirstIndex : 0;

            var totalNumber = PointsPerSecond * Duration - (numberAfter + numberBefore);
            if (totalNumber <= 0)
                return;

            for (var i = 0; i < totalNumber; i++)
            {
                //flatline value
                Points.Add("833");
            }
        }
    }
}