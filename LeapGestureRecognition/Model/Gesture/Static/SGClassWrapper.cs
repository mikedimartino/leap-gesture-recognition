using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGR
{
	public class SGClassWrapper
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public SGClass Gesture { get; set; }
		public SGInstance SampleInstance { get; set; } // For drawing
	}
}
