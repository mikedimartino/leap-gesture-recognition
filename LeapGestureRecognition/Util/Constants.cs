using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LeapGestureRecognition
{
	public static class Constants
	{
		public static string SQLiteFileName = "lgr_db.sqlite";

		public static double FingerTipRadius = 3;
		public static double PalmSphereRadius = 8;
		public static double WristSphereRadius = 8;

		public static Color DefaultColor = Colors.White;

		public static int FrameRate = 30;

		public static Brush ActiveTabBrush = Brushes.LightBlue;

		public static float StaticRecognitionDistance = 3.0f;
		public static float DynamicRecognitionDistance = 3.0f;
		public static Brush RecognizedRowBackgroundColor = Brushes.Yellow;

		public static Dictionary<string, Color> DefaultBoneColors = new Dictionary<string, Color>() 
		{
			{ BoneNames.Palm, (Color)ColorConverter.ConvertFromString("#FF008000") },
			{ BoneNames.Wrist, (Color)ColorConverter.ConvertFromString("#FF008000") },
			{ BoneNames.ForearmCenter, (Color)ColorConverter.ConvertFromString("#FF00FFFF") },
			{ BoneNames.Elbow, (Color)ColorConverter.ConvertFromString("#FF00FFFF") },
			{ BoneNames.Arm, (Color)ColorConverter.ConvertFromString("#FF800080") },
			{ BoneNames.BaseOfHand, (Color)ColorConverter.ConvertFromString("#FFDAA520") },
			{ BoneNames.Pinky_Distal, (Color)ColorConverter.ConvertFromString("#FFFF0000") },
			{ BoneNames.Pinky_Intermediate, (Color)ColorConverter.ConvertFromString("#FF0000CD") },
			{ BoneNames.Pinky_Proximal, (Color)ColorConverter.ConvertFromString("#FF008000") },
			{ BoneNames.Pinky_Metacarpal, (Color)ColorConverter.ConvertFromString("#FFFFFF00") },
			{ BoneNames.Ring_Distal, (Color)ColorConverter.ConvertFromString("#FFFF0000") },
			{ BoneNames.Ring_Intermediate, (Color)ColorConverter.ConvertFromString("#FF0000CD") },
			{ BoneNames.Ring_Proximal, (Color)ColorConverter.ConvertFromString("#FF008000") },
			{ BoneNames.Ring_Metacarpal, (Color)ColorConverter.ConvertFromString("#FFFFFF00") },
			{ BoneNames.Middle_Distal, (Color)ColorConverter.ConvertFromString("#FFFF0000") },
			{ BoneNames.Middle_Intermediate, (Color)ColorConverter.ConvertFromString("#FF0000CD") },
			{ BoneNames.Middle_Proximal, (Color)ColorConverter.ConvertFromString("#FF008000") },
			{ BoneNames.Middle_Metacarpal, (Color)ColorConverter.ConvertFromString("#FFFFFF00") },
			{ BoneNames.Index_Distal, (Color)ColorConverter.ConvertFromString("#FFFF0000") },
			{ BoneNames.Index_Intermediate, (Color)ColorConverter.ConvertFromString("#FF0000CD") },
			{ BoneNames.Index_Proximal, (Color)ColorConverter.ConvertFromString("#FF008000") },
			{ BoneNames.Index_Metacarpal, (Color)ColorConverter.ConvertFromString("#FFFFFF00") },
			{ BoneNames.Thumb_Distal, (Color)ColorConverter.ConvertFromString("#FFFF0000") },
			{ BoneNames.Thumb_Intermediate, (Color)ColorConverter.ConvertFromString("#FF0000CD") },
			{ BoneNames.Thumb_Proximal, (Color)ColorConverter.ConvertFromString("#FF008000") },
		};

		// Meant for the BoneColors table. Some aren't technically bones, like palm sphere, wrist sphere, etc..
		public static class BoneNames
		{
			public const string Palm = "Palm";
			public const string Wrist = "Wrist";
			public const string ForearmCenter = "Forearm Center";
			public const string Elbow = "Elbow";
			public const string Arm = "Arm";
			public const string BaseOfHand = "Base Of Hand";
			public const string Pinky_Distal = "Pinky Distal";
			public const string Pinky_Intermediate = "Pinky Intermediate";
			public const string Pinky_Proximal = "Pinky Proximal";
			public const string Pinky_Metacarpal = "Pinky Metacarpal";
			public const string Ring_Distal = "Ring Distal";
			public const string Ring_Intermediate = "Ring Intermediate";
			public const string Ring_Proximal = "Ring Proximal";
			public const string Ring_Metacarpal = "Ring Metacarpal";
			public const string Middle_Distal = "Middle Distal";
			public const string Middle_Intermediate = "Middle Intermediate";
			public const string Middle_Proximal = "Middle Proximal";
			public const string Middle_Metacarpal = "Middle Metacarpal";
			public const string Index_Distal = "Index Distal";
			public const string Index_Intermediate = "Index Intermediate";
			public const string Index_Proximal = "Index Proximal";
			public const string Index_Metacarpal = "Index Metacarpal";
			public const string Thumb_Distal = "Thumb Distal";
			public const string Thumb_Intermediate = "Thumb Intermediate";
			public const string Thumb_Proximal = "Thumb Proximal";
		}

		public static Dictionary<string, bool> DefaultBoolOptions = new Dictionary<string, bool>()
		{
			{ BoolOptionsNames.ShowAxes, true },
			{ BoolOptionsNames.ShowArms, true },
			{ BoolOptionsNames.UseDTW, false },
		};

		public static class BoolOptionsNames
		{
			public const string ShowAxes = "Show Axes";
			public const string ShowArms = "Show Arms";
			public const string UseDTW = "Use DTW";
		}

	}
}
