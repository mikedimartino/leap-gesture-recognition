﻿using Leap;
using LeapGestureRecognition.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Media;

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

		#region Operators
		public static LGR_Vec3 operator -(LGR_Vec3 vec1, LGR_Vec3 vec2)
		{
			return new LGR_Vec3(vec1.x - vec2.x, vec1.y - vec2.y, vec1.z - vec2.z);
		}
		public static LGR_Vec3 operator +(LGR_Vec3 vec1, LGR_Vec3 vec2)
		{
			return new LGR_Vec3(vec1.x + vec2.x, vec1.y + vec2.y, vec1.z + vec2.z);
		}
		public static LGR_Vec3 operator *(int scalar, LGR_Vec3 vec)
		{
			return new LGR_Vec3(scalar * vec.x, scalar * vec.y, scalar * vec.z);
		}
		public static LGR_Vec3 operator *(LGR_Vec3 vec, int scalar)
		{
			return new LGR_Vec3(scalar * vec.x, scalar * vec.y, scalar * vec.z);
		}
		public static LGR_Vec3 operator *(float scalar, LGR_Vec3 vec)
		{
			return new LGR_Vec3(scalar * vec.x, scalar * vec.y, scalar * vec.z);
		}
		public static LGR_Vec3 operator *(LGR_Vec3 vec, float scalar)
		{
			return new LGR_Vec3(scalar * vec.x, scalar * vec.y, scalar * vec.z);
		}
		#endregion

		public Vector ToLeapVector()
		{
			return new Vector(x, y, z);
		}

		public float DistanceTo(LGR_Vec3 other)
		{
			return (float) Math.Sqrt(Math.Pow(other.x - x, 2) + Math.Pow(other.y - y, 2) + Math.Pow(other.z - z, 2));
		}
	}

	// Not sure where to put this
	public enum LGR_Mode
	{
		Default,
		Playback,
		Recognize,
		Learn,
		Debug,
	};

}