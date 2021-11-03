using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Converter
{
    public class BooleanToPancakeRadius : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var isUser = (bool)value;
                if (isUser)
                    return new CornerRadius(10, 0, 10, 10);
                else
                    return new CornerRadius(0, 10, 10, 10);
            }
            else
            {
                return new CornerRadius(0, 10, 10, 10);
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
