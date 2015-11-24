using Leap;
using LeapGestureRecognition.Model;
using LeapGestureRecognition.ViewModel;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LeapGestureRecognition.Util
{
	public class SharpGLHelper
	{
		private OpenGL _gl;
		private IntPtr _quadric;
		private Dictionary<string, Color> _boneColors;

		// TODO: Define quadrics for every sphere / cylinder so that new ones don't have to be initialized every time.

		public SharpGLHelper(OpenGL gl, Dictionary<string, Color> boneColors)
		{
			_gl = gl;
			_quadric = _gl.NewQuadric();
			_boneColors = boneColors;
		}


		public void DrawFrame(Frame frame, bool showArms)
		{
			foreach (Hand hand in frame.Hands)
			{
				float opacity = hand.Confidence;
				DrawHand(new SingleHandGestureStatic(hand), showArms, opacity);
			}
		}

		public void DrawAxes()
		{
			int axisLength = 300;
			double axisRadius = 2;
			LGR_Vec3 origin = new LGR_Vec3(0, 0, 0);
			LGR_Vec3 xAxis = new LGR_Vec3(1, 0, 0);
			LGR_Vec3 yAxis = new LGR_Vec3(0, 1, 0);
			LGR_Vec3 zAxis = new LGR_Vec3(0, 0, 1);

			// +X = red, +Y = green, +Z = blue
			DrawCylinder(axisRadius, origin, xAxis * axisLength, Colors.Red, 1.0f);
			DrawCylinder(axisRadius, origin, yAxis * axisLength, Colors.Green, 1.0f);
			DrawCylinder(axisRadius, origin, zAxis * axisLength, Colors.Blue, 1.0f);
		}

		public void DrawCylinder(double radius, Vector basePosition, Vector topPosition, Color color, float opacity = 1) // Can get rid of this eventually
		{
			_gl.LoadIdentity();
			_gl.Color(color.R, color.G, color.B, opacity);

			// Rotate to correct orientation (run along vector between basePosition and topBosition)
			// Code inspired by Toby Smith: http://www.thjsmith.com/40/cylinder-between-two-points-opengl-c
			Vector zAxis = new Vector(0, 0, 1); // +z is default direction for cylinders to face in OpenGL
			Vector diffVec = topPosition - basePosition;

			Vector axisOfOrientation = zAxis.Cross(diffVec);
			double angle = (180 / Math.PI) * Math.Acos(zAxis.Dot(diffVec) / diffVec.Magnitude);
			_gl.Translate(basePosition.x, basePosition.y, basePosition.z);
			_gl.Rotate(angle, axisOfOrientation.x, axisOfOrientation.y, axisOfOrientation.z);

			IntPtr cylQuadric = _gl.NewQuadric(); //TODO: Define these somewhere else so new ones aren't initialized every time
			double height = basePosition.DistanceTo(topPosition);
			_gl.Cylinder(cylQuadric, radius, radius, height, 10, 10);
		}

		public void DrawCylinder(double radius, LGR_Vec3 basePosition, LGR_Vec3 topPosition, Color color, float opacity = 1)
		{
			DrawCylinder(radius, basePosition.ToLeapVector(), topPosition.ToLeapVector(), color, opacity);
		}


		public void DrawSphere(Vector position, double radius, Color color, float opacity = 1) 
		{
			_gl.LoadIdentity();
			_gl.Color(color.R, color.G, color.B, opacity);
			_gl.Translate(position.x, position.y, position.z);
			
			IntPtr sphereQuadric = _gl.NewQuadric(); //TODO: Define these somewhere else so new ones aren't initialized every time
			_gl.Sphere(sphereQuadric, radius, 25, 25);
		}

		public void DrawSphere(LGR_Vec3 position, double radius, Color color, float opacity = 1)
		{
			DrawSphere(position.ToLeapVector(), radius, color, opacity);
		}

		// Temp, just for drawing pyramid
		private static float _rotation = 0;
		public void DrawRotatingPyramid()
		{
			//  Load the identity matrix.
			_gl.LoadIdentity();

			//  Rotate around the Y axis.
			_gl.Rotate(_rotation, 0.0f, 1.0f, 0.0f);

			//  Draw a coloured pyramid.
			_gl.Begin(OpenGL.GL_TRIANGLES);
			_gl.Color(1.0f, 0.0f, 0.0f);
			_gl.Vertex(0.0f, 1.0f, 0.0f);
			_gl.Color(0.0f, 1.0f, 0.0f);
			_gl.Vertex(-1.0f, -1.0f, 1.0f);
			_gl.Color(0.0f, 0.0f, 1.0f);
			_gl.Vertex(1.0f, -1.0f, 1.0f);
			_gl.Color(1.0f, 0.0f, 0.0f);
			_gl.Vertex(0.0f, 1.0f, 0.0f);
			_gl.Color(0.0f, 0.0f, 1.0f);
			_gl.Vertex(1.0f, -1.0f, 1.0f);
			_gl.Color(0.0f, 1.0f, 0.0f);
			_gl.Vertex(1.0f, -1.0f, -1.0f);
			_gl.Color(1.0f, 0.0f, 0.0f);
			_gl.Vertex(0.0f, 1.0f, 0.0f);
			_gl.Color(0.0f, 1.0f, 0.0f);
			_gl.Vertex(1.0f, -1.0f, -1.0f);
			_gl.Color(0.0f, 0.0f, 1.0f);
			_gl.Vertex(-1.0f, -1.0f, -1.0f);
			_gl.Color(1.0f, 0.0f, 0.0f);
			_gl.Vertex(0.0f, 1.0f, 0.0f);
			_gl.Color(0.0f, 0.0f, 1.0f);
			_gl.Vertex(-1.0f, -1.0f, -1.0f);
			_gl.Color(0.0f, 1.0f, 0.0f);
			_gl.Vertex(-1.0f, -1.0f, 1.0f);
			_gl.End();

			//  Nudge the rotation.
			_rotation += 3.0f;
		}


		// Diagram of hand: https://blog.leapmotion.com/wp-content/uploads/2014/05/boneapi1.png
		public void DrawHand(SingleHandGestureStatic hand, bool showArms, float opacity = 1)
		{
			// Draw wrist position
			DrawSphere(hand.WristPos, Constants.WristSphereRadius, _boneColors[Constants.BoneNames.Wrist], opacity);
			// Draw palm position
			DrawSphere(hand.PalmPos, Constants.PalmSphereRadius, _boneColors[Constants.BoneNames.Palm], opacity);

			foreach (var fjp in hand.FingerJointPositions)
			{
				var finger = fjp.Value;
				var fingerType = fjp.Key;

				LGR_Vec3 mcp = finger[Finger.FingerJoint.JOINT_MCP];
				LGR_Vec3 pip = finger[Finger.FingerJoint.JOINT_PIP];
				LGR_Vec3 dip = finger[Finger.FingerJoint.JOINT_DIP];
				LGR_Vec3 tip = finger[Finger.FingerJoint.JOINT_TIP];

				DrawSphere(mcp, Constants.FingerTipRadius, Colors.White, opacity);
				DrawSphere(pip, Constants.FingerTipRadius, Colors.White, opacity);
				DrawSphere(dip, Constants.FingerTipRadius, Colors.White, opacity);
				DrawSphere(tip, Constants.FingerTipRadius, Colors.White, opacity);

				// Draw finger bones
				string boneColorKey = HelperMethods.GetBoneColorKey(fingerType, Bone.BoneType.TYPE_DISTAL);
				DrawCylinder(Constants.FingerTipRadius, tip, dip, _boneColors[boneColorKey], opacity);
				boneColorKey = HelperMethods.GetBoneColorKey(fingerType, Bone.BoneType.TYPE_INTERMEDIATE);
				DrawCylinder(Constants.FingerTipRadius, dip, pip, _boneColors[boneColorKey], opacity);
				boneColorKey = HelperMethods.GetBoneColorKey(fingerType, Bone.BoneType.TYPE_PROXIMAL);
				DrawCylinder(Constants.FingerTipRadius, pip, mcp, _boneColors[boneColorKey], opacity);
			}

			// Draw hand bones
			LGR_Vec3 indexMCP = hand.FingerJointPositions[Finger.FingerType.TYPE_INDEX][Finger.FingerJoint.JOINT_MCP];
			LGR_Vec3 middleMCP = hand.FingerJointPositions[Finger.FingerType.TYPE_MIDDLE][Finger.FingerJoint.JOINT_MCP];
			LGR_Vec3 ringMCP = hand.FingerJointPositions[Finger.FingerType.TYPE_RING][Finger.FingerJoint.JOINT_MCP];
			LGR_Vec3 pinkyMCP = hand.FingerJointPositions[Finger.FingerType.TYPE_PINKY][Finger.FingerJoint.JOINT_MCP];

			DrawCylinder(Constants.FingerTipRadius, hand.IndexBasePos, indexMCP, _boneColors[Constants.BoneNames.Index_Metacarpal], opacity);
			DrawCylinder(Constants.FingerTipRadius, hand.MiddleBasePos, middleMCP, _boneColors[Constants.BoneNames.Middle_Metacarpal], opacity);
			DrawCylinder(Constants.FingerTipRadius, hand.RingBasePos, ringMCP, _boneColors[Constants.BoneNames.Ring_Metacarpal], opacity);
			DrawCylinder(Constants.FingerTipRadius, hand.PinkyBasePos, pinkyMCP, _boneColors[Constants.BoneNames.Pinky_Metacarpal], opacity);

			// Draw base of hand
			DrawCylinder(Constants.FingerTipRadius, hand.PinkyBasePos, hand.ThumbBasePos, _boneColors[Constants.BoneNames.BaseOfHand], opacity);

			if (showArms)
			{
				// Draw elbow
				DrawSphere(hand.ElbowPos, Constants.WristSphereRadius, _boneColors[Constants.BoneNames.Elbow], opacity);
				// Draw center of forearm
				DrawSphere(hand.ForearmCenter, Constants.WristSphereRadius, _boneColors[Constants.BoneNames.ForearmCenter], opacity);

				float forearmWidth = hand.PinkyBasePos.DistanceTo(hand.ThumbBasePos);

				// Draw two cylinders for arms (Ulna - pinky side, Radius - thumb side)
				LGR_Vec3 ulnaBase = hand.WristPos + ((forearmWidth / 2) * hand.ArmX);
				LGR_Vec3 ulnaTop = hand.ElbowPos + ((forearmWidth / 2) * hand.ArmX);
				DrawSphere(ulnaBase, Constants.FingerTipRadius * 1.2, Colors.CadetBlue, opacity);
				DrawSphere(ulnaTop, Constants.FingerTipRadius * 1.2, Colors.CadetBlue, opacity);
				DrawCylinder(Constants.FingerTipRadius, ulnaBase, ulnaTop, _boneColors[Constants.BoneNames.Arm], opacity);

				LGR_Vec3 radiusBase = hand.WristPos - ((forearmWidth / 2) * hand.ArmX);
				LGR_Vec3 radiusTop = hand.ElbowPos - ((forearmWidth / 2) * hand.ArmX);
				DrawSphere(radiusBase, Constants.FingerTipRadius * 1.2, Colors.CadetBlue, opacity);
				DrawSphere(radiusTop, Constants.FingerTipRadius * 1.2, Colors.CadetBlue, opacity);
				DrawCylinder(Constants.FingerTipRadius, radiusBase, radiusTop, _boneColors[Constants.BoneNames.Arm], opacity);

				// Connect the forearm bones at top and bottom
				DrawCylinder(Constants.FingerTipRadius, ulnaBase, radiusBase, _boneColors[Constants.BoneNames.Arm], opacity);
				DrawCylinder(Constants.FingerTipRadius, ulnaTop, radiusTop, _boneColors[Constants.BoneNames.Arm], opacity);
			}

		}

	}
}
