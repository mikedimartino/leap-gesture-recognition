using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LGR
{
	public class StatisticalClassifier
	{
		private ObservableCollection<StaticGestureClassWrapper> _staticGestureClasses;
		private ObservableCollection<DynamicGestureClassWrapper> _dynamicGestureClasses;

		public StatisticalClassifier(ObservableCollection<StaticGestureClassWrapper> staticGestureClasses, ObservableCollection<DynamicGestureClassWrapper> dynamicGestureClasses) 
		{
			_staticGestureClasses = staticGestureClasses;
			_dynamicGestureClasses = dynamicGestureClasses;
		}

		#region Public Properties
		public ObservableCollection<StaticGestureClassWrapper> StaticGestureClasses
		{
			get { return _staticGestureClasses; }
			set { _staticGestureClasses = value; }
		}

		public ObservableCollection<DynamicGestureClassWrapper> DynamicGestureClasses
		{
			get { return _dynamicGestureClasses; }
			set { _dynamicGestureClasses = value; }
		}
		#endregion

		#region Public Methods
		public Dictionary<StaticGestureClassWrapper, float> GetDistancesFromAllClasses(StaticGestureInstance gestureInstance)
		{
				lock (_staticGestureClasses)
				{
					var gestureDistances = new Dictionary<StaticGestureClassWrapper, float>();
					foreach (var gestureClass in _staticGestureClasses)
					{
						gestureDistances.Add(gestureClass, gestureClass.Gesture.DistanceTo(gestureInstance));
					}
					return gestureDistances;
				}
		}
		#endregion

	}
}
