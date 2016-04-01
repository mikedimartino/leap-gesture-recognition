using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LGR
{
	public class StatisticalClassifier
	{
		private ObservableCollection<SGClassWrapper> _staticGestureClasses;
		private ObservableCollection<DGClassWrapper> _dynamicGestureClasses;

		public StatisticalClassifier(ObservableCollection<SGClassWrapper> staticGestureClasses, ObservableCollection<DGClassWrapper> dynamicGestureClasses) 
		{
			_staticGestureClasses = staticGestureClasses;
			_dynamicGestureClasses = dynamicGestureClasses;
		}

		#region Public Properties
		public ObservableCollection<SGClassWrapper> StaticGestureClasses
		{
			get { return _staticGestureClasses; }
			set { _staticGestureClasses = value; }
		}

		public ObservableCollection<DGClassWrapper> DynamicGestureClasses
		{
			get { return _dynamicGestureClasses; }
			set { _dynamicGestureClasses = value; }
		}
		#endregion

		#region Public Methods
		public Dictionary<SGClassWrapper, float> GetDistancesFromAllClasses(SGInstance gestureInstance)
		{
				lock (_staticGestureClasses)
				{
					var gestureDistances = new Dictionary<SGClassWrapper, float>();
					foreach (var gestureClass in _staticGestureClasses)
					{
						gestureDistances.Add(gestureClass, gestureClass.Gesture.DistanceTo(gestureInstance));
					}
					return gestureDistances;
				}
		}

		public Dictionary<DGClassWrapper, float> GetDistancesFromAllClasses(DGInstance gestureInstance)
		{
			lock (_dynamicGestureClasses)
			{
				var gestureDistances = new Dictionary<DGClassWrapper, float>();
				foreach (var gestureClass in _dynamicGestureClasses)
				{
					gestureDistances.Add(gestureClass, gestureClass.Gesture.DistanceTo(gestureInstance));
				}
				return gestureDistances;
			}
		}
		#endregion

	}
}
