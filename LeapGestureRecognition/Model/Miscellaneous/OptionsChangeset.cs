using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LeapGestureRecognition
{
	public class OptionsChangeset
	{
		public Dictionary<string, bool> BoolOptionsChangeset;
		public Dictionary<string, Color> BoneColorsChangeset;
		public List<int> DeletedUserIds;

		public OptionsChangeset()
		{
			BoolOptionsChangeset = new Dictionary<string, bool>();
			BoneColorsChangeset = new Dictionary<string, Color>();
		}

	}
}
