using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGR
{
	public class StaticGestureClassWrapper
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public StaticGestureClass Gesture { get; set; }
		public StaticGestureInstance SampleInstance { get; set; } // For drawing
	}
}
