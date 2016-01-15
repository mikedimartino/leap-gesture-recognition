using Leap;
using LeapGestureRecognition.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Media;

namespace LeapGestureRecognition.Model
{
	public class LGR_Feature
	{
		public LGR_Feature(FeatureType type, object value)
		{
			Type = type;
			Value = value;
		}

		public FeatureType Type { get; set; }
		public object Value { get; set; }
		// Weight?
	}


}
