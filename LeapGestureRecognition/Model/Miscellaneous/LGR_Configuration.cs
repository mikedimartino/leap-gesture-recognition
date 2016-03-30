﻿
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LGR
{
	public class LGR_Configuration
	{
		public LGR_Configuration(SQLiteProvider provider)
		{
			ActiveUser = provider.GetActiveUser();
			AllUsers = provider.GetAllUsers();
			BoolOptions = provider.GetBoolOptions();
			BoneColors = provider.GetBoneColors();
		}

		public User ActiveUser { get; set; }
		public List<User> AllUsers { get; set; } // Not sure if this should go here or in MainViewModel (or somewhere else).
		public Dictionary<string, bool> BoolOptions { get; set; }
		public Dictionary<string, Color> BoneColors { get; set; }
	}
}
