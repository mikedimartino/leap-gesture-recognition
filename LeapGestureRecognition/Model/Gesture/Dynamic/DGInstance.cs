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


		public DGInstance GetDtwMappedInstance(DGInstance target)
		{
			DGInstance mappedInstance;
			GetDtwDistance(target, out mappedInstance);
			return mappedInstance;
		}

		public float GetDtwDistance(DGInstance otherInstance, out DGInstance mappedInstance)
		{
			float[,] distances = new float[Samples.Count, otherInstance.Samples.Count];
			for (int i = 0; i < Samples.Count; i++)
			{
				for (int j = 0; j < otherInstance.Samples.Count; j++)
				{
					distances[i, j] = Samples[i].DistanceTo(otherInstance.Samples[j]);
				}
			}

			float[,] dtw = new float[Samples.Count, otherInstance.Samples.Count];
			for (int i = 0; i < Samples.Count; i++)
			{
				for (int j = 0; j < otherInstance.Samples.Count; j++)
				{
					float cost = distances[i, j];
					var precedingCosts = new List<float>() 
					{ 
						dtw[Math.Max(i-1, 0), j], 
						dtw[i, Math.Max(j-1, 0)], 
						dtw[Math.Max(i-1, 0), Math.Max(j-1, 0)] 
					
					};
					dtw[i, j] = cost + precedingCosts.Min();
				}
			}

			// TODO: Backtrack through and find the path (particularly interested in its length)
			var path = new Stack<Tuple<int, int>>();
			int x = Samples.Count - 1;
			int y = otherInstance.Samples.Count - 1;
			path.Push(new Tuple<int, int>(x, y));
			while (x > 0 || y > 0)
			{
				if (x == 0) y--;
				else if (y == 0) x--;
				else
				{
					var neighboringCosts = new List<float>() 
					{ 
						dtw[x - 1, y - 1],
						dtw[x - 1, y],
						dtw[x, y - 1]
					};
					if (dtw[x - 1, y] == neighboringCosts.Min()) x--;
					else if (dtw[x, y - 1] == neighboringCosts.Min()) y--;
					else { x--; y--; }
				}
				path.Push(new Tuple<int, int>(x, y));
			}

			// x (Item1) is index of this instance; y (Item2) is index of other instance
			var mappedSamples = new List<DGInstanceSample>();
			while (path.Count > 0)
			{
				int index = path.Pop().Item1;
				mappedSamples.Add(Samples[index]);
			}

			mappedInstance = new DGInstance(mappedSamples);

			float distance = dtw[Samples.Count - 1, otherInstance.Samples.Count - 1] / path.Count;
			return distance;
		}
		#endregion 
	}
}
