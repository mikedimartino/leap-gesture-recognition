using Leap;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LGR
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

		#region Public Properties
		[DataMember]
		public Dictionary<FeatureName, object> MeanValues { get; set; }
		[DataMember]
		public Dictionary<FeatureName, float> StdDevValues { get; set; } // Should I leave as float? Or change to object?

		// Need to consider edge cases with hands
		//	What if user does right hand point 5 times and left hand point 2 times?

		[DataMember]
		public int NumHands { get; set; }
		[DataMember]
		public bool LeftHandExists { get; set; }
		[DataMember]
		public bool RightHandExists { get; set; }
		#endregion

		HashSet<FeatureName> featuresToSkip = new HashSet<FeatureName>()
		{
			FeatureName.LeftPalmPosition,
			FeatureName.RightPalmPosition,
		};

		#region Public Methods
		public float DistanceTo(StaticGestureInstance gestureInstance)
		{
			float distance = 0;
			foreach(var feature in gestureInstance.FeatureVector)
			{
				if (featuresToSkip.Contains(feature.Name)) continue;

				if (feature.Value is Vec3)
				{
					Vec3 mean = ((Newtonsoft.Json.Linq.JObject)MeanValues[feature.Name]).ToObject<Vec3>();
					//float stdDev = ((Newtonsoft.Json.Linq.JObject)StdDevValues[feature.Name]).ToObject<float>()
					//distance += ((Vec3)(feature.Value)).DistanceTo((Vec3)(MeanValues[feature.Name])) / StdDevValues[feature.Name];
					distance += ((Vec3)(feature.Value)).DistanceTo(mean) / StdDevValues[feature.Name];

				}
				else if (feature.Value is float)
				{
					distance += ((float)feature.Value - (float)(double)MeanValues[feature.Name]) / StdDevValues[feature.Name];
				}
				distance *= feature.Weight;
			}
			return distance / (gestureInstance.FeatureVector.Count - featuresToSkip.Count);
		}
		#endregion

		#region Private Methods
		private void computeMeanValues(ObservableCollection<StaticGestureInstanceWrapper> instances)
		{
			Vec3 LeftPalmPosTotal = new Vec3();
			Vec3 LeftThumbTipPosTotal = new Vec3();
			Vec3 LeftIndexTipPosTotal = new Vec3();
			Vec3 LeftMiddleTipPosTotal = new Vec3();
			Vec3 LeftRingTipPosTotal = new Vec3();
			Vec3 LeftPinkyTipPosTotal = new Vec3();
			float LeftYawTotal = 0;
			float LeftPitchTotal = 0;
			float LeftRollTotal = 0;

			Vec3 RightPalmPosTotal = new Vec3();
			Vec3 RightThumbTipPosTotal = new Vec3();
			Vec3 RightIndexTipPosTotal = new Vec3();
			Vec3 RightMiddleTipPosTotal = new Vec3();
			Vec3 RightRingTipPosTotal = new Vec3();
			Vec3 RightPinkyTipPosTotal = new Vec3();
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

			MeanValues = new Dictionary<FeatureName, object>();

			MeanValues.Add(FeatureName.LeftPalmPosition, LeftPalmPosTotal / instances.Count);
			MeanValues.Add(FeatureName.LeftThumbTipPosition, LeftThumbTipPosTotal / instances.Count);
			MeanValues.Add(FeatureName.LeftIndexTipPosition, LeftIndexTipPosTotal / instances.Count);
			MeanValues.Add(FeatureName.LeftMiddleTipPosition, LeftMiddleTipPosTotal / instances.Count);
			MeanValues.Add(FeatureName.LeftRingTipPosition, LeftRingTipPosTotal / instances.Count);
			MeanValues.Add(FeatureName.LeftPinkyTipPosition, LeftPinkyTipPosTotal / instances.Count);
			MeanValues.Add(FeatureName.LeftYaw, LeftYawTotal / instances.Count);
			MeanValues.Add(FeatureName.LeftPitch, LeftPitchTotal / instances.Count);
			MeanValues.Add(FeatureName.LeftRoll, LeftRollTotal / instances.Count);

			MeanValues.Add(FeatureName.RightPalmPosition, RightPalmPosTotal / instances.Count);
			MeanValues.Add(FeatureName.RightThumbTipPosition, RightThumbTipPosTotal / instances.Count);
			MeanValues.Add(FeatureName.RightIndexTipPosition, RightIndexTipPosTotal / instances.Count);
			MeanValues.Add(FeatureName.RightMiddleTipPosition, RightMiddleTipPosTotal / instances.Count);
			MeanValues.Add(FeatureName.RightRingTipPosition, RightRingTipPosTotal / instances.Count);
			MeanValues.Add(FeatureName.RightPinkyTipPosition, RightPinkyTipPosTotal / instances.Count);
			MeanValues.Add(FeatureName.RightYaw, RightYawTotal / instances.Count);
			MeanValues.Add(FeatureName.RightPitch, RightPitchTotal / instances.Count);
			MeanValues.Add(FeatureName.RightRoll, RightRollTotal / instances.Count);

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
						LeftPalmPosTotal += hand.PalmPosition.DistanceTo((Vec3)MeanValues[FeatureName.LeftPalmPosition]);
						LeftThumbTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_THUMB][Finger.FingerJoint.JOINT_TIP].DistanceTo((Vec3)MeanValues[FeatureName.LeftThumbTipPosition]);
						LeftIndexTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_INDEX][Finger.FingerJoint.JOINT_TIP].DistanceTo((Vec3)MeanValues[FeatureName.LeftIndexTipPosition]);
						LeftMiddleTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_MIDDLE][Finger.FingerJoint.JOINT_TIP].DistanceTo((Vec3)MeanValues[FeatureName.LeftMiddleTipPosition]);
						LeftRingTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_RING][Finger.FingerJoint.JOINT_TIP].DistanceTo((Vec3)MeanValues[FeatureName.LeftRingTipPosition]);
						LeftPinkyTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_PINKY][Finger.FingerJoint.JOINT_TIP].DistanceTo((Vec3)MeanValues[FeatureName.LeftPinkyTipPosition]);
						LeftYawTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureName.LeftYaw]));
						LeftPitchTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureName.LeftPitch]));
						LeftRollTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureName.LeftRoll]));
					}
					else
					{
						RightPalmPosTotal += hand.PalmPosition.DistanceTo((Vec3)MeanValues[FeatureName.RightPalmPosition]);
						RightThumbTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_THUMB][Finger.FingerJoint.JOINT_TIP].DistanceTo((Vec3)MeanValues[FeatureName.RightThumbTipPosition]);
						RightIndexTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_INDEX][Finger.FingerJoint.JOINT_TIP].DistanceTo((Vec3)MeanValues[FeatureName.RightIndexTipPosition]);
						RightMiddleTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_MIDDLE][Finger.FingerJoint.JOINT_TIP].DistanceTo((Vec3)MeanValues[FeatureName.RightMiddleTipPosition]);
						RightRingTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_RING][Finger.FingerJoint.JOINT_TIP].DistanceTo((Vec3)MeanValues[FeatureName.RightRingTipPosition]);
						RightPinkyTipPosTotal += hand.FingerJointPositions_Relative[Finger.FingerType.TYPE_PINKY][Finger.FingerJoint.JOINT_TIP].DistanceTo((Vec3)MeanValues[FeatureName.RightPinkyTipPosition]);
						RightYawTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureName.RightYaw]));
						RightPitchTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureName.RightPitch]));
						RightRollTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureName.RightRoll]));
					}
				}
			}

			StdDevValues = new Dictionary<FeatureName, float>();
			StdDevValues.Add(FeatureName.LeftPalmPosition, LeftPalmPosTotal / instances.Count);
			StdDevValues.Add(FeatureName.LeftThumbTipPosition, LeftThumbTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureName.LeftIndexTipPosition, LeftIndexTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureName.LeftMiddleTipPosition, LeftMiddleTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureName.LeftRingTipPosition, LeftRingTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureName.LeftPinkyTipPosition, LeftPinkyTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureName.LeftYaw, LeftYawTotal / instances.Count);
			StdDevValues.Add(FeatureName.LeftPitch, LeftPitchTotal / instances.Count);
			StdDevValues.Add(FeatureName.LeftRoll, LeftRollTotal / instances.Count);

			StdDevValues.Add(FeatureName.RightPalmPosition, RightPalmPosTotal / instances.Count);
			StdDevValues.Add(FeatureName.RightThumbTipPosition, RightThumbTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureName.RightIndexTipPosition, RightIndexTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureName.RightMiddleTipPosition, RightMiddleTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureName.RightRingTipPosition, RightRingTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureName.RightPinkyTipPosition, RightPinkyTipPosTotal / instances.Count);
			StdDevValues.Add(FeatureName.RightYaw, RightYawTotal / instances.Count);
			StdDevValues.Add(FeatureName.RightPitch, RightPitchTotal / instances.Count);
			StdDevValues.Add(FeatureName.RightRoll, RightRollTotal / instances.Count);

		}
		#endregion

	}
}
