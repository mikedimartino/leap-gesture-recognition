using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LGR
{
	public class DynamicGestureClass 
	{
		#region Public Properties
		public GestureType GestureType { get { return GestureType.Dynamic; } }
		public List<StaticGestureClass> Samples { get; set; }
		#endregion

		#region Constructors
		public DynamicGestureClass() 
		{
			Samples = new List<StaticGestureClass>();
		}

		public DynamicGestureClass(ObservableCollection<DynamicGestureInstanceWrapper> dgInstances)
			: this(dgInstances.Select(dgi => dgi.Instance).ToList()) { }

		public DynamicGestureClass(List<DynamicGestureInstance> dgInstances)
		{
			Samples = new List<StaticGestureClass>();

			int classSampleSize = getAverageSampleSize(dgInstances);
			var lerpedDGInstances = new List<DynamicGestureInstance>(); // lerps the static gesture instances contained within

			foreach (var instance in dgInstances)
			{
				var resizedInstance = new DynamicGestureInstance(instance.GetResizedSampleList(classSampleSize));
				lerpedDGInstances.Add(resizedInstance);
			}

			// At this point, lerpedDGInstances() are all the same length.
			for (int i = 0; i < classSampleSize; i++)
			{
				Samples.Add(new StaticGestureClass(lerpedDGInstances.Select(dgi => dgi.Samples[i]).ToList()));
			}
			// At this point Samples contains StaticGestureClasses in each array position.
		}
		#endregion


		#region Public Methods
		public float DistanceTo(DynamicGestureInstance otherInstance)
		{
			float distance = 0;
			var instanceSamples = otherInstance.GetResizedSampleList(Samples.Count);

			for(int i=0; i < Samples.Count; i++) 
			{
				var temp = Samples[i].DistanceTo(instanceSamples[i]);
				distance += temp;
			}

			distance = distance / (float)Samples.Count;

			return distance;
		}
		#endregion

		#region Private Methods
		// Gets the average number of samples
		private int getAverageSampleSize(List<DynamicGestureInstance> dgInstances)
		{
			if (dgInstances.Count == 0) return 0;
			int samples = 0;
			foreach (var instance in dgInstances) samples += instance.Samples.Count;
			return samples / dgInstances.Count;
		}
		#endregion
	}
}
