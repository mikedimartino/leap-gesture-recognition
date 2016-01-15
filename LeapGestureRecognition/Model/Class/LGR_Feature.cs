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
	[DataContract]
	public class LGR_Feature
	{
		public LGR_Feature(FeatureName name, object value)
		{
			Name = name;
			Value = value;
		}

		[DataMember]
		public FeatureName Name { get; set; }
		[DataMember]
		public object Value { get; set; }
		// Weight?
	}

}
