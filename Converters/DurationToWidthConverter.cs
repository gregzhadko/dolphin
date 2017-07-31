using System;
using System.Globalization;
using System.Windows.Data;


//converter for duration of needed box to width of box in timeline
namespace Timeline.Converters
{
    public class DurationToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var duration = (int) value;
            if (duration > 10)
            {
                duration = 10;
            }
            return duration*35;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}