using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LGR
{
	[DataContract]
	public class DynamicGestureInstanceSample : StaticGestureInstance
	{
		public DynamicGestureInstanceSample() { }
		public DynamicGestureInstanceSample(Frame frame, DynamicGestureInstanceSample startGesture = null) : base(frame)
		{
			if (startGesture == null)
			{
				LeftDisplacementFromStart = new Vec3();
				RightDisplacementFromStart = new Vec3();
			}
			else
			{
				if (startGesture.LeftHand != null) LeftDisplacementFromStart = new Vec3(startGesture.LeftHand.HandTransform.TransformPoint(LeftHand.PalmPosition.ToLeapVector()));
				if (startGesture.RightHand != null) RightDisplacementFromStart = new Vec3(startGesture.RightHand.HandTransform.TransformPoint(RightHand.PalmPosition.ToLeapVector()));
				// Deal with number of hands differences
			}
		}

		#region Public Properties
		[DataMember]
		public Vec3 LeftDisplacementFromStart { get; set; }
		[DataMember]
		public Vec3 RightDisplacementFromStart { get; set; }
		#endregion

		#region Public Methods
		//public float DistanceTo(DynamicGestureInstanceSample otherGesture)
		//{
		//	return ((StaticGestureInstance)this).DistanceTo((StaticGestureInstance)otherGesture);
		//	//TODO: Add to distance based on left / right displacement(s) from start
		//}
		#endregion
	}
}
