﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LeapGestureRecognition.Model
{
	public class EditGestureChangeset
	{
		public string NewName;
		public List<int> DeletedGestureInstances;
		public List<LGR_StaticGesture> NewGestureInstances;

		public EditGestureChangeset()
		{
			NewName = null;
			DeletedGestureInstances = new List<int>();
			NewGestureInstances = new List<LGR_StaticGesture>();
		}
		
	}
}