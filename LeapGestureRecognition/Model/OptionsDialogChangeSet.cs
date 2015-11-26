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
			ActiveUser = null;
			ModifiedUsers = new List<LGR_User>();
			DeletedUserIds = new List<int>();
			NewUsers = new List<LGR_User>();
		}

		public Dictionary<string, bool> BoolOptionsChangeset;
		public Dictionary<string, Color> BoneColorsChangeset;
		public LGR_User ActiveUser;
		public List<LGR_User> ModifiedUsers;
		public List<int> DeletedUserIds;
		public List<LGR_User> NewUsers;

			// on Delete, need to delete original name of gesture
				// Probably should use an ID instead of name...
	}
}
