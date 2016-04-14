using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LeapGestureRecognition
{
	[DataContract]
	public class DGInstanceSample : SGInstance
	{
		#region Public Properties
		[DataMember]
		public Vec3 LeftPalmVelocity { get; set; }
		[DataMember]
		public Vec3 RightPalmVelocity { get; set; }

		// Instead of above, do:
		//[DataMember]
		//public List<Feature> FeatureVector { get; set; }
		#endregion

		#region Constructors
		public DGInstanceSample() : base() { }

		public DGInstanceSample(Frame frame)
			: base(frame)
		{
			foreach (var hand in frame.Hands)
			{
				if (hand.IsLeft) LeftPalmVelocity = new Vec3(hand.PalmVelocity.Normalized);
				if (hand.IsRight) RightPalmVelocity = new Vec3(hand.PalmVelocity);
			}
		}

		public DGInstanceSample(SGInstance sgInstance)
		{
			Hands = sgInstance.Hands;
			LeftHand = sgInstance.LeftHand;
			RightHand = sgInstance.RightHand;
			HandConfiguration = sgInstance.HandConfiguration;
			FeatureVector = sgInstance.FeatureVector; // Should probably have new feature vector here that is a superset of DG feature vector.
			Features = sgInstance.Features;
		}

		#endregion

		#region Public Methods
		//TODO: Check if this works (with all the casting and stuff).
		public DGInstanceSample Lerp(DGInstanceSample otherInstance, float amount)
		{
			SGInstance sgLerp = base.Lerp(otherInstance, amount);
			var lerpedSample = new DGInstanceSample(sgLerp);
			if (LeftHand != null)
			{
				lerpedSample.LeftPalmVelocity = LeftPalmVelocity.Lerp(otherInstance.LeftPalmVelocity, amount);
			}
			if (RightHand != null)
			{
				lerpedSample.RightPalmVelocity = RightPalmVelocity.Lerp(otherInstance.RightPalmVelocity, amount);
			}
			return lerpedSample;
		}
		#endregion

	}
}
