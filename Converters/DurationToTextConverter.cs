using System;
using System.Globalization;
using System.Windows.Data;


//Essentially converts the seconds stated into seconds needed. 
namespace Timeline.Converters
{
    public class DurationToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var duration = (int?) value;

            if (duration != null && duration > 1)
                return $"{duration} seconds";

            return $"{duration} sec";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}