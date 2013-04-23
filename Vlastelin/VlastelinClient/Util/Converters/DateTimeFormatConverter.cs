using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Vlastelin.Common;

namespace VlastelinClient.Util.Converters
{
	/// <summary>
	/// конвертер для получения времени из DateTime
	/// </summary>
	[ValueConversion(typeof(DateTime?), typeof(String))]
	public class DateTimeFormatConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is DateTime?)    
            {
				if (parameter == null || String.IsNullOrWhiteSpace(parameter.ToString()))
				{
					return value.ToString();
				}
				return ((DateTime)value).ToString(parameter.ToString());
            }

            return String.Empty;        
        }

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value != null && !String.IsNullOrEmpty(value.ToString()))
			{
				return DateTime.Parse(value.ToString(), culture);
			}

			return String.Empty; 
		}           
	}
}
