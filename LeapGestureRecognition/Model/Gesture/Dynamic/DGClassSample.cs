using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LGR
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
			float distance = 0;

			distance += (this as SGClass).DistanceTo(otherInstance);

			// Need to figure out how to weight these DG features appropriately.
			int featureCount = 0;
			if (otherInstance.LeftHand != null)
			{
				distance += MeanLeftPalmVelocity.DistanceTo(otherInstance.LeftPalmVelocity) / StdDevLeftPalmVelocity;
				featureCount++;
			}
			if (otherInstance.RightHand != null)
			{
				distance += MeanRightPalmVelocity.DistanceTo(otherInstance.RightPalmVelocity) / StdDevRightPalmVelocity;
				featureCount++;
			}

			distance /= (float)featureCount;
			float result = (distance + base.DistanceTo(otherInstance as SGInstance)) / 2.0f;
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
