using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LeapGestureRecognition
{
	[DataContract]
	public class DGClassSample : SGClass
	{
		#region Public Properties
		[DataMember]
		public Vec3 MeanRightPalmVelocity { get; set; }
		[DataMember]
		public Vec3 MeanLeftPalmVelocity { get; set; }

		[DataMember]
		public float StdDevRightPalmVelocity { get; set; }
		[DataMember]
		public float StdDevLeftPalmVelocity { get; set; }
		#endregion

		#region Contructors
		public DGClassSample() : base() { }

		public DGClassSample(List<DGInstanceSample> instances, Dictionary<string, int> featureWeights = null)
			: base(instances.Select(i => i as SGInstance).ToList(), featureWeights)
		{
			computeMeanValues(instances);
			computeStdDevValues(instances);
		}
		#endregion

		#region Public Methods
		public float DistanceTo(DGInstanceSample otherInstance)
		{
			float dgDistance = 0;

			//distance += (this as SGClass).DistanceTo(otherInstance);

			// Need to figure out how to weight these DG features appropriately.
			int dgFeatureCount = 0;
			if (otherInstance.LeftHand != null)
			{
				dgDistance += MeanLeftPalmVelocity.DistanceTo(otherInstance.LeftPalmVelocity) / StdDevLeftPalmVelocity;
				dgFeatureCount++;
			}
			if (otherInstance.RightHand != null)
			{
				dgDistance += MeanRightPalmVelocity.DistanceTo(otherInstance.RightPalmVelocity) / StdDevRightPalmVelocity;
				dgFeatureCount++;
			}

			dgDistance /= (float)dgFeatureCount;
			float sgDistance = base.DistanceTo(otherInstance as SGInstance);

			// The two weights must add up to 1.0f
			float sgWeight = 0.5f;
			float dgWeight = 0.5f;

			float result = (dgDistance * dgWeight) + (sgDistance * sgWeight);
			return result;
		}
		#endregion

		#region Private Methods
		protected void computeMeanValues(List<DGInstanceSample> instances)
		{
			MeanRightPalmVelocity = new Vec3();
			MeanLeftPalmVelocity = new Vec3();

			foreach (var instance in instances)
			{
				if (instance.LeftHand != null)
				{
					MeanLeftPalmVelocity += instance.LeftPalmVelocity;
				}
				if (instance.RightHand != null)
				{
					MeanRightPalmVelocity += instance.RightPalmVelocity;
				}
			}

			MeanRightPalmVelocity /= instances.Count;
			MeanLeftPalmVelocity /= instances.Count;
		}

		protected void computeStdDevValues(List<DGInstanceSample> instances)
		{
			StdDevLeftPalmVelocity = 0;
			StdDevRightPalmVelocity = 0;

			foreach (var instance in instances)
			{
				if (instance.LeftHand != null)
				{
					StdDevLeftPalmVelocity += instance.LeftPalmVelocity.DistanceTo(MeanLeftPalmVelocity);
				}
				if (instance.RightHand != null)
				{
					StdDevRightPalmVelocity += instance.RightPalmVelocity.DistanceTo(MeanRightPalmVelocity);
				}
			}

			StdDevLeftPalmVelocity /= instances.Count;
			StdDevRightPalmVelocity /= instances.Count;
		}
		#endregion

	}
}
