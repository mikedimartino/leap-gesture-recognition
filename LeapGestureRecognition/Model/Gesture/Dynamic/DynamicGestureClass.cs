using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LGR
{
	public class DynamicGestureClass 
	{
		public DynamicGestureClass() { }
		public DynamicGestureClass(ObservableCollection<DynamicGestureInstanceWrapper> instances)
		{
			//TODO: Implement
		}

		#region Public Properties
		public GestureType GestureType { get { return GestureType.Dynamic; } }
		#endregion

		#region Public Methods
		private Random rand = new Random();
		public float DistanceTo(DynamicGestureInstance otherInstance)
		{
			return 0.1f * rand.Next(100);
		}
		#endregion
	}
}
