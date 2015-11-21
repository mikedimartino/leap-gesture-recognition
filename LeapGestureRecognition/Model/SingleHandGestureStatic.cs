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
						var adjustedJointPos = jointPos - PalmNormal;
						_AllFingerJointsForDrawing.Add(adjustedJointPos);
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
		#endregion

	}
}
