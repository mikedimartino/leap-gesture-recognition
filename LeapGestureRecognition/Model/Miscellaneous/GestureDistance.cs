using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGR
{
	public class GestureDistance
	{
		public GestureDistance() { }
		public GestureDistance(string gestureName, float distance)
		{
			Name = gestureName;
			Distance = distance;
		}

		public string Name { get; set; }
		public float Distance { get; set; }
	}
}
