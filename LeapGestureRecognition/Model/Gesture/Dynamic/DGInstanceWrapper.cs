using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGR
{
	public class DGInstanceWrapper
	{
		public DGInstanceWrapper() 
		{
			Instance = null;
			Id = -1;
			ClassId = -1;
			InstanceName = "new instance";
		}

		public DGInstanceWrapper(DGInstance instance)
		{
			Instance = instance;
			Id = -1;
			ClassId = -1;
			InstanceName = "new instance";
		}

		public int Id { get; set; }
		public int ClassId { get; set; }
		public DGInstance Instance { get; set; }
		public string InstanceName { get; set; }
	}
}
