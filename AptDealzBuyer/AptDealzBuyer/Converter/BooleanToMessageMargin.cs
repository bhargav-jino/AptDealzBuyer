using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Converter
{
    public class BooleanToMessageMargin : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var isUser = (bool)value;
                if (isUser)
                    return new Thickness(20, 20, 0, 0);
                else
                    return new Thickness(0, 20, 20, 0);
            }
            else
            {
                return new Thickness(0, 20, 20, 0);
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Only one way bindings are supported with this converter");
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
