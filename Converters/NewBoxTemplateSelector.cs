using System.Windows;
using System.Windows.Controls;
using Timeline.Model;

namespace Timeline.Converters
{
    public class NewBoxTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BoxTemplate { get; set; }
        public DataTemplate PauseBoxTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Box)
                return BoxTemplate;

            if (item is PauseBox)
                return PauseBoxTemplate;

            return null;
        }
    }
}