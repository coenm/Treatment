namespace Treatment.UI.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isVisible = value != null && (bool)value;

            if (isVisible)
                return Visibility.Visible;

            return parameter is string defaultInvisibility
                       ? (Visibility)Enum.Parse(typeof(Visibility), defaultInvisibility)
                       : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}