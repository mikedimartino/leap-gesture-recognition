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
	public class StaticGestureInstance
	{
		public StaticGestureInstance()
		{
			Hands = new List<StaticGestureInstanceSingleHand>();
		}

		public StaticGestureInstance(Frame frame)
		{
			Hands = new List<StaticGestureInstanceSingleHand>();
			foreach (var leapHand in frame.Hands)
			{
				var hand = new StaticGestureInstanceSingleHand(leapHand);
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
		}

		[DataMember]
		public List<StaticGestureInstanceSingleHand> Hands { get; set; }
		[DataMember]
		public StaticGestureInstanceSingleHand LeftHand { get; set; }
		[DataMember]
		public StaticGestureInstanceSingleHand RightHand { get; set; }
		[DataMember]
		public HandConfiguration HandConfiguration { get; set; }
		[DataMember]
		public List<Feature> FeatureVector { get; set; }
		[DataMember]
		public Dictionary<FeatureName, Feature> Features { get; set; }

		public StaticGestureInstance DeepCopy()
		{
			string json = JsonConvert.SerializeObject(this);
			return JsonConvert.DeserializeObject<StaticGestureInstance>(json);
		}

		HashSet<FeatureName> featuresToSkip = new HashSet<FeatureName>()
		{
			FeatureName.LeftPalmPosition,
			FeatureName.RightPalmPosition,
			FeatureName.LeftPalmSphereCenter,
			FeatureName.RightPalmSphereCenter,
		};

		public float DistanceTo(StaticGestureInstance otherInstance)
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
					featureVector.Add(new Feature(FeatureName.LeftYaw, hand.Yaw, 3));
					featureVector.Add(new Feature(FeatureName.LeftPitch, hand.Pitch, 3));
					featureVector.Add(new Feature(FeatureName.LeftRoll, hand.Roll, 3));
					featureVector.Add(new Feature(FeatureName.LeftPalmSphereRadius, hand.PalmSphereRadius));
					featureVector.Add(new Feature(FeatureName.LeftPalmSphereCenter, hand.PalmSphereCenter));

					featureVector.Add(new Feature(FeatureName.LeftFingerTipPositions, hand.FingerTipPositions));
					featureVector.Add(new Feature(FeatureName.LeftFingersExtended, hand.FingersExtended));
					
					leftPalmPos = hand.PalmPosition;
				}
				else
				{
					featureVector.Add(new Feature(FeatureName.RightPalmPosition, hand.PalmPosition));
					featureVector.Add(new Feature(FeatureName.RightYaw, hand.Yaw, 3));
					featureVector.Add(new Feature(FeatureName.RightPitch, hand.Pitch, 3));
					featureVector.Add(new Feature(FeatureName.RightRoll, hand.Roll, 3));
					featureVector.Add(new Feature(FeatureName.RightPalmSphereRadius, hand.PalmSphereRadius));
					featureVector.Add(new Feature(FeatureName.RightPalmSphereCenter, hand.PalmSphereCenter));

					featureVector.Add(new Feature(FeatureName.RightFingerTipPositions, hand.FingerTipPositions));
					featureVector.Add(new Feature(FeatureName.RightFingersExtended, hand.FingersExtended));

					rightPalmPos = hand.PalmPosition;
				}
			}

			// Determine hands' positioning relative to each other (left to right)
			if (Hands.Count == 2)
			{
				featureVector.Add(new Feature(FeatureName.LeftToRightHand, leftPalmPos - rightPalmPos));
			}

			return featureVector;
		}

		private HandConfiguration getHandConfiguration(List<StaticGestureInstanceSingleHand> hands)
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
