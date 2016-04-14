using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LeapGestureRecognition
{
	[DataContract]
	public class DGInstance
	{
		[DataMember]
		public List<DGInstanceSample> Samples { get; set; }
		[DataMember]
		public bool IncludesLeftHand { get; set; }
		[DataMember]
		public bool IncludesRightHand { get; set; }
		[DataMember]
		public float RightHandPathLength { get; set; }
		[DataMember]
		public float LeftHandPathLength { get; set; }

		public DGInstance() { }
		public DGInstance(List<DGInstanceSample> samples)
		{
			Samples = samples; // For now just store all samples
			IncludesLeftHand = samples.Any(s => s.LeftHand != null);
			IncludesRightHand = samples.Any(s => s.RightHand != null);
			computePathLengths(samples);
		}


		#region Public Methods
		public List<DGInstanceSample> GetResizedSampleList(int size)
		{
			var samples = new List<DGInstanceSample>();

			if (Samples.Count > 0)
			{
				samples.Add(Samples[0]);
				for (int i = 1; i < size; i++)
				{
					float index = (i * (Samples.Count - 1)) / (float)(size - 1); // Actual index (not an int)
					float lerpAmount = index - ((int)index); // Get fractional amount of index

					if (index < 1)
					{
						samples.Add(Samples[0].Lerp(Samples[0], lerpAmount));
					}
					else
					{
						samples.Add(Samples[(int)index - 1].Lerp(Samples[(int)index], lerpAmount));
					}
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
			mappedInstance = null;

			if (Samples.Count == 0 || otherInstance.Samples.Count == 0) return float.PositiveInfinity;

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

		#region Private Methods
		private void computePathLengths(List<DGInstanceSample> samples)
		{
			LeftHandPathLength = 0;
			RightHandPathLength = 0;

			if(samples.Count == 0 || samples.Count == 1) return;

			for (int i = 1; i < samples.Count; i++)
			{
				if (samples[i - 1].HandConfiguration != samples[i].HandConfiguration) continue;

				if (samples[i].LeftHand != null)
				{
					LeftHandPathLength += samples[i - 1].LeftHand.PalmPosition.DistanceTo(samples[i].LeftHand.PalmPosition);
				}
				if (samples[i].RightHand != null)
				{
					RightHandPathLength += samples[i - 1].RightHand.PalmPosition.DistanceTo(samples[i].RightHand.PalmPosition);
				}
			}
		}
		#endregion
	}
}
