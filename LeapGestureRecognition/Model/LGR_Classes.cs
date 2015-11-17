using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LeapGestureRecognition.Model
{
	[DataContract]
	public class LGR_Vec3
	{
		public LGR_Vec3() { }

		public LGR_Vec3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public LGR_Vec3(Leap.Vector leapVector)
		{
			this.x = leapVector.x;
			this.y = leapVector.y;
			this.z = leapVector.z;
		}

		[DataMember]
		public float x;
		[DataMember]
		public float y;
		[DataMember]
		public float z;
	}
}
