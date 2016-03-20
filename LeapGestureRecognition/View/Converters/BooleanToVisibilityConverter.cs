using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace LGR_Converters
{
	public class BooleanToVisibilityConverter : IValueConverter
	{
		private object GetVisibility(object value)
		{
			if (!(value is bool)) return Visibility.Collapsed;
			bool objValue = (bool)value;
			if (objValue)
			{
				return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
		{
			return GetVisibility(value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
		{
			throw new NotImplementedException();
		}
	}
}
