using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LeapGestureRecognition
{
	[DataContract]
	public class GestureDistance
	{
		public GestureDistance() { }
		public GestureDistance(string gestureName, float distance)
		{
			Name = gestureName;
			Distance = distance;
		}

		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public float Distance { get; set; }
	}
}
