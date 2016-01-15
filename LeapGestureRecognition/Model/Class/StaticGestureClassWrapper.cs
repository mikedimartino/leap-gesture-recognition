using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeapGestureRecognition.Model
{
	public class StaticGestureClassWrapper
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public StaticGestureClass Gesture { get; set; }
		public StaticGesture SampleInstance { get; set; } // For drawing
	}
}
