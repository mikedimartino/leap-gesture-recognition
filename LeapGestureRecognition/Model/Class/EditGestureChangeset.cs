using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LGR
{
	public class EditGestureChangeset
	{
		public string NewName;
		public List<int> DeletedGestureInstances;
		public List<StaticGestureInstanceWrapper> NewGestureInstances;

		public EditGestureChangeset()
		{
			NewName = null;
			DeletedGestureInstances = new List<int>();
			NewGestureInstances = new List<StaticGestureInstanceWrapper>();
		}

		public bool ChangesExist()
		{
			return NewGestureInstances.Any() || DeletedGestureInstances.Any();
		}
	}
}
