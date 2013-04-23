using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace VlastelinClient.Util.Converters
{
    /// <summary>
    /// конвертор для передачи списка параметров из мультибиндинга
    /// </summary>
    public class MultiValuesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return values != null ? new List<object>(values) : null; 
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
