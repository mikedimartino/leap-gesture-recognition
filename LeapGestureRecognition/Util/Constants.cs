using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LeapGestureRecognition.Util
{
	public static class Constants
	{
		public static double FingerTipRadius = 3;
		public static double PalmSphereRadius = 8;
		public static double WristSphereRadius = 8;

		public static Color DefaultColor = Colors.White;

		public static Dictionary<Bone.BoneType, Color> BoneColors = new Dictionary<Bone.BoneType, Color>() 
		{
			{ Bone.BoneType.TYPE_DISTAL, Colors.LightSalmon },
			{ Bone.BoneType.TYPE_INTERMEDIATE, Colors.CornflowerBlue },
			{ Bone.BoneType.TYPE_METACARPAL, Colors.Green },
			{ Bone.BoneType.TYPE_PROXIMAL, Colors.Yellow },
		};

		public static string SQLiteFileName = "gesture_store.sqlite";
	}
}
