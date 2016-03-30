using Leap;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Media;

namespace LGR
{
	[DataContract]
	public class Vec3
	{
		public Vec3() { X = Y = Z = 0; }

		public Vec3(float x, float y, float z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public Vec3(Leap.Vector leapVector)
		{
			this.X = leapVector.x;
			this.Y = leapVector.y;
			this.Z = leapVector.z;
		}

		[DataMember]
		public float X { get; set; }
		[DataMember]
		public float Y { get; set; }
		[DataMember]
		public float Z { get; set; }

		#region Operators
		public static Vec3 operator -(Vec3 vec1, Vec3 vec2)
		{
			return new Vec3(vec1.X - vec2.X, vec1.Y - vec2.Y, vec1.Z - vec2.Z);
		}
		public static Vec3 operator +(Vec3 vec1, Vec3 vec2)
		{
			return new Vec3(vec1.X + vec2.X, vec1.Y + vec2.Y, vec1.Z + vec2.Z);
		}
		public static Vec3 operator *(int scalar, Vec3 vec)
		{
			return new Vec3(scalar * vec.X, scalar * vec.Y, scalar * vec.Z);
		}
		public static Vec3 operator *(Vec3 vec, int scalar)
		{
			return new Vec3(scalar * vec.X, scalar * vec.Y, scalar * vec.Z);
		}
		public static Vec3 operator *(float scalar, Vec3 vec)
		{
			return new Vec3(scalar * vec.X, scalar * vec.Y, scalar * vec.Z);
		}
		public static Vec3 operator *(Vec3 vec, float scalar)
		{
			return new Vec3(scalar * vec.X, scalar * vec.Y, scalar * vec.Z);
		}
		public static Vec3 operator /(Vec3 vec, float scalar)
		{
			return new Vec3(vec.X / scalar, vec.Y / scalar, vec.Z / scalar);
		}
		#endregion

		public Vector ToLeapVector()
		{
			return new Vector(X, Y, Z);
		}

		public float DistanceTo(Vec3 other)
		{
			return (float) Math.Sqrt(Math.Pow(other.X - X, 2) + Math.Pow(other.Y - Y, 2) + Math.Pow(other.Z - Z, 2));
		}

		// Linearly interpolate between two Vec3's.
		// amount should be between 0 and 1.
		public Vec3 Lerp(Vec3 other, float amount) 
		{
			float deltaX = (Math.Abs(X - other.X) * amount) * ((X < other.X) ? 1 : -1);
			float deltaY = (Math.Abs(Y - other.Y) * amount) * ((Y < other.Y) ? 1 : -1);
			float deltaZ = (Math.Abs(Z - other.Z) * amount) * ((Z < other.Z) ? 1 : -1);
			return new Vec3(X + deltaX, Y + deltaY, Z + deltaZ);
		}
	}

}
