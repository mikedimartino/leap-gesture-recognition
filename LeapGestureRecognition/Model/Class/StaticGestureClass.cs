using Leap;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LeapGestureRecognition.Model
{
	[DataContract]
	public class StaticGestureClass
	{
		public StaticGestureClass() { }
		public StaticGestureClass(ObservableCollection<StaticGestureInstanceWrapper> instances)
		{
			computeMeanValues(instances);
			computeStdDevValues(instances);
		}

		[DataMember]
		public Dictionary<FeatureType, object> MeanValues { get; set; }
		[DataMember]
		public Dictionary<FeatureType, float> StdDevValues { get; set; } // Should I leave as float? Or change to object?

		private void computeMeanValues(ObservableCollection<StaticGestureInstanceWrapper> instances)
		{
			LGR_Vec3 LeftPalmPosTotal = new LGR_Vec3();
			LGR_Vec3 LeftThumbTipPosTotal = new LGR_Vec3();
			LGR_Vec3 LeftIndexTipPosTotal = new LGR_Vec3();
			LGR_Vec3 LeftMiddleTipPosTotal = new LGR_Vec3();
			LGR_Vec3 LeftRingTipPosTotal = new LGR_Vec3();
			LGR_Vec3 LeftPinkyTipPosTotal = new LGR_Vec3();
			float LeftYawTotal = 0;
			float LeftPitchTotal = 0;
			float LeftRollTotal = 0;

			LGR_Vec3 RightPalmPosTotal = new LGR_Vec3();
			LGR_Vec3 RightThumbTipPosTotal = new LGR_Vec3();
			LGR_Vec3 RightIndexTipPosTotal = new LGR_Vec3();
			LGR_Vec3 RightMiddleTipPosTotal = new LGR_Vec3();
			LGR_Vec3 RightRingTipPosTotal = new LGR_Vec3();
			LGR_Vec3 RightPinkyTipPosTotal = new LGR_Vec3();
			float RightYawTotal = 0;
			float RightPitchTotal = 0;
			float RightRollTotal = 0;

			foreach (var instance in instances)
			{
				foreach (var hand in instance.Gesture.Hands)
				{
					if (hand.IsLeft)
					{
						LeftPalmPosTotal += hand.PalmPosition;
						LeftThumbTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_THUMB][Finger.FingerJoint.JOINT_TIP];
						LeftIndexTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_INDEX][Finger.FingerJoint.JOINT_TIP];
						LeftMiddleTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_MIDDLE][Finger.FingerJoint.JOINT_TIP];
						LeftRingTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_RING][Finger.FingerJoint.JOINT_TIP];
						LeftPinkyTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_PINKY][Finger.FingerJoint.JOINT_TIP];
						LeftYawTotal += hand.Yaw;
						LeftPitchTotal += hand.Pitch;
						LeftRollTotal += hand.Roll;
					}
					else
					{
						RightPalmPosTotal += hand.PalmPosition;
						RightThumbTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_THUMB][Finger.FingerJoint.JOINT_TIP];
						RightIndexTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_INDEX][Finger.FingerJoint.JOINT_TIP];
						RightMiddleTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_MIDDLE][Finger.FingerJoint.JOINT_TIP];
						RightRingTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_RING][Finger.FingerJoint.JOINT_TIP];
						RightPinkyTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_PINKY][Finger.FingerJoint.JOINT_TIP];
						RightYawTotal += hand.Yaw;
						RightPitchTotal += hand.Pitch;
						RightRollTotal += hand.Roll;
					}
				}
			}

			MeanValues = new Dictionary<FeatureType, object>();

			MeanValues.Add(FeatureType.LeftPalmPosition, LeftPalmPosTotal / instances.Count);
			MeanValues.Add(FeatureType.LeftThumbTipPosition, LeftThumbTipPosTotal / instances.Count);
			MeanValues.Add(FeatureType.LeftIndexTipPosition, LeftIndexTipPosTotal / instances.Count);
			MeanValues.Add(FeatureType.LeftMiddleTipPosition, LeftMiddleTipPosTotal / instances.Count);
			MeanValues.Add(FeatureType.LeftRingTipPosition, LeftRingTipPosTotal / instances.Count);
			MeanValues.Add(FeatureType.LeftPinkyTipPosition, LeftPinkyTipPosTotal / instances.Count);
			MeanValues.Add(FeatureType.LeftYaw, LeftYawTotal / instances.Count);
			MeanValues.Add(FeatureType.LeftPitch, LeftPitchTotal / instances.Count);
			MeanValues.Add(FeatureType.LeftRoll, LeftRollTotal / instances.Count);

			MeanValues.Add(FeatureType.RightPalmPosition, RightPalmPosTotal / instances.Count);
			MeanValues.Add(FeatureType.RightThumbTipPosition, RightThumbTipPosTotal / instances.Count);
			MeanValues.Add(FeatureType.RightIndexTipPosition, RightIndexTipPosTotal / instances.Count);
			MeanValues.Add(FeatureType.RightMiddleTipPosition, RightMiddleTipPosTotal / instances.Count);
			MeanValues.Add(FeatureType.RightRingTipPosition, RightRingTipPosTotal / instances.Count);
			MeanValues.Add(FeatureType.RightPinkyTipPosition, RightPinkyTipPosTotal / instances.Count);
			MeanValues.Add(FeatureType.RightYaw, RightYawTotal / instances.Count);
			MeanValues.Add(FeatureType.RightPitch, RightPitchTotal / instances.Count);
			MeanValues.Add(FeatureType.RightRoll, RightRollTotal / instances.Count);

		}

		private void computeStdDevValues(ObservableCollection<StaticGestureInstanceWrapper> instances)
		{
			float LeftPalmPosTotal = 0;
			float LeftThumbTipPosTotal = 0;
			float LeftIndexTipPosTotal = 0;
			float LeftMiddleTipPosTotal = 0;
			float LeftRingTipPosTotal = 0;
			float LeftPinkyTipPosTotal = 0;
			float LeftYawTotal = 0;
			float LeftPitchTotal = 0;
			float LeftRollTotal = 0;

			float RightPalmPosTotal = 0;
			float RightThumbTipPosTotal = 0;
			float RightIndexTipPosTotal = 0;
			float RightMiddleTipPosTotal = 0;
			float RightRingTipPosTotal = 0;
			float RightPinkyTipPosTotal = 0;
			float RightYawTotal = 0;
			float RightPitchTotal = 0;
			float RightRollTotal = 0;

			foreach (var instance in instances)
			{
				foreach (var hand in instance.Gesture.Hands)
				{
					if (hand.IsLeft)
					{
						LeftPalmPosTotal += hand.PalmPosition.DistanceTo((LGR_Vec3)MeanValues[FeatureType.LeftPalmPosition]);
						LeftThumbTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_THUMB][Finger.FingerJoint.JOINT_TIP].DistanceTo((LGR_Vec3)MeanValues[FeatureType.LeftThumbTipPosition]);
						LeftIndexTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_INDEX][Finger.FingerJoint.JOINT_TIP].DistanceTo((LGR_Vec3)MeanValues[FeatureType.LeftIndexTipPosition]);
						LeftMiddleTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_MIDDLE][Finger.FingerJoint.JOINT_TIP].DistanceTo((LGR_Vec3)MeanValues[FeatureType.LeftMiddleTipPosition]);
						LeftRingTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_RING][Finger.FingerJoint.JOINT_TIP].DistanceTo((LGR_Vec3)MeanValues[FeatureType.LeftRingTipPosition]);
						LeftPinkyTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_PINKY][Finger.FingerJoint.JOINT_TIP].DistanceTo((LGR_Vec3)MeanValues[FeatureType.LeftPinkyTipPosition]);
						LeftYawTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureType.LeftYaw]));
						LeftPitchTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureType.LeftPitch]));
						LeftRollTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureType.LeftRoll]));
					}
					else
					{
						RightPalmPosTotal += hand.PalmPosition.DistanceTo((LGR_Vec3)MeanValues[FeatureType.RightPalmPosition]);
						RightThumbTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_THUMB][Finger.FingerJoint.JOINT_TIP].DistanceTo((LGR_Vec3)MeanValues[FeatureType.RightThumbTipPosition]);
						RightIndexTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_INDEX][Finger.FingerJoint.JOINT_TIP].DistanceTo((LGR_Vec3)MeanValues[FeatureType.RightIndexTipPosition]);
						RightMiddleTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_MIDDLE][Finger.FingerJoint.JOINT_TIP].DistanceTo((LGR_Vec3)MeanValues[FeatureType.RightMiddleTipPosition]);
						RightRingTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_RING][Finger.FingerJoint.JOINT_TIP].DistanceTo((LGR_Vec3)MeanValues[FeatureType.RightRingTipPosition]);
						RightPinkyTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_PINKY][Finger.FingerJoint.JOINT_TIP].DistanceTo((LGR_Vec3)MeanValues[FeatureType.RightPinkyTipPosition]);
						RightYawTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureType.RightYaw]));
						RightPitchTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureType.RightPitch]));
						RightRollTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureType.RightRoll]));
					}
				}
			}

			StdDevValues = new Dictionary<FeatureType, float>();
			StdDevValues.Add(FeatureType.LeftPalmPosition, LeftPalmPosTotal / instances.Count);
			StdDevValues.Add(FeatureType.LeftThumbTipPosition, LeftThumbTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureType.LeftIndexTipPosition, LeftIndexTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureType.LeftMiddleTipPosition, LeftMiddleTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureType.LeftRingTipPosition, LeftRingTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureType.LeftPinkyTipPosition, LeftPinkyTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureType.LeftYaw, LeftYawTotal / instances.Count);
			StdDevValues.Add(FeatureType.LeftPitch, LeftPitchTotal / instances.Count);
			StdDevValues.Add(FeatureType.LeftRoll, LeftRollTotal / instances.Count);

			StdDevValues.Add(FeatureType.RightPalmPosition, RightPalmPosTotal / instances.Count);
			StdDevValues.Add(FeatureType.RightThumbTipPosition, RightThumbTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureType.RightIndexTipPosition, RightIndexTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureType.RightMiddleTipPosition, RightMiddleTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureType.RightRingTipPosition, RightRingTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureType.RightPinkyTipPosition, RightPinkyTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureType.RightYaw, RightYawTotal / instances.Count);
			StdDevValues.Add(FeatureType.RightPitch, RightPitchTotal / instances.Count);
			StdDevValues.Add(FeatureType.RightRoll, RightRollTotal / instances.Count);

		}
			

	}
}
