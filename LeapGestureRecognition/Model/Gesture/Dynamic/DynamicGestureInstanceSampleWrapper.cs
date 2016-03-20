using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGR
{
	// THIS CLASS MAY NOT BE NECESSARY
	public class DynamicGestureInstanceSampleWrapper
	{
		public DynamicGestureInstanceSampleWrapper() 
		{
			Sample = null;
			Id = -1;
			InstanceId = -1;
			SampleName = "new sample";
		}

		public DynamicGestureInstanceSampleWrapper(DynamicGestureInstanceSample sample)
		{
			Sample = sample;
			Id = -1;
			InstanceId = -1;
			SampleName = "new sample";
		}

		public DynamicGestureInstanceSampleWrapper(Frame frame)
		{
			Sample = new DynamicGestureInstanceSample(frame);
			Id = -1;
			InstanceId = -1;
			SampleName = "new sample";
		}

		public int Id { get; set; }
		public int InstanceId { get; set; }
		public DynamicGestureInstanceSample Sample { get; set; }
		public string SampleName { get; set; }
	}
}
