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
		public List<StaticGestureInstance> GetResizedSampleList(int size)
		{
			var samples = new List<StaticGestureInstance>();

			samples.Add(Samples[0]);
			for (int i = 1; i < size; i++)
			{
				float index = (i * Samples.Count) / (float)size; // Actual index (not an int)
				float lerpAmount = index - ((int)index); // Get fractional amount of index

				if (index < 1)
				{
					samples.Add(Samples[0].Lerp(Samples[1], lerpAmount));
				}
				else
				{
					samples.Add(Samples[(int)index - 1].Lerp(Samples[(int)index], lerpAmount));
				}
			}

			return samples;
		}
		#endregion
	}
}
