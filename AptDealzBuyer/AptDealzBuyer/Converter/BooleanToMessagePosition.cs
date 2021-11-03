using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AptDealzBuyer.Converter
{
    public class BooleanToMessagePosition : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var isUser = (bool)value;
                if (isUser)
                    return LayoutOptions.EndAndExpand;
                else
                    return LayoutOptions.StartAndExpand;
            }
            else
            {
                return LayoutOptions.StartAndExpand;
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
