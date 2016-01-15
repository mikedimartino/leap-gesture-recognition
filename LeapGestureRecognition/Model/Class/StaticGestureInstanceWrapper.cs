using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeapGestureRecognition.Model
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

		public StaticGestureInstanceWrapper(StaticGesture instance)
		{
			Gesture = instance;
			Id = -1;
			ClassId = -1;
			InstanceName = "new instance";
		}

		public int Id { get; set; }
		public int ClassId { get; set; }
		public StaticGesture Gesture { get; set; }
		public string InstanceName { get; set; }
	}
}
