﻿using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace LeapGestureRecognition.Model
{
	[DataContract]
	public class SingleHandGestureStatic
	{
		public SingleHandGestureStatic() { }

		public SingleHandGestureStatic(Hand hand, string name = "_unnamed")
		{
			Name = name;
			IsLeft = hand.IsLeft;
			IsRight = hand.IsRight;
			Pitch = hand.Direction.Pitch;
			Yaw = hand.Direction.Yaw;
			Roll = hand.Direction.Roll;
			PalmCenter = new LGR_Vec3(hand.PalmPosition);
			PalmNormal = new LGR_Vec3(hand.PalmNormal);
			FingerJointPositions = getFingerJointPositions(hand);
			WristPos = new LGR_Vec3(hand.WristPosition);
			ElbowPos = new LGR_Vec3(hand.Arm.ElbowPosition);
			ForearmCenter = new LGR_Vec3(hand.Arm.Center);
			ArmX = new LGR_Vec3(hand.Arm.Basis.xBasis);
			ArmY = new LGR_Vec3(hand.Arm.Basis.yBasis);
			ArmZ = new LGR_Vec3(hand.Arm.Basis.zBasis);
			setFingerBasePositions(hand);

			HandOrientation = getHandOrientation(hand);
			RecognizableInstance = new StaticGestureInstance(this);
		}

		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public bool IsLeft { get; set; }
		[DataMember]
		public bool IsRight { get; set; }
		[DataMember]
		public float Pitch { get; set; }
		[DataMember]
		public float Yaw { get; set; }
		[DataMember]
		public float Roll { get; set; }
		[DataMember]
		public LGR_Vec3 PalmCenter { get; set; }
		[DataMember]
		public LGR_Vec3 PalmNormal { get; set; }
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
		// Arm x, y, and z basis vectors
		[DataMember]
		public LGR_Vec3 ArmX { get; set; }
		[DataMember]
		public LGR_Vec3 ArmY { get; set; }
		[DataMember]
		public LGR_Vec3 ArmZ { get; set; }

		public Matrix HandOrientation { get; set; }

		public StaticGestureInstance RecognizableInstance { get; set; }


		public LGR_Vec3 ThumbBasePos
		{
			// Because of 0 length metacarpal
			get { return FingerJointPositions[Finger.FingerType.TYPE_THUMB][Finger.FingerJoint.JOINT_MCP]; }
		}

		private List<LGR_Vec3> _AllFingerJointsForDrawing;
		public List<LGR_Vec3> AllFingerJointsForDrawing
		{
			get
			{
				if (_AllFingerJointsForDrawing != null) return _AllFingerJointsForDrawing;

				_AllFingerJointsForDrawing = new List<LGR_Vec3>();
				foreach (var finger in FingerJointPositions.Values)
				{
					foreach (var jointPos in finger.Values)
					{
						// I'm storing the position relative to the palm normal. 
						// Need to adjust for drawing.
						//var adjustedJointPos = jointPos - PalmNormal;
						//_AllFingerJointsForDrawing.Add(adjustedJointPos);
						_AllFingerJointsForDrawing.Add(jointPos);
					}
				}
				return _AllFingerJointsForDrawing;
			}
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
					fjp[finger.Type].Add(jointType, new LGR_Vec3(finger.JointPosition(jointType)));
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

			IndexBasePos = new LGR_Vec3(index.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint);
			MiddleBasePos = new LGR_Vec3(middle.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint);
			RingBasePos = new LGR_Vec3(ring.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint);
			PinkyBasePos = new LGR_Vec3(pinky.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint);
		}

		private Matrix getHandOrientation(Hand hand) // Might want to move this somewhere else
		{
			Vector yBasis = hand.PalmNormal; // Might want to flip the sign of this
			Vector zBasis = hand.Direction;
			Vector xBasis = yBasis.Cross(zBasis);
			Vector origin = hand.PalmPosition;
			return new Matrix(xBasis, yBasis, zBasis, origin);
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
			float length = PalmCenter.DistanceTo(joints[Finger.FingerJoint.JOINT_MCP]);
			length += joints[Finger.FingerJoint.JOINT_MCP].DistanceTo(joints[Finger.FingerJoint.JOINT_PIP]);
			length += joints[Finger.FingerJoint.JOINT_PIP].DistanceTo(joints[Finger.FingerJoint.JOINT_DIP]);
			length += joints[Finger.FingerJoint.JOINT_DIP].DistanceTo(joints[Finger.FingerJoint.JOINT_TIP]);
			return length;
		}
		#endregion
	}
}