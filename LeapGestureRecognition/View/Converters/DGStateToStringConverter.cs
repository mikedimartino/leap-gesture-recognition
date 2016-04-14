using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace LeapGestureRecognition.Converters
{
	public class DGStateToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
		{
			string state = value.ToString();
			if (string.IsNullOrWhiteSpace(state)) return "";

			StringBuilder sb = new StringBuilder();
			sb.Append(state[0]);
			for (int i = 1; i < state.Length; i++)
			{
				if (Char.IsLower(state[i - 1]) && Char.IsUpper(state[i]))
				{
					sb.Append(" ");
				}
				sb.Append(state[i]);
			}

			return sb.ToString();
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
		{
			throw new NotImplementedException();
		}
	}
}
