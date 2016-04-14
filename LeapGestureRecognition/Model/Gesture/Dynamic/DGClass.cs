using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LeapGestureRecognition
{
	[DataContract]
	public class DGClass 
	{
		#region Public Properties
		//[DataMember]
		//public List<DGClassSample> Samples { get; set; }
		[DataMember]
		public List<DGClassSample> DtwSamples { get; set; }
		[DataMember]
		public List<DGClassSample> LerpedSamples { get; set; }
		[DataMember]
		public bool IncludesLeftHand { get; set; }
		[DataMember]
		public bool IncludesRightHand { get; set; }

		[DataMember]
		public float MeanSampleCount { get; set; }
		[DataMember]
		public float MeanLeftHandPathLength { get; set; }
		[DataMember]
		public float MeanRightHandPathLength { get; set; }

		[DataMember]
		public float StdDevSampleCount { get; set; }
		[DataMember]
		public float StdDevLeftHandPathLength { get; set; }
		[DataMember]
		public float StdDevRightHandPathLength { get; set; }
		#endregion

		#region Constructors
		public DGClass() 
		{
			//Samples = new List<DGClassSample>();
			LerpedSamples = new List<DGClassSample>();
			DtwSamples = new List<DGClassSample>();
		}

		public DGClass(ObservableCollection<DGInstanceWrapper> dgInstances)
			: this(dgInstances.Select(dgi => dgi.Instance).ToList()) { }

		public DGClass(List<DGInstance> dgInstances)
		{
			LerpedSamples = GetLerpedSamples(dgInstances);
			DtwSamples = GetDtwSamples(dgInstances);

			int includeLeftHandCount = dgInstances.Where(dgi => dgi.IncludesLeftHand).Count();
			int includeRightHandCount = dgInstances.Where(dgi => dgi.IncludesRightHand).Count();
			IncludesLeftHand = includeLeftHandCount > dgInstances.Count / 2;
			IncludesRightHand = includeRightHandCount > dgInstances.Count / 2;

			computeMeanAndStdDevValues(dgInstances);
		}
		#endregion


		#region Public Methods
		public float DistanceTo(DGInstance otherInstance)
		{
			float dtwDist = GetDTWDistance(otherInstance);
			float lerpedDist = GetLerpedDistance(otherInstance);

			float dist = (dtwDist + lerpedDist) / 2.0f;

			// Factor in path length (with weight 1/5):
			int newFeatureCount = 0;
			float additionalDist = 0;
			if (IncludesLeftHand)
			{
				additionalDist += Math.Abs(otherInstance.LeftHandPathLength - MeanLeftHandPathLength) / StdDevLeftHandPathLength;
				newFeatureCount++;
			}
			if (IncludesRightHand)
			{
				additionalDist += Math.Abs(otherInstance.RightHandPathLength - MeanRightHandPathLength) / StdDevRightHandPathLength;
				newFeatureCount++;
			}
			additionalDist /= (float)newFeatureCount;

			float additionalDistWeight = 0;// 0.1f;
			dist = ((1 - additionalDistWeight) * dist) + (additionalDistWeight * additionalDist);

			//return dist;
			return lerpedDist;
			//return dtwDist;
		}

		// Lerps instance to make its sample collection same size as this class.
		public float GetLerpedDistance(DGInstance otherInstance)
		{
			float distance = 0;
			var instanceSamples = otherInstance.GetResizedSampleList(LerpedSamples.Count);

			for (int i = 0; i < LerpedSamples.Count; i++)
			{
				var temp = LerpedSamples[i].DistanceTo(instanceSamples[i]);
				distance += temp;
			}

			distance = distance / (float)LerpedSamples.Count;

			return distance;
		}

		public float GetDTWDistance(DGInstance otherInstance, int windowSize = 3)
		{
			if (DtwSamples.Count == 0 || otherInstance.Samples.Count == 0) return float.PositiveInfinity;

			float[,] distances = new float[DtwSamples.Count, otherInstance.Samples.Count];
			for (int i = 0; i < DtwSamples.Count; i++)
			{
				for (int j = 0; j < otherInstance.Samples.Count; j++)
				{
					distances[i, j] = DtwSamples[i].DistanceTo(otherInstance.Samples[j]);
				}
			}

			int window = Math.Max(windowSize, Math.Abs(DtwSamples.Count - otherInstance.Samples.Count));

			float[,] dtw = new float[DtwSamples.Count, otherInstance.Samples.Count];
			for (int i = 0; i < DtwSamples.Count; i++)
			{
				for (int j = 0; j < otherInstance.Samples.Count; j++)
				{
					dtw[i, j] = float.PositiveInfinity;
				}
			}
			dtw[0, 0] = 0;

			for (int i = 1; i < DtwSamples.Count; i++)
			{
				//for (int j = 0; j < otherInstance.Samples.Count; j++)
				for (int j = Math.Max(1, i - window); j < Math.Min(otherInstance.Samples.Count, i + window); j++)
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
			int x = DtwSamples.Count - 1;
			int y = otherInstance.Samples.Count - 1;
			path.Push(new Tuple<int, int>(x, y));
			while (x > 0 || y > 0)
			{
				if (x == 0) { y--; }
				else if (y == 0) { x--; }
				else
				{
					var neighboringCosts = new List<float>() 
					{ 
						dtw[Math.Max(0, x - 1), Math.Max(0, y - 1)],
						dtw[Math.Max(0, x - 1), y],
						dtw[x, Math.Max(0, y - 1)]
					};
					if (dtw[Math.Max(0, x - 1), y] == neighboringCosts.Min()) x--;
					else if (dtw[x, Math.Max(0, y - 1)] == neighboringCosts.Min()) y--;
					else { x--; y--; }
				}
				path.Push(new Tuple<int, int>(x, y));
			}

			float distance = dtw[DtwSamples.Count - 1, otherInstance.Samples.Count - 1] / (float)path.Count;

			return distance;
		}
		#endregion

		#region Private Methods
		// Gets the average number of samples
		private int getAverageSampleSize(List<DGInstance> dgInstances)
		{
			if (dgInstances.Count == 0) return 0;
			int samples = 0;
			foreach (var instance in dgInstances) samples += instance.Samples.Count;
			return samples / dgInstances.Count;
		}

		// Finds the average number of samples for all dg instances.
		// Lerps all DG instances to be this length, and creates a class for each index.
		public List<DGClassSample> GetLerpedSamples(List<DGInstance> dgInstances)
		{
			var samples = new List<DGClassSample>();

			int classSampleSize = getAverageSampleSize(dgInstances);
			var lerpedDGInstances = new List<DGInstance>(); // lerps the static gesture instances contained within

			foreach (var instance in dgInstances)
			{
				var resizedInstance = new DGInstance(instance.GetResizedSampleList(classSampleSize));
				lerpedDGInstances.Add(resizedInstance);
			}

			// At this point, lerpedDGInstances() are all the same length.
			for (int i = 0; i < classSampleSize; i++)
			{
				samples.Add(new DGClassSample(lerpedDGInstances.Select(dgi => dgi.Samples[i]).ToList()));
			}

			return samples;
		}

		public List<DGClassSample> GetDtwSamples(List<DGInstance> dgInstances)
		{
			var samples = new List<DGClassSample>();

			if (dgInstances != null && dgInstances.Count > 0)
			{
				int maxSamples = dgInstances.Max(dgi => dgi.Samples.Count);
				var longestInstance = dgInstances.Where(dgi => dgi.Samples.Count == maxSamples).First();
				var mappedInstances = new List<DGInstance>();
				foreach (var instance in dgInstances)
				{
					// All of these instances should have the same # of samples
					mappedInstances.Add(instance.GetDtwMappedInstance(longestInstance));
				}
				for (int i = 0; i < longestInstance.Samples.Count; i++)
				{
					samples.Add(new DGClassSample(mappedInstances.Select(dgi => dgi.Samples[i]).ToList()));
				}
			}

			return samples;
		}

		private void computeMeanAndStdDevValues(List<DGInstance> dgInstances)
		{
			// Mean
			MeanSampleCount = 0;
			MeanLeftHandPathLength = 0;
			MeanRightHandPathLength = 0;

			foreach (var instance in dgInstances)
			{
				MeanSampleCount += instance.Samples.Count;
				MeanLeftHandPathLength += instance.LeftHandPathLength;
				MeanRightHandPathLength += instance.RightHandPathLength;
			}

			MeanSampleCount /= (float)dgInstances.Count;
			MeanLeftHandPathLength /= (float)dgInstances.Count;
			MeanRightHandPathLength /= (float)dgInstances.Count;

			// Standard Deviation
			StdDevSampleCount = 0;
			StdDevLeftHandPathLength = 0;
			StdDevRightHandPathLength = 0;

			foreach (var instance in dgInstances)
			{
				StdDevSampleCount += Math.Abs(instance.Samples.Count - MeanSampleCount);
				StdDevLeftHandPathLength += Math.Abs(instance.LeftHandPathLength - MeanLeftHandPathLength);
				StdDevRightHandPathLength += Math.Abs(instance.RightHandPathLength - MeanRightHandPathLength);
			}

			StdDevSampleCount /= (float)dgInstances.Count;
			StdDevLeftHandPathLength /= (float)dgInstances.Count;
			StdDevRightHandPathLength /= (float)dgInstances.Count;
		}
		#endregion
	}
}
