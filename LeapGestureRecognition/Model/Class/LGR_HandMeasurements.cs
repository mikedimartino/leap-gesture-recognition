using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeapGestureRecognition.Model
{
	public class LGR_HandMeasurements
	{
		public LGR_HandMeasurements() { }

		public float PinkyLength { get; set; }
		public float RingLength { get; set; }
		public float MiddleLength { get; set; }
		public float IndexLength { get; set; }
		public float ThumbLength { get; set; }
	}
}
