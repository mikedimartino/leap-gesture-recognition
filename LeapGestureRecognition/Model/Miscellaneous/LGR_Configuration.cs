﻿
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LeapGestureRecognition
{
	public class LGR_Configuration
	{
		public LGR_Configuration(SQLiteProvider provider)
		{
			BoolOptions = provider.GetBoolOptions();
			BoneColors = provider.GetBoneColors();
		}

		public Dictionary<string, bool> BoolOptions { get; set; }
		public Dictionary<string, Color> BoneColors { get; set; }
	}
}
