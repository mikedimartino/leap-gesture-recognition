using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LeapGestureRecognition
{
	[DataContract]
	public class FeatureWeight
	{
		public FeatureWeight() { }
		public FeatureWeight(string name, int weight)
		{
			Name = name;
			Weight = weight;
		}

		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public int Weight { get; set; }
	}
}
