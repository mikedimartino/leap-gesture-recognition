using Leap;
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
			Center = new LGR_Vec3(hand.PalmPosition);
			PalmNormal = new LGR_Vec3(hand.PalmNormal);
			FingerJointPositions = getFingerJointPositions(hand);
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
		public LGR_Vec3 Center { get; set; }
		[DataMember]
		public LGR_Vec3 PalmNormal { get; set; }
		[DataMember]
		public Dictionary<Finger.FingerType, Dictionary<Finger.FingerJoint, LGR_Vec3>> FingerJointPositions;


		private Dictionary<Finger.FingerType, Dictionary<Finger.FingerJoint, LGR_Vec3>> getFingerJointPositions(Hand hand)
		{
			Dictionary<Finger.FingerType, Dictionary<Finger.FingerJoint, LGR_Vec3>> fjp = new Dictionary<Finger.FingerType, Dictionary<Finger.FingerJoint, LGR_Vec3>>();
			foreach (Finger finger in hand.Fingers)
			{
				fjp.Add(finger.Type, new Dictionary<Finger.FingerJoint, LGR_Vec3>());
				foreach (Finger.FingerJoint jointType in (Finger.FingerJoint[])Enum.GetValues(typeof(Finger.FingerJoint)))
				{
					fjp[finger.Type].Add(jointType, new LGR_Vec3(finger.JointPosition(jointType)));
				}
			}
			return fjp;
		}

	}
}
