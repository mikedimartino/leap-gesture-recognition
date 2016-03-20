using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LGR
{
	[DataContract]
	public class DynamicGestureInstance
	{
		private int numSamples = 10;

		public DynamicGestureInstance() { }
		public DynamicGestureInstance(List<DynamicGestureInstanceSample> samples)
		{
			// Need to think about number of samples to take.
			// Use first and last sample, and then divide evenly between the rest.
			Samples = new List<DynamicGestureInstanceSample>();
			int increment = samples.Count / numSamples;
			for (int i = 0; i < numSamples - 1; i += increment)
			{
				Samples.Add(samples[i]);
			}
			// Make last sample be the final instance
			Samples.Add(samples[samples.Count - 1]);
		}

		#region Public Properties
		[DataMember]
		public List<DynamicGestureInstanceSample> Samples { get; set; }

		public GestureType GestureType { get { return GestureType.Dynamic; } }
		#endregion

		#region Public Methods
		public float DistanceTo(DynamicGestureInstance otherInstance)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
