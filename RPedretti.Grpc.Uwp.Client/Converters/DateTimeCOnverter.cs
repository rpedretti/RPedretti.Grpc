using System;
using Windows.UI.Xaml.Data;

namespace RPedretti.Grpc.Uwp.Client.Converters
{
    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is DateTime dt ? new DateTimeOffset(dt.ToUniversalTime()) : value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is DateTimeOffset dto ? (DateTime?)dto.DateTime.Date : null;
        }
    }
}
