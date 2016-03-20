using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGR
{
	public class DynamicGestureClassWrapper
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DynamicGestureClass Gesture { get; set; }
		public DynamicGestureInstance SampleInstance { get; set; }
	}
}
