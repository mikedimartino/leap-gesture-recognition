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
	public class SGClass
	{
		#region Public Properties
		public GestureType GestureType { get { return GestureType.Static; } }

		[DataMember]
		public Dictionary<FeatureName, object> MeanValues { get; set; }
		[DataMember]
		public Dictionary<FeatureName, object> StdDevValues { get; set; }
		[DataMember]
		public Dictionary<string, int> FeatureWeights { get; set; } // Key is FeatureName.ToString() + [FingerType.ToString()]


		// Need to consider edge cases with hands
		//	What if user does right hand point 5 times and left hand point 2 times?
		[DataMember]
		public HandConfiguration HandConfiguration { get; set; }
		#endregion

		#region Constructors
		public SGClass() { }

		// Should be StaticGestureInstance (not wrapper). Also try to do IEnumerable or List instead of ObservableCollection.
		public SGClass(ObservableCollection<SGInstanceWrapper> sgInstances, Dictionary<string, int> featureWeights = null)
			: this(sgInstances.Select(sgi => sgi.Gesture).ToList(), featureWeights) { }

		public SGClass(List<SGInstance> sgInstances, Dictionary<string, int> featureWeights = null)
		{
			computeMeanValuesAndHandConfiguration(sgInstances);
			computeStdDevValues(sgInstances);
			FeatureWeights = featureWeights ?? GetDefaultFeatureWeights();
		}
		#endregion

		HashSet<FeatureName> featuresToSkip = new HashSet<FeatureName>()
		{
			FeatureName.LeftPalmPosition,
			FeatureName.RightPalmPosition,
			FeatureName.LeftPalmSphereCenter,
			FeatureName.RightPalmSphereCenter,
			FeatureName.LeftPalmSphereRadius, // Issue with this
			FeatureName.RightPalmSphereRadius, // Issue with this
		};

		#region Public Methods
		// Returns an initialized Dictionary with all feature weights set to 1
		public static Dictionary<string, int> GetDefaultFeatureWeights()
		{
			var featureWeights = new Dictionary<string, int>();
			//var fingerType in (Finger.FingerType[])Enum.GetValues(typeof(Finger.FingerType))
			foreach (var feature in (FeatureName[])Enum.GetValues(typeof(FeatureName)))
			{
				string featureString = feature.ToString();
				if (featureString.Contains("Finger"))
				{
					foreach (var fingerType in (Finger.FingerType[])Enum.GetValues(typeof(Finger.FingerType)))
					{
						featureWeights.Add(featureString + fingerType, 1);
					}
				}
				else
				{
					featureWeights.Add(featureString, 1);
				}
			}
			return featureWeights;
		}

		public float DistanceTo(SGInstance gestureInstance)
		{
			// Check if same hand configuration (BothHands, LeftHandOnly, RightHandOnly)...
			if (HandConfiguration != gestureInstance.HandConfiguration) return Single.PositiveInfinity;

			float distance = 0;
			int featureCount = 0; // Necessary to do this because some features (like Dictionary's) are really 5 features.
			int featureWeight;
			foreach (var feature in gestureInstance.FeatureVector)
			{
				if (featuresToSkip.Contains(feature.Name)) continue;

				if (feature.Value is Vec3)
				{
					featureWeight = FeatureWeights[feature.Name.ToString()];
					Vec3 mean = ((Newtonsoft.Json.Linq.JObject)MeanValues[feature.Name]).ToObject<Vec3>();
					distance += featureWeight * ((Vec3)(feature.Value)).DistanceTo(mean) / ((float)StdDevValues[feature.Name]);
					featureCount += featureWeight;
				}
				else if (feature.Value is float)
				{
					featureWeight = FeatureWeights[feature.Name.ToString()];
					// float values include Yaw, Pitch, Roll, and Sphere Radius
					distance += featureWeight * Math.Abs(((float)feature.Value - (float)(double)MeanValues[feature.Name]) / ((float)(double)StdDevValues[feature.Name]));
					featureCount += featureWeight;
				}
				else if (feature.Value is Dictionary<Finger.FingerType, Vec3>)
				{
					// This is just for fingersTipPositions
					foreach (var fingerTipPosition in (Dictionary<Finger.FingerType, Vec3>)feature.Value)
					{
						Finger.FingerType fingerType = fingerTipPosition.Key;
						featureWeight = FeatureWeights[feature.Name.ToString() + fingerType];
						var meanFingerTipPositions = ((Newtonsoft.Json.Linq.JObject)MeanValues[feature.Name]).ToObject<Dictionary<Finger.FingerType, Vec3>>();
						var stdDevFingerTipPositions = ((Newtonsoft.Json.Linq.JObject)StdDevValues[feature.Name]).ToObject<Dictionary<Finger.FingerType, float>>();
						//var stdDevFingerTipPositions = (Dictionary<Finger.FingerType, float>)StdDevValues[feature.Name];
						Vec3 instancePos = fingerTipPosition.Value;
						distance += featureWeight * (instancePos).DistanceTo(meanFingerTipPositions[fingerType]) / stdDevFingerTipPositions[fingerType];
						featureCount += featureWeight;
					}
				}
				else if (feature.Value is Dictionary<Finger.FingerType, bool>)
				{
					// This is just for fingers.IsExtended
					foreach (var fingerExtended in (Dictionary<Finger.FingerType, bool>)feature.Value)
					{
						Finger.FingerType fingerType = fingerExtended.Key;
						featureWeight = FeatureWeights[feature.Name.ToString() + fingerType];
						bool isExtended = fingerExtended.Value;
						float fingerExtendedPercentage = (((Newtonsoft.Json.Linq.JObject)MeanValues[feature.Name]).ToObject<Dictionary<Finger.FingerType, float>>())[fingerType];
						if (fingerExtendedPercentage >= 0.5f) // finger should be extended
						{
							if (!isExtended) distance += featureWeight; //distance += fingerExtendedPercentage;
						}
						else
						{
							if (isExtended) distance += featureWeight; //distance += 1 - fingerExtendedPercentage;
						}
						featureCount += featureWeight;
					}
				}
			}
			//return distance / (gestureInstance.FeatureVector.Count - featuresToSkip.Count);
			return distance / featureCount;
		}
		#endregion

		#region Private Methods
		private void computeMeanValuesAndHandConfiguration(List<SGInstance> instances)
		{
			Vec3 LeftPalmPosTotal = new Vec3();
			Vec3 LeftPalmSphereCenterTotal = new Vec3();
			float LeftPalmSphereRadiusTotal = 0;
			float LeftYawTotal = 0;
			float LeftPitchTotal = 0;
			float LeftRollTotal = 0;
			var LeftFingersExtendedTotal = new Dictionary<Finger.FingerType, int>();
			var LeftFingerTipPositionsTotal = new Dictionary<Finger.FingerType, Vec3>();

			Vec3 RightPalmPosTotal = new Vec3();
			Vec3 RightPalmSphereCenterTotal = new Vec3();
			float RightPalmSphereRadiusTotal = 0;
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

			// Keep track of hands' positions relative to each other (left to right)
			Vec3 LeftToRightHandTotal = new Vec3();


			foreach (var instance in instances)
			{
				var leftPalmPos = new Vec3(); // for getting hands positions relative to each other totals
				var rightPalmPos = new Vec3();

				handConfigCounts[instance.HandConfiguration]++;
				foreach (var hand in instance.Hands)
				{
					if (hand.IsLeft)
					{
						leftPalmPos = hand.PalmPosition;
						LeftPalmPosTotal += hand.PalmPosition;
						LeftPalmSphereCenterTotal += hand.PalmSphereCenter;
						LeftPalmSphereRadiusTotal += hand.PalmSphereRadius;
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
						rightPalmPos = hand.PalmPosition;
						RightPalmPosTotal += hand.PalmPosition;
						RightPalmSphereCenterTotal += hand.PalmSphereCenter;
						RightPalmSphereRadiusTotal += hand.PalmSphereRadius;
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

				LeftToRightHandTotal += leftPalmPos - rightPalmPos;
			}

			HandConfiguration = handConfigCounts.FirstOrDefault(h => h.Value == handConfigCounts.Values.Max()).Key;


			MeanValues = new Dictionary<FeatureName, object>();

			float numInstances = instances.Count; // Needs to be float for divisions

			MeanValues.Add(FeatureName.LeftPalmPosition, LeftPalmPosTotal / numInstances);
			MeanValues.Add(FeatureName.LeftPalmSphereCenter, LeftPalmSphereCenterTotal / numInstances);
			MeanValues.Add(FeatureName.LeftPalmSphereRadius, LeftPalmSphereRadiusTotal / numInstances);
			MeanValues.Add(FeatureName.LeftYaw, LeftYawTotal / numInstances);
			MeanValues.Add(FeatureName.LeftPitch, LeftPitchTotal / numInstances);
			MeanValues.Add(FeatureName.LeftRoll, LeftRollTotal / numInstances);

			MeanValues.Add(FeatureName.RightPalmPosition, RightPalmPosTotal / numInstances);
			MeanValues.Add(FeatureName.RightPalmSphereCenter, RightPalmSphereCenterTotal / numInstances);
			MeanValues.Add(FeatureName.RightPalmSphereRadius, RightPalmSphereRadiusTotal / numInstances);
			MeanValues.Add(FeatureName.RightYaw, RightYawTotal / numInstances);
			MeanValues.Add(FeatureName.RightPitch, RightPitchTotal / numInstances);
			MeanValues.Add(FeatureName.RightRoll, RightRollTotal / numInstances);

			var averagedLeftFingerTipPositions = new Dictionary<Finger.FingerType, Vec3>();
			var averagedRightFingerTipPositions = new Dictionary<Finger.FingerType, Vec3>();
			var leftFingersExtendedPercentages = new Dictionary<Finger.FingerType, float>();
			var rightFingersExtendedPercentages = new Dictionary<Finger.FingerType, float>();
			foreach (var fingerType in (Finger.FingerType[])Enum.GetValues(typeof(Finger.FingerType)))
			{
				averagedLeftFingerTipPositions.Add(fingerType, LeftFingerTipPositionsTotal[fingerType] / numInstances);
				averagedRightFingerTipPositions.Add(fingerType, RightFingerTipPositionsTotal[fingerType] / numInstances);
				// Set MeanValue for FingersExtended as a float: The percentage that are true.
				leftFingersExtendedPercentages.Add(fingerType, LeftFingersExtendedTotal[fingerType] / numInstances);
				rightFingersExtendedPercentages.Add(fingerType, RightFingersExtendedTotal[fingerType] / numInstances);
			}
			MeanValues.Add(FeatureName.LeftFingerTipPositions, averagedLeftFingerTipPositions);
			MeanValues.Add(FeatureName.RightFingerTipPositions, averagedRightFingerTipPositions);
			MeanValues.Add(FeatureName.LeftFingersExtended, leftFingersExtendedPercentages);
			MeanValues.Add(FeatureName.RightFingersExtended, rightFingersExtendedPercentages);

			MeanValues.Add(FeatureName.LeftToRightHand, LeftToRightHandTotal / numInstances);
		}

		private void computeStdDevValues(List<SGInstance> instances)
		{
			float LeftPalmPosTotal = 0;
			float LeftYawTotal = 0;
			float LeftPitchTotal = 0;
			float LeftRollTotal = 0;
			float LeftPalmSphereRadiusTotal = 0;
			float LeftPalmSphereCenterTotal = 0;
			var LeftFingerTipPosTotal = new Dictionary<Finger.FingerType, float>();

			float RightPalmPosTotal = 0;
			float RightYawTotal = 0;
			float RightPitchTotal = 0;
			float RightRollTotal = 0;
			float RightPalmSphereRadiusTotal = 0;
			float RightPalmSphereCenterTotal = 0;
			var RightFingerTipPosTotal = new Dictionary<Finger.FingerType, float>();

			float LeftToRightHandTotal = 0;

			foreach (var fingerType in (Finger.FingerType[])Enum.GetValues(typeof(Finger.FingerType)))
			{
				LeftFingerTipPosTotal.Add(fingerType, 0);
				RightFingerTipPosTotal.Add(fingerType, 0);
			}

			foreach (var instance in instances)
			{
				var leftPalmPos = new Vec3();
				var rightPalmPos = new Vec3();

				foreach (var hand in instance.Hands)
				{
					if (hand.IsLeft)
					{
						leftPalmPos = hand.PalmPosition;
						LeftPalmPosTotal += hand.PalmPosition.DistanceTo((Vec3)MeanValues[FeatureName.LeftPalmPosition]);
						LeftYawTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureName.LeftYaw]));
						LeftPitchTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureName.LeftPitch]));
						LeftRollTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureName.LeftRoll]));
						LeftPalmSphereCenterTotal += hand.PalmSphereCenter.DistanceTo((Vec3)MeanValues[FeatureName.RightPalmSphereCenter]);
						LeftPalmSphereRadiusTotal += Math.Abs(hand.PalmSphereRadius - ((float)MeanValues[FeatureName.RightPalmSphereRadius]));
						foreach (var fingerType in (Finger.FingerType[])Enum.GetValues(typeof(Finger.FingerType)))
						{
							LeftFingerTipPosTotal[fingerType] += hand.FingerTipPositions[fingerType].DistanceTo(((Dictionary<Finger.FingerType, Vec3>)MeanValues[FeatureName.LeftFingerTipPositions])[fingerType]);
						}
					}
					else
					{
						rightPalmPos = hand.PalmPosition;
						RightPalmPosTotal += hand.PalmPosition.DistanceTo((Vec3)MeanValues[FeatureName.RightPalmPosition]);;
						RightYawTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureName.RightYaw]));
						RightPitchTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureName.RightPitch]));
						RightRollTotal += Math.Abs(hand.Yaw - ((float)MeanValues[FeatureName.RightRoll]));
						RightPalmSphereCenterTotal += hand.PalmSphereCenter.DistanceTo((Vec3)MeanValues[FeatureName.RightPalmSphereCenter]);
						RightPalmSphereRadiusTotal += Math.Abs(hand.PalmSphereRadius - ((float)MeanValues[FeatureName.RightPalmSphereRadius]));
						foreach (var fingerType in (Finger.FingerType[])Enum.GetValues(typeof(Finger.FingerType)))
						{
							RightFingerTipPosTotal[fingerType] += hand.FingerTipPositions[fingerType].DistanceTo(((Dictionary<Finger.FingerType, Vec3>)MeanValues[FeatureName.RightFingerTipPositions])[fingerType]);
						}
					}
				}

				LeftToRightHandTotal += (leftPalmPos - rightPalmPos).DistanceTo((Vec3)MeanValues[FeatureName.LeftToRightHand]);
			}

			StdDevValues = new Dictionary<FeatureName, object>();

			float numInstances = instances.Count; // Need to be float for divisions

			StdDevValues.Add(FeatureName.LeftPalmPosition, LeftPalmPosTotal / numInstances);
			StdDevValues.Add(FeatureName.LeftYaw, LeftYawTotal / numInstances);
			StdDevValues.Add(FeatureName.LeftPitch, LeftPitchTotal / numInstances);
			StdDevValues.Add(FeatureName.LeftRoll, LeftRollTotal / numInstances);
			StdDevValues.Add(FeatureName.LeftPalmSphereCenter, LeftPalmSphereCenterTotal / numInstances);
			StdDevValues.Add(FeatureName.LeftPalmSphereRadius, LeftPalmSphereRadiusTotal / numInstances);

			StdDevValues.Add(FeatureName.RightPalmPosition, RightPalmPosTotal / numInstances);
			StdDevValues.Add(FeatureName.RightYaw, RightYawTotal / numInstances);
			StdDevValues.Add(FeatureName.RightPitch, RightPitchTotal / numInstances);
			StdDevValues.Add(FeatureName.RightRoll, RightRollTotal / numInstances);
			StdDevValues.Add(FeatureName.RightPalmSphereCenter, RightPalmSphereCenterTotal / numInstances);
			StdDevValues.Add(FeatureName.RightPalmSphereRadius, RightPalmSphereRadiusTotal / numInstances);

			var stdDevRightFingerTipPositions = new Dictionary<Finger.FingerType, float>();
			var stdDevLeftFingerTipPositions = new Dictionary<Finger.FingerType, float>();
			foreach (var fingerType in (Finger.FingerType[])Enum.GetValues(typeof(Finger.FingerType)))
			{
				stdDevRightFingerTipPositions.Add(fingerType, RightFingerTipPosTotal[fingerType] / numInstances);
				stdDevLeftFingerTipPositions.Add(fingerType, LeftFingerTipPosTotal[fingerType] / numInstances);
			}
			StdDevValues.Add(FeatureName.LeftFingerTipPositions, stdDevLeftFingerTipPositions);
			StdDevValues.Add(FeatureName.RightFingerTipPositions, stdDevRightFingerTipPositions);

			StdDevValues.Add(FeatureName.LeftToRightHand, LeftToRightHandTotal / numInstances);
		}

		#endregion

	}
}
