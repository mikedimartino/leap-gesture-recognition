using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGR
{
	public class GestureDistance
	{
		public GestureDistance() { }
		public GestureDistance(StaticGestureClassWrapper gesture, float distance)
		{
			Gesture = gesture;
			Distance = distance;
		}

		public StaticGestureClassWrapper Gesture { get; set; }
		public float Distance { get; set; }
	}
}
