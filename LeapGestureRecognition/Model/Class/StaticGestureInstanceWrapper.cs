using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGR
{
	public class StaticGestureInstanceWrapper
	{
		public StaticGestureInstanceWrapper() 
		{
			Gesture = null;
			Id = -1;
			ClassId = -1;
			InstanceName = "new instance";
		}

		public StaticGestureInstanceWrapper(StaticGestureInstance instance)
		{
			Gesture = instance;
			Id = -1;
			ClassId = -1;
			InstanceName = "new instance";
		}

		public StaticGestureInstanceWrapper(Frame frame)
		{
			Gesture = new StaticGestureInstance(frame);
			Id = -1;
			ClassId = -1;
			InstanceName = "new instance";
		}

		public int Id { get; set; }
		public int ClassId { get; set; }
		public StaticGestureInstance Gesture { get; set; }
		public string InstanceName { get; set; }
	}
}
