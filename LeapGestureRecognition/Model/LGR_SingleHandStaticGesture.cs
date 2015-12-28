using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace LeapGestureRecognition.Model
{
	[DataContract]
	public class LGR_SingleHandStaticGesture
	{
		public LGR_SingleHandStaticGesture() { }

		// Gotta figure out how to handle both hands
		public LGR_SingleHandStaticGesture(Hand hand, string name = "_unnamed")
		{
			HandTransform = getHandTransform(hand);

			Name = name; // Might not care about name... just ID.
			IsLeft = hand.IsLeft;
			IsRight = hand.IsRight;
			PalmPosition = new LGR_Vec3(hand.PalmPosition); // Do not HandTransform.TransformPoint()
			PalmNormal = new LGR_Vec3(hand.PalmNormal);
			FingerJointPositions = getFingerJointPositions(hand);
			WristPos = new LGR_Vec3(HandTransform.TransformPoint(hand.WristPosition));
			ElbowPos = new LGR_Vec3(HandTransform.TransformPoint(hand.Arm.ElbowPosition));
			ForearmCenter = new LGR_Vec3(HandTransform.TransformPoint(hand.Arm.Center));
			setFingerBasePositions(hand);

			Id = -1;
			AveragedGestureId = -1;
		}

		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public bool IsLeft { get; set; }
		[DataMember]
		public bool IsRight { get; set; }
		[DataMember]
		public LGR_Vec3 PalmPosition { get; set; }
		[DataMember]
		public LGR_Vec3 PalmNormal { get; set; }

		// NOTE: All coordinates are relative to the hand's object space.
		[DataMember]
		public Dictionary<Finger.FingerType, Dictionary<Finger.FingerJoint, LGR_Vec3>> FingerJointPositions;
		[DataMember]
		public LGR_Vec3 WristPos { get; set; }
		[DataMember]
		public LGR_Vec3 ElbowPos { get; set; }
		[DataMember]
		public LGR_Vec3 IndexBasePos { get; set; }
		[DataMember]
		public LGR_Vec3 MiddleBasePos { get; set; }
		[DataMember]
		public LGR_Vec3 RingBasePos { get; set; }
		[DataMember]
		public LGR_Vec3 PinkyBasePos { get; set; }
		[DataMember]
		public LGR_Vec3 ForearmCenter { get; set; }

		public Matrix HandTransform { get; set; }

		public int Id { get; set; }
		public int AveragedGestureId { get; set; }


		public LGR_Vec3 ThumbBasePos
		{
			// Because of 0 length metacarpal
			get { return FingerJointPositions[Finger.FingerType.TYPE_THUMB][Finger.FingerJoint.JOINT_MCP]; }
		}


		#region Private Methods
		private Dictionary<Finger.FingerType, Dictionary<Finger.FingerJoint, LGR_Vec3>> getFingerJointPositions(Hand hand)
		{
			var fjp = new Dictionary<Finger.FingerType, Dictionary<Finger.FingerJoint, LGR_Vec3>>();
			foreach (var finger in hand.Fingers)
			{
				fjp.Add(finger.Type, new Dictionary<Finger.FingerJoint, LGR_Vec3>());
				foreach (var jointType in (Finger.FingerJoint[])Enum.GetValues(typeof(Finger.FingerJoint)))
				{
					// TODO: Confirm that Matrix.TransformPoint() does what I think it does.
					LGR_Vec3 translatedPoint = new LGR_Vec3(HandTransform.TransformPoint(finger.JointPosition(jointType)));
					fjp[finger.Type].Add(jointType, translatedPoint);
				}
			}
			return fjp;
		}

		private void setFingerBasePositions(Hand hand)
		{
			Finger index = hand.Fingers.Where(f => f.Type == Finger.FingerType.TYPE_INDEX).FirstOrDefault();
			Finger middle = hand.Fingers.Where(f => f.Type == Finger.FingerType.TYPE_MIDDLE).FirstOrDefault();
			Finger ring = hand.Fingers.Where(f => f.Type == Finger.FingerType.TYPE_RING).FirstOrDefault();
			Finger pinky = hand.Fingers.Where(f => f.Type == Finger.FingerType.TYPE_PINKY).FirstOrDefault();

			IndexBasePos = new LGR_Vec3(HandTransform.TransformPoint(index.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint));
			MiddleBasePos = new LGR_Vec3(HandTransform.TransformPoint(middle.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint));
			RingBasePos = new LGR_Vec3(HandTransform.TransformPoint(ring.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint));
			PinkyBasePos = new LGR_Vec3(HandTransform.TransformPoint(pinky.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint));
		}


		// Inspired by "Transforming Finger Coordinates into the Hand’s Frame of Reference"
		//	from https://developer.leapmotion.com/documentation/csharp/devguide/Leap_Hand.html
		private Matrix getHandTransform(Hand hand) // Might want to move this somewhere else
		{
			Vector yBasis = -hand.PalmNormal;
			Vector zBasis = -hand.Direction;
			Vector xBasis = yBasis.Cross(zBasis);
			Vector origin = hand.PalmPosition;
			Matrix handTransform = new Matrix(xBasis, yBasis, zBasis, origin);
			handTransform = handTransform.RigidInverse(); // I don't really understand the point of this
			return handTransform;
		}
		#endregion

		#region Public Methods
		public LGR_HandMeasurements GetMeasurements()
		{
			return new LGR_HandMeasurements()
			{
				PinkyLength = GetFingerLength(Finger.FingerType.TYPE_PINKY),
				RingLength = GetFingerLength(Finger.FingerType.TYPE_RING),
				MiddleLength = GetFingerLength(Finger.FingerType.TYPE_MIDDLE),
				IndexLength = GetFingerLength(Finger.FingerType.TYPE_INDEX),
				ThumbLength = GetFingerLength(Finger.FingerType.TYPE_THUMB)
			};
		}

		public float GetFingerLength(Finger.FingerType fingerType)
		{
			// Finger length = dist(PalmCenter, MCP) + dist(MCP, PIP) + dist(PIP, DIP) + dist(DIP, TIP)
			var joints = FingerJointPositions[fingerType];
			float length = PalmPosition.DistanceTo(joints[Finger.FingerJoint.JOINT_MCP]);
			length += joints[Finger.FingerJoint.JOINT_MCP].DistanceTo(joints[Finger.FingerJoint.JOINT_PIP]);
			length += joints[Finger.FingerJoint.JOINT_PIP].DistanceTo(joints[Finger.FingerJoint.JOINT_DIP]);
			length += joints[Finger.FingerJoint.JOINT_DIP].DistanceTo(joints[Finger.FingerJoint.JOINT_TIP]);
			return length;
		}
		#endregion
	}
}
