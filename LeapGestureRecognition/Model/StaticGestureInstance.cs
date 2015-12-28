using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Leap;

namespace LeapGestureRecognition.Model
{
	// Used in Naive Bayes Classifier
	[DataContract]
	public class StaticGestureInstance
	{
		public StaticGestureInstance() { }

		public StaticGestureInstance(SingleHandGestureStatic gesture)
		{
			Name = gesture.Name;
			IsLeft = gesture.IsLeft;
			IsRight = gesture.IsRight;
			HandOrientation = gesture.HandOrientation;

			PalmCenter = gesture.PalmCenter;
			PalmNormal = gesture.PalmNormal;
			FingerJointPositions = gesture.FingerJointPositions;
			WristPos = gesture.WristPos;
			ElbowPos = gesture.ElbowPos;
			ForearmCenter = gesture.ForearmCenter;
		}


		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public bool IsLeft { get; set; }
		[DataMember]
		public bool IsRight { get; set; }
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
		public LGR_Vec3 ForearmCenter { get; set; }

		public Matrix HandOrientation { get; set; }


		public LGR_Vec3 ThumbBasePos
		{
			// Because of 0 length metacarpal
			get { return FingerJointPositions[Finger.FingerType.TYPE_THUMB][Finger.FingerJoint.JOINT_MCP]; }
		}
	}
}
