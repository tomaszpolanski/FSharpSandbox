using Luncher.Api;
using System;
using System.Diagnostics;
using Windows.UI.Xaml.Data;

namespace Luncher.Converters
{
    public class DayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Debug.Assert(value is DateTime);
            return Date.parseData((DateTime)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
