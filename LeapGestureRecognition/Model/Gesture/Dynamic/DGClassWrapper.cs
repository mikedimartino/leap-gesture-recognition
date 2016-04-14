using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeapGestureRecognition
{
	public class DGClassWrapper
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DGClass Gesture { get; set; }
		public DGInstance SampleInstance { get; set; }
	}
}
