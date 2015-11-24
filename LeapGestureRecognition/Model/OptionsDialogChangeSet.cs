using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LeapGestureRecognition.Model
{
	public class OptionsDialogChangeset
	{
		public OptionsDialogChangeset()
		{
			BoolOptionsChangeset = new Dictionary<string, bool>();
			BoneColorsChangeset = new Dictionary<string, Color>();
		}

		public Dictionary<string, bool> BoolOptionsChangeset;
		public Dictionary<string, Color> BoneColorsChangeset;
	}
}
