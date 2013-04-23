using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Data;
using System.Windows.Controls;
using System.Windows;

namespace VlastelinClient.Util.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class RowColumnToCellConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            DataRowView row = values[0] as DataRowView;
            DataGridColumn column = values[1] as DataGridColumn;
            return row != null && column != null && row.Row.RowState != DataRowState.Detached
                ? row[column.SortMemberPath]
                : DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
