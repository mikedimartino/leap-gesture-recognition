using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace LGR_Converters
{
	public class FeatureWeightKeyToLabelConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string featureKey = value.ToString();
			string label = "";
			if (featureKey.Contains("TYPE_")) // Finger specific. The "TYPE_" comes from Leap.Finger.FingerType
			{
				label = featureKey.StartsWith("Left") ? "Left" : "Right";
				switch (featureKey.Split('_')[1])
				{
					case "PINKY": label += "Pinky"; break;
					case "RING": label += "Ring"; break;
					case "MIDDLE": label += "Middle"; break;
					case "INDEX": label += "Index"; break;
					case "THUMB": label += "Thumb"; break;
				}
				label += (featureKey.Contains("TipPosition")) ? "TipPosition" : "Extended";
			}
			else
			{
				label = featureKey;
			}

			// Add space between lower and upper case letters
			StringBuilder sb = new StringBuilder();
			foreach (char c in label)
			{
				if (char.IsUpper(c))
				{
					sb.Append(' ');
				}
				sb.Append(c);
			}

			return sb.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}
}
