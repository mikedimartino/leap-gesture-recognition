using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeapGestureRecognition
{
	public class SGInstanceWrapper
	{
		public SGInstanceWrapper() 
		{
			Gesture = null;
			Id = -1;
			ClassId = -1;
			InstanceName = "new instance";
		}

		public SGInstanceWrapper(SGInstance instance)
		{
			Gesture = instance;
			Id = -1;
			ClassId = -1;
			InstanceName = "new instance";
		}

		public SGInstanceWrapper(Frame frame)
		{
			Gesture = new SGInstance(frame);
			Id = -1;
			ClassId = -1;
			InstanceName = "new instance";
		}

		public int Id { get; set; }
		public int ClassId { get; set; }
		public SGInstance Gesture { get; set; }
		public string InstanceName { get; set; }
	}
}
