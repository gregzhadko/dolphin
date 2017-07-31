using System.Collections.Generic;
using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace Timeline.Model
{
    public abstract class BaseBox : ObservableObject
    {
        public const int PointsPerSecond = 256;

        public abstract int Duration { get; set; }
        public abstract Color Color { get; set; }

        public List<string> Points { get; set; }
        public virtual string Title { get; set; }
    }
}