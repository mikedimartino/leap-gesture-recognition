using Leap;
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
		private int numSamples = 5;

		[DataMember]
		public List<StaticGestureInstance> Samples { get; set; }


		public DynamicGestureInstance() { }
		public DynamicGestureInstance(List<StaticGestureInstance> samples)
		{
			//// Need to think about number of samples to take.
			//// Use first and last sample, and then divide evenly between the rest.
			//Samples = new List<StaticGestureInstance>();
			//int increment = samples.Count / numSamples;
			//for (int i = 0; i < numSamples - 1; i += increment)
			//{
			//	Samples.Add(samples[i]);
			//}
			//// Make last sample be the final instance
			//Samples.Add(samples[samples.Count - 1]);

			Samples = samples; // For now just store all samples
		}

		public DynamicGestureInstance(List<Frame> frames)
		{
			// Need to think about number of samples to take.
			// Use first and last sample, and then divide evenly between the rest.
			Samples = new List<StaticGestureInstance>();
			int increment = frames.Count / numSamples;
			for (int i = 0; i < numSamples - 1; i++)
			{
				Samples.Add(new StaticGestureInstance(frames[i * increment]));
			}
			// Make last sample be the final instance
			Samples.Add(new StaticGestureInstance(frames[frames.Count - 1]));
		}
		

		#region Public Methods
		public float DistanceTo(DynamicGestureInstance otherInstance)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
