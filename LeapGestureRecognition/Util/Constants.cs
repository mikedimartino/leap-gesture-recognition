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
		public static string SQLiteFileName = "gesture_store.sqlite";

		public static double FingerTipRadius = 3;
		public static double PalmSphereRadius = 8;
		public static double WristSphereRadius = 8;

		public static Color DefaultColor = Colors.White;

		public static int FrameRate = 30;

		public static Dictionary<string, Color> DefaultBoneColors = new Dictionary<string, Color>() 
		{
			{ BoneNames.Palm, Colors.White },
			{ BoneNames.Wrist, Colors.White },
			{ BoneNames.ForearmCenter, Colors.White },
			{ BoneNames.Elbow, Colors.White },
			{ BoneNames.Arm, Colors.White },
			{ BoneNames.BaseOfHand, Colors.White },
			{ BoneNames.Pinky_Distal, Colors.White },
			{ BoneNames.Pinky_Intermediate, Colors.White },
			{ BoneNames.Pinky_Proximal, Colors.White },
			{ BoneNames.Pinky_Metacarpal, Colors.White },
			{ BoneNames.Ring_Distal, Colors.White },
			{ BoneNames.Ring_Intermediate, Colors.White },
			{ BoneNames.Ring_Proximal, Colors.White },
			{ BoneNames.Ring_Metacarpal, Colors.White },
			{ BoneNames.Middle_Distal, Colors.White },
			{ BoneNames.Middle_Intermediate, Colors.White },
			{ BoneNames.Middle_Proximal, Colors.White },
			{ BoneNames.Middle_Metacarpal, Colors.White },
			{ BoneNames.Index_Distal, Colors.White },
			{ BoneNames.Index_Intermediate, Colors.White },
			{ BoneNames.Index_Proximal, Colors.White },
			{ BoneNames.Index_Metacarpal, Colors.White },
			{ BoneNames.Thumb_Distal, Colors.White },
			{ BoneNames.Thumb_Intermediate, Colors.White },
			{ BoneNames.Thumb_Proximal, Colors.White },
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
			{ BoolOptionsNames.ShowGestureLibrary, true },
			{ BoolOptionsNames.ShowOutputWindow, true }
		};

		public static class BoolOptionsNames
		{
			public const string ShowAxes = "Show Axes";
			public const string ShowArms = "Show Arms";
			public const string ShowGestureLibrary = "Show Gesture Library";
			public const string ShowOutputWindow = "Show Output Window";
		}


		public static Dictionary<string, string> DefaultStringOptions = new Dictionary<string, string>()
		{
			{ StringOptionsNames.ActiveUser, "NO ACTIVE USER" },
		};

		public static class StringOptionsNames
		{
			public const string ActiveUser = "Active User";
		}

	}
}
