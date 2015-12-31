using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeapGestureRecognition.Model
{
	public class BayesStaticGestureWrapper
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public BayesStaticGesture Gesture { get; set; }
		public LGR_StaticGesture SampleInstance { get; set; } // For drawing
	}

}
