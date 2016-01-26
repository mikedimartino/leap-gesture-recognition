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
			Hands = new List<SingleHandStaticGesture>();
		}

		public StaticGestureInstance(Frame frame)
		{
			Hands = new List<SingleHandStaticGesture>();
			foreach (var hand in frame.Hands)
			{
				Hands.Add(new SingleHandStaticGesture(hand));
			}

			FeatureVector = buildFeatureVector();

			// Set HandConfiguration
			if (Hands.Count == 0)
			{
				HandConfiguration = HandConfiguration.NoHands;
			}
			else if (Hands.Count == 1)
			{
				if (Hands[0].IsLeft) HandConfiguration = HandConfiguration.LeftHandOnly;
				else HandConfiguration = HandConfiguration.RightHandOnly;
			}
			else
			{
				HandConfiguration = HandConfiguration.BothHands;
			}

		}

		[DataMember]
		public List<SingleHandStaticGesture> Hands { get; set; } // Might not need this eventually, but maybe for displaying live hands
		[DataMember]
		public HandConfiguration HandConfiguration { get; set; }
		[DataMember]
		public List<Feature> FeatureVector { get; set; }


		public StaticGestureInstance DeepCopy()
		{
			string json = JsonConvert.SerializeObject(this);
			return JsonConvert.DeserializeObject<StaticGestureInstance>(json);
		}

		private List<Feature> buildFeatureVector()
		{
			var featureVector = new List<Feature>();
			foreach (var hand in Hands)
			{
				if (hand.IsLeft)
				{
					featureVector.Add(new Feature(FeatureName.LeftPalmPosition, hand.PalmPosition));
					featureVector.Add(new Feature(FeatureName.LeftYaw, hand.Yaw));
					featureVector.Add(new Feature(FeatureName.LeftPitch, hand.Pitch));
					featureVector.Add(new Feature(FeatureName.LeftRoll, hand.Roll));
					featureVector.Add(new Feature(FeatureName.LeftPalmSphereRadius, hand.PalmSphereRadius));
					featureVector.Add(new Feature(FeatureName.LeftPalmSphereCenter, hand.PalmSphereCenter));

					featureVector.Add(new Feature(FeatureName.LeftFingerTipPositions, hand.FingerTipPositions));
					featureVector.Add(new Feature(FeatureName.LeftFingersExtended, hand.FingersExtended));
					
				}
				else
				{
					featureVector.Add(new Feature(FeatureName.RightPalmPosition, hand.PalmPosition));
					featureVector.Add(new Feature(FeatureName.RightYaw, hand.Yaw));
					featureVector.Add(new Feature(FeatureName.RightPitch, hand.Pitch));
					featureVector.Add(new Feature(FeatureName.RightRoll, hand.Roll));
					featureVector.Add(new Feature(FeatureName.RightPalmSphereRadius, hand.PalmSphereRadius));
					featureVector.Add(new Feature(FeatureName.RightPalmSphereCenter, hand.PalmSphereCenter));

					featureVector.Add(new Feature(FeatureName.RightFingerTipPositions, hand.FingerTipPositions));
					featureVector.Add(new Feature(FeatureName.RightFingersExtended, hand.FingersExtended));
					
				}
			}

			return featureVector;
		}

	}
}
