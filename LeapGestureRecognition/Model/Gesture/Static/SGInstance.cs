using Leap;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LGR
{
	[DataContract]
	public class SGInstance
	{
		#region Public Properties
		[DataMember]
		public List<SGInstanceSingleHand> Hands { get; set; }
		[DataMember]
		public SGInstanceSingleHand LeftHand { get; set; }
		[DataMember]
		public SGInstanceSingleHand RightHand { get; set; }
		[DataMember]
		public HandConfiguration HandConfiguration { get; set; }
		[DataMember]
		public List<Feature> FeatureVector { get; set; }
		//[DataMember]
		public Dictionary<FeatureName, Feature> Features { get; set; }
		#endregion

		#region Constructors
		public SGInstance()
		{
			Hands = new List<SGInstanceSingleHand>();
			FeatureVector = new List<Feature>();
			Features = new Dictionary<FeatureName, Feature>();
		}

		public SGInstance(Frame frame)
		{
			Hands = new List<SGInstanceSingleHand>();
			foreach (var leapHand in frame.Hands)
			{
				var hand = new SGInstanceSingleHand(leapHand);
				if (leapHand.IsLeft) LeftHand = hand;
				else RightHand = hand;
				Hands.Add(hand);
			}

			FeatureVector = buildFeatureVector();

			Features = new Dictionary<FeatureName, Feature>();
			foreach (var feature in FeatureVector)
			{
				Features.Add(feature.Name, feature);
			}

			HandConfiguration = getHandConfiguration(Hands);

			if (RightHand != null)
			{
				LeapGestureRecognition.ViewModel.MainViewModel.ClearOutputWindow();
				LeapGestureRecognition.ViewModel.MainViewModel.WriteLineToOutputWindow("Pitch = " + RightHand.Pitch);
				LeapGestureRecognition.ViewModel.MainViewModel.WriteLineToOutputWindow("Yaw = " + RightHand.Yaw);
				LeapGestureRecognition.ViewModel.MainViewModel.WriteLineToOutputWindow("Roll = " + RightHand.Roll);
			}
		}

		public SGInstance(List<Feature> featureVector)
		{
			FeatureVector = featureVector;

			Features = new Dictionary<FeatureName, Feature>();
			foreach (var feature in FeatureVector)
			{
				Features.Add(feature.Name, feature);
			}

			if (Features.ContainsKey(FeatureName.HandConfiguration))
			{
				HandConfiguration = (HandConfiguration)Features[FeatureName.HandConfiguration].Value;
			}

			//Hands = new List<StaticGestureInstanceSingleHand>();
			//if(HandConfiguration == HandConfiguration.LeftHandOnly || HandConfiguration == HandConfiguration.BothHands) {
			//	Hands.Add(new StaticGestureInstanceSingleHand(){ IsLeft = true });
			//}
			//else if(HandConfiguration == HandConfiguration.RightHandOnly || HandConfiguration == HandConfiguration.BothHands) {
			//	Hands.Add(new StaticGestureInstanceSingleHand(){ IsRight = true });
			//}
			//foreach(var feature)
		}
		#endregion



		HashSet<FeatureName> featuresToSkip = new HashSet<FeatureName>()
		{
			FeatureName.LeftPalmPosition,
			FeatureName.RightPalmPosition,
			FeatureName.LeftPalmSphereCenter,
			FeatureName.RightPalmSphereCenter,
		};

		public float DistanceTo(SGInstance otherInstance)
		{
			// Check if same hand configuration (BothHands, LeftHandOnly, RightHandOnly)...
			if (HandConfiguration != otherInstance.HandConfiguration) return Single.PositiveInfinity;

			float distance = 0;
			int featureCount = 0; // Necessary to do this because some features (like Dictionary's) are really 5 features.
			foreach (var feature in otherInstance.FeatureVector)
			{
				if (featuresToSkip.Contains(feature.Name)) continue;

				if (feature.Value is Vec3)
				{
					distance += feature.Weight * ((Vec3)(feature.Value)).DistanceTo((Vec3)Features[feature.Name].Value);
					featureCount += feature.Weight;
				}
				else if (feature.Value is float)
				{
					// float values include Yaw, Pitch, Roll, and Sphere Radius
					distance += feature.Weight * Math.Abs((float)feature.Value - (float)Features[feature.Name].Value);
					featureCount += feature.Weight;
				}
				else if (feature.Value is Dictionary<Finger.FingerType, Vec3>)
				{
					// This is just for fingersTipPositions
					Dictionary<Finger.FingerType, Vec3> thisHandFingersTipPositions = (Dictionary<Finger.FingerType, Vec3>)(Features[feature.Name].Value);
					foreach (var fingerTipPosition in (Dictionary<Finger.FingerType, Vec3>)feature.Value)
					{
						//var theFingerTipPositions = ((Newtonsoft.Json.Linq.JObject)Features[feature.Name].Value).ToObject<Dictionary<Finger.FingerType, Vec3>>();
						Finger.FingerType fingerType = fingerTipPosition.Key;
						Vec3 instancePos = fingerTipPosition.Value;
						distance += feature.Weight * (instancePos).DistanceTo(thisHandFingersTipPositions[fingerType]);
						featureCount += feature.Weight;
					}
				}
				else if (feature.Value is Dictionary<Finger.FingerType, bool>)
				{
					// This is just for fingers.IsExtended
					Dictionary<Finger.FingerType, bool> thisHandFingersExtended = (Dictionary<Finger.FingerType, bool>)(Features[feature.Name].Value);
					foreach (var fingerExtended in (Dictionary<Finger.FingerType, bool>)feature.Value)
					{
						Finger.FingerType fingerType = fingerExtended.Key;
						bool isExtended = fingerExtended.Value;
						if (thisHandFingersExtended[fingerType]) // finger should be extended
						{
							if (!isExtended) distance += feature.Weight; //distance += fingerExtendedPercentage;
						}
						else
						{
							if (isExtended) distance += feature.Weight; //distance += 1 - fingerExtendedPercentage;
						}
						featureCount += feature.Weight;
					}
				}
			}
			//return distance / (gestureInstance.FeatureVector.Count - featuresToSkip.Count);
			return distance / featureCount;
		}

		public float DistanceTo(SGClass gestureClass)
		{
			return gestureClass.DistanceTo(this);
		}


		#region old lerp
		//public StaticGestureInstance Lerp(StaticGestureInstance otherInstance, float amount)
		//{
		//	if (this.HandConfiguration != otherInstance.HandConfiguration) return this; // Maybe throw an exception or handle another way.


		//	var lerpedFeatureVector = new List<Feature>();

		//	foreach (var feature in FeatureVector)
		//	{ 
		//		if (feature.Value is Vec3)
		//		{
		//			Vec3 lerped = ((Vec3)feature.Value).Lerp((Vec3)otherInstance.Features[feature.Name].Value, amount);
		//			lerpedFeatureVector.Add(new Feature(feature.Name, lerped, feature.Weight));
		//		}
		//		else if (feature.Value is float)
		//		{
		//			float lerped = HelperMethods.Lerp((float)feature.Value, (float)otherInstance.Features[feature.Name].Value, amount);
		//			lerpedFeatureVector.Add(new Feature(feature.Name, lerped, feature.Weight));
		//		}
		//		else if (feature.Value is Dictionary<Finger.FingerType, Vec3>)
		//		{
		//			// This is just for fingersTipPositions
		//			Dictionary<Finger.FingerType, Vec3> thisHandFingersTipPositions = (Dictionary<Finger.FingerType, Vec3>)(Features[feature.Name].Value);
		//			var lerpedTipPositions = new Dictionary<Finger.FingerType, Vec3>();
		//			foreach (var fingerTipPosition in (Dictionary<Finger.FingerType, Vec3>)feature.Value)
		//			{
		//				//var theFingerTipPositions = ((Newtonsoft.Json.Linq.JObject)Features[feature.Name].Value).ToObject<Dictionary<Finger.FingerType, Vec3>>();
		//				Finger.FingerType fingerType = fingerTipPosition.Key;
		//				Vec3 otherPos = fingerTipPosition.Value;
		//				Vec3 lerpedTip = thisHandFingersTipPositions[fingerType].Lerp(otherPos, amount);
		//				lerpedTipPositions.Add(fingerType, lerpedTip);
		//			}
		//			lerpedFeatureVector.Add(new Feature(feature.Name, lerpedTipPositions));
		//		}
		//		else if (feature.Value is Dictionary<Finger.FingerType, bool>)
		//		{
		//			// This is just for fingers.IsExtended // Don't do any lerping for now.
		//			//lerpedFeatureVector.Add(new Feature(feature.Name, feature.Value));
		//		}
		//		else if(feature.Value is HandConfiguration) 
		//		{
		//			lerpedFeatureVector.Add(new Feature(FeatureName.HandConfiguration, feature.Value));
		//		}
		//	}

		//	var lerpedInstance = new StaticGestureInstance(lerpedFeatureVector); // TODO: Create constructor for this.

		//	return lerpedInstance;
		//}
		#endregion

		public SGInstance Lerp(SGInstance otherInstance, float amount)
		{
			if (this.HandConfiguration != otherInstance.HandConfiguration) return this; // Maybe throw an exception or handle another way.

			var lerpedSGI = this.DeepCopy(); // Just to avoid any potential null errors. Make first hand value the default.
			//var lerpedSGI = new StaticGestureInstance();

			lerpedSGI.Hands.Clear();

			#region Left Hand
			if (LeftHand != null)
			{
				lerpedSGI.LeftHand.ArmX = LeftHand.ArmX.Lerp(otherInstance.LeftHand.ArmX, amount);
				lerpedSGI.LeftHand.ArmY = LeftHand.ArmY.Lerp(otherInstance.LeftHand.ArmY, amount);
				lerpedSGI.LeftHand.ArmZ = LeftHand.ArmZ.Lerp(otherInstance.LeftHand.ArmZ, amount);
				lerpedSGI.LeftHand.ElbowPos_Relative = LeftHand.ElbowPos_Relative.Lerp(otherInstance.LeftHand.ElbowPos_Relative, amount);
				lerpedSGI.LeftHand.ElbowPos_World = LeftHand.ElbowPos_World.Lerp(otherInstance.LeftHand.ElbowPos_World, amount);

				foreach(var fingerType in (Finger.FingerType[])Enum.GetValues(typeof(Finger.FingerType))) 
				{
					foreach(var jointType in (Finger.FingerJoint[])Enum.GetValues(typeof(Finger.FingerJoint))) 
					{
						var fjp_rel1 = LeftHand.FingerJointPositions_Relative[fingerType][jointType];
						var fjp_rel2 = otherInstance.LeftHand.FingerJointPositions_Relative[fingerType][jointType];
						lerpedSGI.LeftHand.FingerJointPositions_Relative[fingerType][jointType] = fjp_rel1.Lerp(fjp_rel2, amount);

						var fjp_world1 = LeftHand.FingerJointPositions_World[fingerType][jointType];
						var fjp_world2 = otherInstance.LeftHand.FingerJointPositions_World[fingerType][jointType];
						lerpedSGI.LeftHand.FingerJointPositions_World[fingerType][jointType] = fjp_world1.Lerp(fjp_world2, amount);
					}
					lerpedSGI.LeftHand.FingerTipPositions[fingerType] = LeftHand.FingerTipPositions[fingerType].Lerp(otherInstance.LeftHand.FingerTipPositions[fingerType], amount);
				}

				lerpedSGI.LeftHand.ForearmCenter_Relative = LeftHand.ForearmCenter_Relative.Lerp(otherInstance.LeftHand.ForearmCenter_Relative, amount);
				lerpedSGI.LeftHand.ForearmCenter_World = LeftHand.ForearmCenter_World.Lerp(otherInstance.LeftHand.ForearmCenter_World, amount);
				lerpedSGI.LeftHand.HandDirection = LeftHand.HandDirection.Lerp(otherInstance.LeftHand.HandDirection, amount);

				// Do I need to worry about HandTransform? Hopefully not.
				
				lerpedSGI.LeftHand.IndexBasePos_Relative = LeftHand.IndexBasePos_Relative.Lerp(otherInstance.LeftHand.IndexBasePos_Relative, amount);
				lerpedSGI.LeftHand.MiddleBasePos_Relative = LeftHand.MiddleBasePos_Relative.Lerp(otherInstance.LeftHand.MiddleBasePos_Relative, amount);
				lerpedSGI.LeftHand.RingBasePos_Relative = LeftHand.RingBasePos_Relative.Lerp(otherInstance.LeftHand.RingBasePos_Relative, amount);
				lerpedSGI.LeftHand.PinkyBasePos_Relative = LeftHand.PinkyBasePos_Relative.Lerp(otherInstance.LeftHand.PinkyBasePos_Relative, amount);
				lerpedSGI.LeftHand.IndexBasePos_World = LeftHand.IndexBasePos_World.Lerp(otherInstance.LeftHand.IndexBasePos_World, amount);
				lerpedSGI.LeftHand.MiddleBasePos_World = LeftHand.MiddleBasePos_World.Lerp(otherInstance.LeftHand.MiddleBasePos_World, amount);
				lerpedSGI.LeftHand.RingBasePos_World = LeftHand.RingBasePos_World.Lerp(otherInstance.LeftHand.RingBasePos_World, amount);
				lerpedSGI.LeftHand.PinkyBasePos_World = LeftHand.PinkyBasePos_World.Lerp(otherInstance.LeftHand.PinkyBasePos_World, amount);

				lerpedSGI.LeftHand.PalmNormal = LeftHand.PalmNormal.Lerp(otherInstance.LeftHand.PalmNormal, amount);
				lerpedSGI.LeftHand.PalmPosition = LeftHand.PalmPosition.Lerp(otherInstance.LeftHand.PalmPosition, amount);
				lerpedSGI.LeftHand.PalmSphereCenter = LeftHand.PalmSphereCenter.Lerp(otherInstance.LeftHand.PalmSphereCenter, amount);
				lerpedSGI.LeftHand.PalmSphereRadius = HelperMethods.Lerp(LeftHand.PalmSphereRadius, otherInstance.LeftHand.PalmSphereRadius, amount);

				lerpedSGI.LeftHand.Pitch = HelperMethods.Lerp(LeftHand.Pitch, otherInstance.LeftHand.Pitch, amount);
				lerpedSGI.LeftHand.Roll = HelperMethods.Lerp(LeftHand.Roll, otherInstance.LeftHand.Roll, amount);
				lerpedSGI.LeftHand.Yaw = HelperMethods.Lerp(LeftHand.Yaw, otherInstance.LeftHand.Yaw, amount);

				lerpedSGI.LeftHand.WristPos_Relative = LeftHand.WristPos_Relative.Lerp(otherInstance.LeftHand.WristPos_Relative, amount);
				lerpedSGI.LeftHand.WristPos_World = LeftHand.WristPos_World.Lerp(otherInstance.LeftHand.WristPos_World, amount);

				lerpedSGI.Hands.Add(lerpedSGI.LeftHand);
			}
			#endregion

			#region Right Hand
			if (RightHand != null)
			{
				lerpedSGI.RightHand.ArmX = RightHand.ArmX.Lerp(otherInstance.RightHand.ArmX, amount);
				lerpedSGI.RightHand.ArmY = RightHand.ArmY.Lerp(otherInstance.RightHand.ArmY, amount);
				lerpedSGI.RightHand.ArmZ = RightHand.ArmZ.Lerp(otherInstance.RightHand.ArmZ, amount);
				lerpedSGI.RightHand.ElbowPos_Relative = RightHand.ElbowPos_Relative.Lerp(otherInstance.RightHand.ElbowPos_Relative, amount);
				lerpedSGI.RightHand.ElbowPos_World = RightHand.ElbowPos_World.Lerp(otherInstance.RightHand.ElbowPos_World, amount);

				foreach (var fingerType in (Finger.FingerType[])Enum.GetValues(typeof(Finger.FingerType)))
				{
					foreach (var jointType in (Finger.FingerJoint[])Enum.GetValues(typeof(Finger.FingerJoint)))
					{
						var fjp_rel1 = RightHand.FingerJointPositions_Relative[fingerType][jointType];
						var fjp_rel2 = otherInstance.RightHand.FingerJointPositions_Relative[fingerType][jointType];
						lerpedSGI.RightHand.FingerJointPositions_Relative[fingerType][jointType] = fjp_rel1.Lerp(fjp_rel2, amount);

						var fjp_world1 = RightHand.FingerJointPositions_World[fingerType][jointType];
						var fjp_world2 = otherInstance.RightHand.FingerJointPositions_World[fingerType][jointType];
						lerpedSGI.RightHand.FingerJointPositions_World[fingerType][jointType] = fjp_world1.Lerp(fjp_world2, amount);
					}
					lerpedSGI.RightHand.FingerTipPositions[fingerType] = RightHand.FingerTipPositions[fingerType].Lerp(otherInstance.RightHand.FingerTipPositions[fingerType], amount);
				}

				lerpedSGI.RightHand.ForearmCenter_Relative = RightHand.ForearmCenter_Relative.Lerp(otherInstance.RightHand.ForearmCenter_Relative, amount);
				lerpedSGI.RightHand.ForearmCenter_World = RightHand.ForearmCenter_World.Lerp(otherInstance.RightHand.ForearmCenter_World, amount);
				lerpedSGI.RightHand.HandDirection = RightHand.HandDirection.Lerp(otherInstance.RightHand.HandDirection, amount);

				// Do I need to worry about HandTransform? Hopefully not.

				lerpedSGI.RightHand.IndexBasePos_Relative = RightHand.IndexBasePos_Relative.Lerp(otherInstance.RightHand.IndexBasePos_Relative, amount);
				lerpedSGI.RightHand.MiddleBasePos_Relative = RightHand.MiddleBasePos_Relative.Lerp(otherInstance.RightHand.MiddleBasePos_Relative, amount);
				lerpedSGI.RightHand.RingBasePos_Relative = RightHand.RingBasePos_Relative.Lerp(otherInstance.RightHand.RingBasePos_Relative, amount);
				lerpedSGI.RightHand.PinkyBasePos_Relative = RightHand.PinkyBasePos_Relative.Lerp(otherInstance.RightHand.PinkyBasePos_Relative, amount);
				lerpedSGI.RightHand.IndexBasePos_World = RightHand.IndexBasePos_World.Lerp(otherInstance.RightHand.IndexBasePos_World, amount);
				lerpedSGI.RightHand.MiddleBasePos_World = RightHand.MiddleBasePos_World.Lerp(otherInstance.RightHand.MiddleBasePos_World, amount);
				lerpedSGI.RightHand.RingBasePos_World = RightHand.RingBasePos_World.Lerp(otherInstance.RightHand.RingBasePos_World, amount);
				lerpedSGI.RightHand.PinkyBasePos_World = RightHand.PinkyBasePos_World.Lerp(otherInstance.RightHand.PinkyBasePos_World, amount);

				lerpedSGI.RightHand.PalmNormal = RightHand.PalmNormal.Lerp(otherInstance.RightHand.PalmNormal, amount);
				lerpedSGI.RightHand.PalmPosition = RightHand.PalmPosition.Lerp(otherInstance.RightHand.PalmPosition, amount);
				lerpedSGI.RightHand.PalmSphereCenter = RightHand.PalmSphereCenter.Lerp(otherInstance.RightHand.PalmSphereCenter, amount);
				lerpedSGI.RightHand.PalmSphereRadius = HelperMethods.Lerp(RightHand.PalmSphereRadius, otherInstance.RightHand.PalmSphereRadius, amount);

				lerpedSGI.RightHand.Pitch = HelperMethods.Lerp(RightHand.Pitch, otherInstance.RightHand.Pitch, amount);
				lerpedSGI.RightHand.Roll = HelperMethods.Lerp(RightHand.Roll, otherInstance.RightHand.Roll, amount);
				lerpedSGI.RightHand.Yaw = HelperMethods.Lerp(RightHand.Yaw, otherInstance.RightHand.Yaw, amount);

				lerpedSGI.RightHand.WristPos_Relative = RightHand.WristPos_Relative.Lerp(otherInstance.RightHand.WristPos_Relative, amount);
				lerpedSGI.RightHand.WristPos_World = RightHand.WristPos_World.Lerp(otherInstance.RightHand.WristPos_World, amount);

				lerpedSGI.Hands.Add(lerpedSGI.RightHand);
			}
			#endregion

			lerpedSGI.UpdateFeatureVector();

			return lerpedSGI;
		}

		public SGInstance DeepCopy()
		{
			string serialized = JsonConvert.SerializeObject(this);
			SGInstance deserialized = JsonConvert.DeserializeObject<SGInstance>(serialized);
			//deserialized.UpdateFeatureVector();
			return deserialized;
		}

		public void UpdateFeatureVector()
		{
			FeatureVector = buildFeatureVector();
			Features = new Dictionary<FeatureName, Feature>();
			foreach (var feature in FeatureVector)
			{
				Features.Add(feature.Name, feature);
			}
		}

		private List<Feature> buildFeatureVector()
		{
			var featureVector = new List<Feature>();
			var leftPalmPos = new Vec3();
			var rightPalmPos = new Vec3();

			foreach (var hand in Hands)
			{
				if (hand.IsLeft)
				{
					featureVector.Add(new Feature(FeatureName.LeftPalmPosition, hand.PalmPosition));
					featureVector.Add(new Feature(FeatureName.LeftYaw, hand.Yaw, 1));
					featureVector.Add(new Feature(FeatureName.LeftPitch, hand.Pitch, 1));
					featureVector.Add(new Feature(FeatureName.LeftRoll, hand.Roll, 1));
					featureVector.Add(new Feature(FeatureName.LeftPalmSphereRadius, hand.PalmSphereRadius));
					featureVector.Add(new Feature(FeatureName.LeftPalmSphereCenter, hand.PalmSphereCenter));
					featureVector.Add(new Feature(FeatureName.LeftFingerTipPositions, hand.FingerTipPositions));
					featureVector.Add(new Feature(FeatureName.LeftFingersExtended, hand.FingersExtended));
					
					leftPalmPos = hand.PalmPosition;
				}
				else
				{
					featureVector.Add(new Feature(FeatureName.RightPalmPosition, hand.PalmPosition));
					featureVector.Add(new Feature(FeatureName.RightYaw, hand.Yaw, 1));
					featureVector.Add(new Feature(FeatureName.RightPitch, hand.Pitch, 1));
					featureVector.Add(new Feature(FeatureName.RightRoll, hand.Roll, 1));
					featureVector.Add(new Feature(FeatureName.RightPalmSphereRadius, hand.PalmSphereRadius));
					featureVector.Add(new Feature(FeatureName.RightPalmSphereCenter, hand.PalmSphereCenter));
					featureVector.Add(new Feature(FeatureName.RightFingerTipPositions, hand.FingerTipPositions));
					featureVector.Add(new Feature(FeatureName.RightFingersExtended, hand.FingersExtended));

					rightPalmPos = hand.PalmPosition;
				}
			}

			featureVector.Add(new Feature(FeatureName.HandConfiguration, getHandConfiguration(Hands)));

			// Determine hands' positioning relative to each other (left to right):
			if (Hands.Count == 2)
			{
				featureVector.Add(new Feature(FeatureName.LeftToRightHand, leftPalmPos - rightPalmPos));
			}

			return featureVector;
		}

		private HandConfiguration getHandConfiguration(List<SGInstanceSingleHand> hands)
		{
			HandConfiguration config;
			if (Hands.Count == 0)
			{
				config = HandConfiguration.NoHands;
			}
			else if (Hands.Count == 1)
			{
				if (Hands[0].IsLeft) config = HandConfiguration.LeftHandOnly;
				else config = HandConfiguration.RightHandOnly;
			}
			else
			{
				config = HandConfiguration.BothHands;
			}
			return config;
		}

	}
}
