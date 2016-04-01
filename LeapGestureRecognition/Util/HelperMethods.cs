using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using LGR;
using Newtonsoft.Json;

namespace LGR
{
	public class HelperMethods
	{
		public static string GetBoneColorKey(Finger.FingerType fingerType, Bone.BoneType boneType)
		{
			// This must match the constants in Constants.BoneNames
			return FingerTypeToString(fingerType) + " " + BoneTypeToString(boneType);
		}

		public static string FingerTypeToString(Finger.FingerType fingerType)
		{
			string fingerName = fingerType.ToString().Split('_').LastOrDefault().ToLower();
			fingerName = Char.ToUpper(fingerName[0]) + fingerName.Substring(1);
			return fingerName;
		}

		public static string BoneTypeToString(Bone.BoneType boneType)
		{
			string boneName = boneType.ToString().Split('_').LastOrDefault().ToLower();
			boneName = Char.ToUpper(boneName[0]) + boneName.Substring(1);
			return boneName;
		}

		public static float Lerp(float f1, float f2, float amount)
		{
			float delta = (Math.Abs(f1 - f2) * amount) * ((f1 < f2) ? 1 : -1);
			return f1 + delta;
		}


		public static void TestSerializer(SGInstance sgi)
		{
			string serialized = JsonConvert.SerializeObject(sgi);
			SGInstance deserialized = (SGInstance)JsonConvert.DeserializeObject<SGInstance>(serialized);
			deserialized.UpdateFeatureVector();
		}
	}
}
