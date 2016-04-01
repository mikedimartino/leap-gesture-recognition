using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LGR
{
	public class DGClass 
	{
		#region Public Properties
		public GestureType GestureType { get { return GestureType.Dynamic; } }
		public List<DGClassSample> Samples { get; set; }
		#endregion

		#region Constructors
		public DGClass() 
		{
			Samples = new List<DGClassSample>();
		}

		public DGClass(ObservableCollection<DGInstanceWrapper> dgInstances)
			: this(dgInstances.Select(dgi => dgi.Instance).ToList()) { }

		public DGClass(List<DGInstance> dgInstances)
		{
			//buildExactSamples(dgInstances);
			buildDtwSamples(dgInstances);
		}
		#endregion


		#region Public Methods
		public float DistanceTo(DGInstance otherInstance)
		{
			float dist = GetDTWDistance(otherInstance);
			//float dist = GetExactDistance(otherInstance);
			return dist;
		}

		// Lerps instance to make its sample collection same size as this class.
		public float GetExactDistance(DGInstance otherInstance)
		{
			float distance = 0;
			var instanceSamples = otherInstance.GetResizedSampleList(Samples.Count);

			for (int i = 0; i < Samples.Count; i++)
			{
				var temp = Samples[i].DistanceTo(instanceSamples[i]);
				distance += temp;
			}

			distance = distance / (float)Samples.Count;

			return distance;
		}

		public float GetDTWDistance(DGInstance otherInstance)
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
					float cost = distances[i,j];
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

			float distance = dtw[Samples.Count - 1, otherInstance.Samples.Count - 1] / path.Count;
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
		public void buildExactSamples(List<DGInstance> dgInstances)
		{
			Samples = new List<DGClassSample>();

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
				Samples.Add(new DGClassSample(lerpedDGInstances.Select(dgi => dgi.Samples[i]).ToList()));
			}
			// At this point Samples contains StaticGestureClasses in each array position.
		}

		public void buildDtwSamples(List<DGInstance> dgInstances)
		{
			Samples = new List<DGClassSample>();

			if (dgInstances == null || dgInstances.Count == 0) return;

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
				Samples.Add(new DGClassSample(mappedInstances.Select(dgi => dgi.Samples[i]).ToList()));
			}
		}

		#endregion
	}
}
