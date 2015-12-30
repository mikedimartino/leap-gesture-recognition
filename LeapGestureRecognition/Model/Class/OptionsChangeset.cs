using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LeapGestureRecognition.Model
{
	public class OptionsChangeset
	{
		public Dictionary<string, bool> BoolOptionsChangeset;
		public Dictionary<string, Color> BoneColorsChangeset;
		public LGR_User ActiveUser;
		public List<LGR_User> ModifiedUsers;
		public List<int> DeletedUserIds;
		public List<LGR_User> NewUsers;

		public OptionsChangeset()
		{
			BoolOptionsChangeset = new Dictionary<string, bool>();
			BoneColorsChangeset = new Dictionary<string, Color>();
			ActiveUser = null;
			ModifiedUsers = new List<LGR_User>();
			DeletedUserIds = new List<int>();
			NewUsers = new List<LGR_User>();
		}

	}
}
