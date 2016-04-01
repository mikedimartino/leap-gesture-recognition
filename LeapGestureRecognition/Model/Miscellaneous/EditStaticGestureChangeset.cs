using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LGR
{
	public class EditStaticGestureChangeset
	{
		public string NewName;
		public List<int> DeletedGestureInstances;
		public List<SGInstanceWrapper> NewGestureInstances;

		public EditStaticGestureChangeset()
		{
			NewName = null;
			DeletedGestureInstances = new List<int>();
			NewGestureInstances = new List<SGInstanceWrapper>();
		}

		public bool ChangesExist()
		{
			return NewGestureInstances.Any() || DeletedGestureInstances.Any();
		}

	}
}
