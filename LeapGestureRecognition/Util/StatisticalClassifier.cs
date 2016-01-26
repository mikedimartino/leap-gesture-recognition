using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LGR
{
	public class StatisticalClassifier
	{
		private ObservableCollection<StaticGestureClassWrapper> _gestureClasses;

		public StatisticalClassifier(ObservableCollection<StaticGestureClassWrapper> gestureClasses) 
		{
			_gestureClasses = gestureClasses;
		}

		public ObservableCollection<StaticGestureClassWrapper> GestureClasses
		{
			get { return _gestureClasses; }
			set { _gestureClasses = value; }
		}

		public Dictionary<StaticGestureClassWrapper, float> GetDistancesFromAllClasses(StaticGestureInstance gestureInstance)
		{
			var gestureDistances = new Dictionary<StaticGestureClassWrapper, float>();
			foreach (var gestureClass in _gestureClasses)
			{
				gestureDistances.Add(gestureClass, gestureClass.Gesture.DistanceTo(gestureInstance));
			}
			return gestureDistances;
		}
	}
}
