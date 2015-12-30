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
		public LGR_SingleHandStaticGesture(Hand hand)
		{
			HandTransform = getHandTransform(hand);

			IsLeft = hand.IsLeft;
			IsRight = hand.IsRight;
			PalmPosition = new LGR_Vec3(hand.PalmPosition); // Do not HandTransform.TransformPoint()
			PalmNormal = new LGR_Vec3(hand.PalmNormal);
			HandDirection = new LGR_Vec3(hand.Direction);
			ArmX = new LGR_Vec3(hand.Arm.Basis.xBasis);
			ArmY = new LGR_Vec3(hand.Arm.Basis.yBasis);
			ArmZ = new LGR_Vec3(hand.Arm.Basis.zBasis);

			// Coordinates relative to the hand's object space:
			WristPos_Relative = new LGR_Vec3(HandTransform.TransformPoint(hand.WristPosition));
			ElbowPos_Relative = new LGR_Vec3(HandTransform.TransformPoint(hand.Arm.ElbowPosition));
			ForearmCenter_Relative = new LGR_Vec3(HandTransform.TransformPoint(hand.Arm.Center));

			// World coordinates
			WristPos_World = new LGR_Vec3(hand.WristPosition);
			ElbowPos_World = new LGR_Vec3(hand.Arm.ElbowPosition);
			ForearmCenter_World = new LGR_Vec3(hand.Arm.Center);

			// Sets both _World and _Relative coordinates
			setFingerJointPositions(hand);
			setFingerBasePositions(hand);
		}

		[DataMember]
		public bool IsLeft { get; set; }
		[DataMember]
		public bool IsRight { get; set; }
		[DataMember]
		public LGR_Vec3 PalmPosition { get; set; }
		[DataMember]
		public LGR_Vec3 PalmNormal { get; set; }
		[DataMember]
		public LGR_Vec3 HandDirection { get; set; }
		[DataMember]
		public LGR_Vec3 ArmX { get; set; }
		[DataMember]
		public LGR_Vec3 ArmY { get; set; }
		[DataMember]
		public LGR_Vec3 ArmZ { get; set; }

		// Coordinates relative to the hand's object space:
		[DataMember]
		public Dictionary<Finger.FingerType, Dictionary<Finger.FingerJoint, LGR_Vec3>> FingerJointPositions_Relative;
		[DataMember]
		public LGR_Vec3 WristPos_Relative { get; set; }
		[DataMember]
		public LGR_Vec3 ElbowPos_Relative { get; set; }
		[DataMember]
		public LGR_Vec3 IndexBasePos_Relative { get; set; }
		[DataMember]
		public LGR_Vec3 MiddleBasePos_Relative { get; set; }
		[DataMember]
		public LGR_Vec3 RingBasePos_Relative { get; set; }
		[DataMember]
		public LGR_Vec3 PinkyBasePos_Relative { get; set; }
		[DataMember]
		public LGR_Vec3 ForearmCenter_Relative { get; set; }

		// World coordinates:
		[DataMember]
		public Dictionary<Finger.FingerType, Dictionary<Finger.FingerJoint, LGR_Vec3>> FingerJointPositions_World;
		[DataMember]
		public LGR_Vec3 WristPos_World { get; set; }
		[DataMember]
		public LGR_Vec3 ElbowPos_World { get; set; }
		[DataMember]
		public LGR_Vec3 IndexBasePos_World { get; set; }
		[DataMember]
		public LGR_Vec3 MiddleBasePos_World { get; set; }
		[DataMember]
		public LGR_Vec3 RingBasePos_World { get; set; }
		[DataMember]
		public LGR_Vec3 PinkyBasePos_World { get; set; }
		[DataMember]
		public LGR_Vec3 ForearmCenter_World { get; set; }
		

		public Matrix HandTransform { get; set; }

		public LGR_Vec3 ThumbBasePos_Relative
		{
			// Because of 0 length metacarpal
			get { return FingerJointPositions_Relative[Finger.FingerType.TYPE_THUMB][Finger.FingerJoint.JOINT_MCP]; }
		}

		public LGR_Vec3 ThumbBasePos_World
		{
			// Because of 0 length metacarpal
			get { return FingerJointPositions_World[Finger.FingerType.TYPE_THUMB][Finger.FingerJoint.JOINT_MCP]; }
		}


		#region Private Methods
		private void setFingerJointPositions(Hand hand)
		{
			FingerJointPositions_Relative = new Dictionary<Finger.FingerType, Dictionary<Finger.FingerJoint, LGR_Vec3>>();
			FingerJointPositions_World = new Dictionary<Finger.FingerType,Dictionary<Finger.FingerJoint,LGR_Vec3>>();
			foreach (var finger in hand.Fingers)
			{
				FingerJointPositions_Relative.Add(finger.Type, new Dictionary<Finger.FingerJoint, LGR_Vec3>());
				FingerJointPositions_World.Add(finger.Type, new Dictionary<Finger.FingerJoint, LGR_Vec3>());
				foreach (var jointType in (Finger.FingerJoint[])System.Enum.GetValues(typeof(Finger.FingerJoint)))
				{
					// TODO: Confirm that Matrix.TransformPoint() does what I think it does.
					LGR_Vec3 relativePoint = new LGR_Vec3(HandTransform.TransformPoint(finger.JointPosition(jointType)));
					FingerJointPositions_Relative[finger.Type].Add(jointType, relativePoint);
					LGR_Vec3 worldPoint = new LGR_Vec3(finger.JointPosition(jointType));
					FingerJointPositions_World[finger.Type].Add(jointType, worldPoint);
				}
			}
		}

		private void setFingerBasePositions(Hand hand)
		{
			Finger index = hand.Fingers.Where(f => f.Type == Finger.FingerType.TYPE_INDEX).FirstOrDefault();
			Finger middle = hand.Fingers.Where(f => f.Type == Finger.FingerType.TYPE_MIDDLE).FirstOrDefault();
			Finger ring = hand.Fingers.Where(f => f.Type == Finger.FingerType.TYPE_RING).FirstOrDefault();
			Finger pinky = hand.Fingers.Where(f => f.Type == Finger.FingerType.TYPE_PINKY).FirstOrDefault();

			IndexBasePos_World = new LGR_Vec3(index.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint);
			MiddleBasePos_World = new LGR_Vec3(middle.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint);
			RingBasePos_World = new LGR_Vec3(ring.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint);
			PinkyBasePos_World = new LGR_Vec3(pinky.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint);

			IndexBasePos_Relative = new LGR_Vec3(HandTransform.TransformPoint(index.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint));
			MiddleBasePos_Relative = new LGR_Vec3(HandTransform.TransformPoint(middle.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint));
			RingBasePos_Relative = new LGR_Vec3(HandTransform.TransformPoint(ring.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint));
			PinkyBasePos_Relative = new LGR_Vec3(HandTransform.TransformPoint(pinky.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint));
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
			var joints = FingerJointPositions_World[fingerType];
			float length = PalmPosition.DistanceTo(joints[Finger.FingerJoint.JOINT_MCP]);
			length += joints[Finger.FingerJoint.JOINT_MCP].DistanceTo(joints[Finger.FingerJoint.JOINT_PIP]);
			length += joints[Finger.FingerJoint.JOINT_PIP].DistanceTo(joints[Finger.FingerJoint.JOINT_DIP]);
			length += joints[Finger.FingerJoint.JOINT_DIP].DistanceTo(joints[Finger.FingerJoint.JOINT_TIP]);
			return length;
		}
		#endregion
	}
}
