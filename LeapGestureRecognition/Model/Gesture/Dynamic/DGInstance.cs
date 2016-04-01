using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LGR
{
	[DataContract]
	public class DGInstance
	{
		private int numSamples = 5;

		[DataMember]
		public List<DGInstanceSample> Samples { get; set; }
		[DataMember]
		public Vec3 RightStartPos { get; set; }
		[DataMember]
		public Vec3 LeftStartPos { get; set; }


		public DGInstance() { }
		public DGInstance(List<DGInstanceSample> samples)
		{
			Samples = samples; // For now just store all samples
		}

		public DGInstance(List<Frame> frames)
		{
			// Need to think about number of samples to take.
			// Use first and last sample, and then divide evenly between the rest.
			Samples = new List<DGInstanceSample>();
			int increment = frames.Count / numSamples;
			for (int i = 0; i < numSamples - 1; i++)
			{
				Samples.Add(new DGInstanceSample(frames[i * increment]));
			}
			// Make last sample be the final instance
			Samples.Add(new DGInstanceSample(frames[frames.Count - 1]));

			if (Samples.Count > 0)
			{
				var firstSample = Samples.First();
				if (firstSample.LeftHand != null)
				{
					LeftStartPos = firstSample.LeftHand.PalmPosition;
				}
				if (firstSample.RightHand != null)
				{
					RightStartPos = firstSample.RightHand.PalmPosition;
				}
			}
		}


		#region Public Methods
		public List<DGInstanceSample> GetResizedSampleList(int size)
		{
			var samples = new List<DGInstanceSample>();

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
