using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGR
{
	public class DynamicGestureInstanceWrapper
	{
		public DynamicGestureInstanceWrapper() 
		{
			Instance = null;
			Id = -1;
			ClassId = -1;
			InstanceName = "new instance";
		}

		public DynamicGestureInstanceWrapper(DynamicGestureInstance instance)
		{
			Instance = instance;
			Id = -1;
			ClassId = -1;
			InstanceName = "new instance";
		}

		public int Id { get; set; }
		public int ClassId { get; set; }
		public DynamicGestureInstance Instance { get; set; }
		public string InstanceName { get; set; }
	}
}
