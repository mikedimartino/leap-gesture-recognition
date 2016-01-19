using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LGR
{
	public class OptionsChangeset
	{
		public Dictionary<string, bool> BoolOptionsChangeset;
		public Dictionary<string, Color> BoneColorsChangeset;
		public User ActiveUser;
		public List<User> ModifiedUsers;
		public List<int> DeletedUserIds;
		public List<User> NewUsers;

		public OptionsChangeset()
		{
			BoolOptionsChangeset = new Dictionary<string, bool>();
			BoneColorsChangeset = new Dictionary<string, Color>();
			ActiveUser = null;
			ModifiedUsers = new List<User>();
			DeletedUserIds = new List<int>();
			NewUsers = new List<User>();
		}

	}
}
