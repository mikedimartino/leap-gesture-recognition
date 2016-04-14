using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace LeapGestureRecognition.Converters
{
	public class GestureDistanceToBackgroundConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
		{
			float threshold = (parameter.ToString() == "Static") ? Constants.StaticRecognitionDistance : Constants.DynamicRecognitionDistance;

			float distance = (float)value;
			if (distance < threshold) return Constants.RecognizedRowBackgroundColor;
			else return Brushes.Transparent;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
		{
			throw new NotImplementedException();
		}
	}
}
