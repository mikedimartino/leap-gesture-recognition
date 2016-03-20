using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGR
{
	public class EditDynamicGestureChangeset
	{
		public string NewName;
		public List<int> DeletedGestureInstances;
		public List<DynamicGestureInstanceWrapper> NewGestureInstances;

		public EditDynamicGestureChangeset()
		{
			NewName = null;
			DeletedGestureInstances = new List<int>();
			NewGestureInstances = new List<DynamicGestureInstanceWrapper>();
		}

		public bool ChangesExist()
		{
			return NewGestureInstances.Any() || DeletedGestureInstances.Any();
		}
	}
}
