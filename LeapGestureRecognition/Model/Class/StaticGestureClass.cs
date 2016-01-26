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
			computeMeanValuesAndHandConfiguration(instances);
			computeStdDevValues(instances);
		}

		#region Public Properties
		[DataMember]
		public Dictionary<FeatureName, object> MeanValues { get; set; }
		[DataMember]
		public Dictionary<FeatureName, object> StdDevValues { get; set; } // Should I leave as float? Or change to object?


		// Need to consider edge cases with hands
		//	What if user does right hand point 5 times and left hand point 2 times?
		[DataMember]
		public HandConfiguration HandConfiguration { get; set; }
		#endregion

		HashSet<FeatureName> featuresToSkip = new HashSet<FeatureName>()
		{
			FeatureName.LeftPalmPosition,
			FeatureName.RightPalmPosition,

			// Temp:
			FeatureName.RightPalmSphereCenter,
			FeatureName.RightPalmSphereRadius,
			FeatureName.LeftPalmSphereCenter,
			FeatureName.LeftPalmSphereRadius,
		};

		#region Public Methods
		public float DistanceTo(StaticGestureInstance gestureInstance)
		{
			try
			{
				// Check if same hand configuration (BothHands, LeftHandOnly, RightHandOnly)...
				if (HandConfiguration != gestureInstance.HandConfiguration) return Single.PositiveInfinity;

				float distance = 0;
				int featureCount = 0; // Necessary to do this because some features (like Dictionary's) are really 5 features.
				foreach (var feature in gestureInstance.FeatureVector)
				{
					if (featuresToSkip.Contains(feature.Name)) continue;

					if (feature.Value is float)
					{
						// float values include Yaw, Pitch, Roll, and Sphere Radius
						var temp = Math.Abs(((float)feature.Value - (float)(double)MeanValues[feature.Name]) / ((float)(double)StdDevValues[feature.Name]));
						distance += Math.Abs(((float)feature.Value - (float)(double)MeanValues[feature.Name]) / ((float)(double)StdDevValues[feature.Name]));
						featureCount++;
					}
					else if (feature.Value is Dictionary<Finger.FingerType, Vec3>)
					{
						// This is just for fingersTipPositions
						foreach (var fingerTipPosition in (Dictionary<Finger.FingerType, Vec3>)feature.Value)
						{
							var meanFingerTipPositions = ((Newtonsoft.Json.Linq.JObject)MeanValues[feature.Name]).ToObject<Dictionary<Finger.FingerType, Vec3>>();
							var stdDevFingerTipPositions = ((Newtonsoft.Json.Linq.JObject)StdDevValues[feature.Name]).ToObject<Dictionary<Finger.FingerType, float>>();
							//var stdDevFingerTipPositions = (Dictionary<Finger.FingerType, float>)StdDevValues[feature.Name];
							Finger.FingerType fingerType = fingerTipPosition.Key;
							Vec3 instancePos = fingerTipPosition.Value;
							var temp = (instancePos).DistanceTo(meanFingerTipPositions[fingerType]) / stdDevFingerTipPositions[fingerType];
							distance += (instancePos).DistanceTo(meanFingerTipPositions[fingerType]) / stdDevFingerTipPositions[fingerType];
						}
						featureCount += 5;
					}
					else if (feature.Value is Dictionary<Finger.FingerType, bool>)
					{
						// This is just for fingers.IsExtended
						foreach (var fingerExtended in (Dictionary<Finger.FingerType, bool>)feature.Value)
						{
							Finger.FingerType fingerType = fingerExtended.Key;
							bool isExtended = fingerExtended.Value;
							float fingerExtendedPercentage = (((Newtonsoft.Json.Linq.JObject)MeanValues[feature.Name]).ToObject<Dictionary<Finger.FingerType, float>>())[fingerType];
							if (fingerExtendedPercentage >= 0.5f) // finger should be extended
							{
								if (!isExtended) distance += fingerExtendedPercentage;
							}
							else
							{
								if (isExtended) distance += 1 - fingerExtendedPercentage;
							}
						}
						featureCount += 5;
					}

					distance *= feature.Weight;
				}
				//return distance / (gestureInstance.FeatureVector.Count - featuresToSkip.Count);
				return distance / featureCount;
			}
			catch (Exception ex) { }
			return 0;
		}
		#endregion

		#region Private Methods
		private void computeMeanValuesAndHandConfiguration(ObservableCollection<StaticGestureInstanceWrapper> instances)
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
			var LeftFingersExtendedTotal = new Dictionary<Finger.FingerType, int>();
			var LeftFingerTipPositionsTotal = new Dictionary<Finger.FingerType, Vec3>();

			Vec3 RightPalmPosTotal = new Vec3();
			Vec3 RightThumbTipPosTotal = new Vec3();
			Vec3 RightIndexTipPosTotal = new Vec3();
			Vec3 RightMiddleTipPosTotal = new Vec3();
			Vec3 RightRingTipPosTotal = new Vec3();
			Vec3 RightPinkyTipPosTotal = new Vec3();
			float RightYawTotal = 0;
			float RightPitchTotal = 0;
			float RightRollTotal = 0;
			var RightFingersExtendedTotal = new Dictionary<Finger.FingerType, int>();
			var RightFingerTipPositionsTotal = new Dictionary<Finger.FingerType, Vec3>();

			// Initialize FingersExtended Dicts
			foreach (var fingerType in (Finger.FingerType[])Enum.GetValues(typeof(Finger.FingerType)))
			{
				LeftFingersExtendedTotal.Add(fingerType, 0);
				RightFingersExtendedTotal.Add(fingerType, 0);
				LeftFingerTipPositionsTotal.Add(fingerType, new Vec3());
				RightFingerTipPositionsTotal.Add(fingerType, new Vec3());
			}


			// Keep track of how many instances of each hand config.
			// If 31 instances of right hand only and 14 instances of left only, 
			//	consider the class right hand only.
			var handConfigCounts = new Dictionary<HandConfiguration, int>()
			{
				{ HandConfiguration.NoHands, 0 },
				{ HandConfiguration.LeftHandOnly, 0 },
				{ HandConfiguration.RightHandOnly, 0 },
				{ HandConfiguration.BothHands, 0 },
			};

			foreach (var instance in instances)
			{
				handConfigCounts[instance.Gesture.HandConfiguration]++;
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
						foreach (var fingerType in (Finger.FingerType[])Enum.GetValues(typeof(Finger.FingerType)))
						{
							if (hand.FingersExtended[fingerType]) LeftFingersExtendedTotal[fingerType]++;
							LeftFingerTipPositionsTotal[fingerType] += hand.FingerJointPositions_Relative[fingerType][Finger.FingerJoint.JOINT_TIP];
						}
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
						foreach (var fingerType in (Finger.FingerType[])Enum.GetValues(typeof(Finger.FingerType)))
						{
							if (hand.FingersExtended[fingerType]) RightFingersExtendedTotal[fingerType]++;
							RightFingerTipPositionsTotal[fingerType] += hand.FingerJointPositions_Relative[fingerType][Finger.FingerJoint.JOINT_TIP];
						}
					}
				}
			}

			HandConfiguration = handConfigCounts.FirstOrDefault(h => h.Value == handConfigCounts.Values.Max()).Key;


			MeanValues = new Dictionary<FeatureName, object>();

			MeanValues.Add(FeatureName.LeftPalmPosition, LeftPalmPosTotal / (float)instances.Count);
			MeanValues.Add(FeatureName.LeftYaw, LeftYawTotal / (float)instances.Count);
			MeanValues.Add(FeatureName.LeftPitch, LeftPitchTotal / (float)instances.Count);
			MeanValues.Add(FeatureName.LeftRoll, LeftRollTotal / (float)instances.Count);

			MeanValues.Add(FeatureName.RightPalmPosition, RightPalmPosTotal / (float)instances.Count);
			MeanValues.Add(FeatureName.RightYaw, RightYawTotal / (float)instances.Count);
			MeanValues.Add(FeatureName.RightPitch, RightPitchTotal / (float)instances.Count);
			MeanValues.Add(FeatureName.RightRoll, RightRollTotal / (float)instances.Count);

			var averagedLeftFingerTipPositions = new Dictionary<Finger.FingerType, Vec3>();
			var averagedRightFingerTipPositions = new Dictionary<Finger.FingerType, Vec3>();
			var leftFingersExtendedPercentages = new Dictionary<Finger.FingerType, float>();
			var rightFingersExtendedPercentages = new Dictionary<Finger.FingerType, float>();
			foreach (var fingerType in (Finger.FingerType[])Enum.GetValues(typeof(Finger.FingerType)))
			{
				averagedLeftFingerTipPositions.Add(fingerType, LeftFingerTipPositionsTotal[fingerType] / (float)instances.Count);
				averagedRightFingerTipPositions.Add(fingerType, RightFingerTipPositionsTotal[fingerType] / (float)instances.Count);
				// Set MeanValue for FingersExtended as a float: The percentage that are true.
				leftFingersExtendedPercentages.Add(fingerType, LeftFingersExtendedTotal[fingerType] / (float)instances.Count);
				rightFingersExtendedPercentages.Add(fingerType, RightFingersExtendedTotal[fingerType] / (float)instances.Count);
			}
			MeanValues.Add(FeatureName.LeftFingerTipPositions, averagedLeftFingerTipPositions);
			MeanValues.Add(FeatureName.RightFingerTipPositions, averagedRightFingerTipPositions);
			MeanValues.Add(FeatureName.LeftFingersExtended, leftFingersExtendedPercentages);
			MeanValues.Add(FeatureName.RightFingersExtended, rightFingersExtendedPercentages);
		}

		private void computeStdDevValues(ObservableCollection<StaticGestureInstanceWrapper> instances)
		{
			float LeftPalmPosTotal = 0;
			float LeftYawTotal = 0;
			float LeftPitchTotal = 0;
			float LeftRollTotal = 0;
			var LeftFingerTipPosTotal = new Dictionary<Finger.FingerType, float>();

			float RightPalmPosTotal = 0;
			float RightYawTotal = 0;
			float RightPitchTotal = 0;
			float RightRollTotal = 0;
			var RightFingerTipPosTotal = new Dictionary<Finger.FingerType, float>();

			foreach (var fingerType in (Finger.FingerType[])Enum.GetValues(typeof(Finger.FingerType)))
			{
				LeftFingerTipPosTotal.Add(fingerType, 0);
				RightFingerTipPosTotal.Add(fingerType, 0);
			}

			foreach (var instance in instances)
			{
				foreach (var hand in instance.Gesture.Hands)
				{
					if (hand.IsLeft)
					{
						LeftPalmPosTotal += hand.PalmPosition.DistanceTo((Vec3)MeanValues[FeatureName.LeftPalmPosition]);
						LeftYawTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureName.LeftYaw]));
						LeftPitchTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureName.LeftPitch]));
						LeftRollTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureName.LeftRoll]));
						foreach (var fingerType in (Finger.FingerType[])Enum.GetValues(typeof(Finger.FingerType)))
						{
							LeftFingerTipPosTotal[fingerType] += hand.FingerTipPositions[fingerType].DistanceTo(((Dictionary<Finger.FingerType, Vec3>)MeanValues[FeatureName.LeftFingerTipPositions])[fingerType]);
						}
					}
					else
					{
						RightPalmPosTotal += hand.PalmPosition.DistanceTo((Vec3)MeanValues[FeatureName.RightPalmPosition]);
						RightYawTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureName.RightYaw]));
						RightPitchTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureName.RightPitch]));
						RightRollTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureName.RightRoll]));
						foreach (var fingerType in (Finger.FingerType[])Enum.GetValues(typeof(Finger.FingerType)))
						{
							RightFingerTipPosTotal[fingerType] += hand.FingerTipPositions[fingerType].DistanceTo(((Dictionary<Finger.FingerType, Vec3>)MeanValues[FeatureName.RightFingerTipPositions])[fingerType]);
						}
					}
				}
			}

			StdDevValues = new Dictionary<FeatureName, object>();
			StdDevValues.Add(FeatureName.LeftPalmPosition, LeftPalmPosTotal / (float)instances.Count);
			StdDevValues.Add(FeatureName.LeftYaw, LeftYawTotal / (float)instances.Count);
			StdDevValues.Add(FeatureName.LeftPitch, LeftPitchTotal / (float)instances.Count);
			StdDevValues.Add(FeatureName.LeftRoll, LeftRollTotal / (float)instances.Count);

			StdDevValues.Add(FeatureName.RightPalmPosition, RightPalmPosTotal / (float)instances.Count);
			StdDevValues.Add(FeatureName.RightYaw, RightYawTotal / (float)instances.Count);
			StdDevValues.Add(FeatureName.RightPitch, RightPitchTotal / (float)instances.Count);
			StdDevValues.Add(FeatureName.RightRoll, RightRollTotal / (float)instances.Count);

			var stdDevRightFingerTipPositions = new Dictionary<Finger.FingerType, float>();
			var stdDevLeftFingerTipPositions = new Dictionary<Finger.FingerType, float>();
			foreach (var fingerType in (Finger.FingerType[])Enum.GetValues(typeof(Finger.FingerType)))
			{
				stdDevRightFingerTipPositions.Add(fingerType, RightFingerTipPosTotal[fingerType] / (float)instances.Count);
				stdDevLeftFingerTipPositions.Add(fingerType, LeftFingerTipPosTotal[fingerType] / (float)instances.Count);
			}
			StdDevValues.Add(FeatureName.LeftFingerTipPositions, stdDevLeftFingerTipPositions);
			StdDevValues.Add(FeatureName.RightFingerTipPositions, stdDevRightFingerTipPositions);

		}

		#endregion

	}
}
