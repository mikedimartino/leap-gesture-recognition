﻿using Leap;
using LeapGestureRecognition.ViewModel;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LeapGestureRecognition
{
	public class SharpGLHelper
	{
		private OpenGL _gl;
		private Dictionary<string, Color> _boneColors;

		// TODO: Define quadrics for every sphere / cylinder so that new ones don't have to be initialized every time.

		public SharpGLHelper(OpenGL gl, Dictionary<string, Color> boneColors)
		{
			_gl = gl;
			_boneColors = boneColors;
		}

		public void DrawFrame(Frame frame, bool showArms)
		{
			foreach (Hand hand in frame.Hands)
			{
				float opacity = hand.Confidence;
				DrawHand(new SGInstanceSingleHand(hand), showArms, opacity);
			}
		}

		public void DrawAxes()
		{
			int axisLength = 300;
			double axisRadius = 2;
			Vec3 origin = new Vec3(0, 0, 0);
			Vec3 xAxis = new Vec3(1, 0, 0);
			Vec3 yAxis = new Vec3(0, 1, 0);
			Vec3 zAxis = new Vec3(0, 0, 1);

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
			_gl.DeleteQuadric(cylQuadric);
		}

		public void DrawCylinder(double radius, Vec3 basePosition, Vec3 topPosition, Color color, float opacity = 1)
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
			_gl.DeleteQuadric(sphereQuadric);
		}

		public void DrawSphere(Vec3 position, double radius, Color color, float opacity = 1)
		{
			DrawSphere(position.ToLeapVector(), radius, color, opacity);
		}

		// Not used, but example of how to draw a pyramid.
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

		public void DrawStaticGesture(SGInstance gesture, bool showArms = true) 
		{
			foreach (var hand in gesture.Hands)
			{
				DrawHand(hand, showArms);
			}
		}

		// Diagram of hand: https://blog.leapmotion.com/wp-content/uploads/2014/05/boneapi1.png
		public void DrawHand(SGInstanceSingleHand hand, bool showArms, float opacity = 1)
		{
			// Draw wrist position
			DrawSphere(hand.WristPos_World, Constants.WristSphereRadius, _boneColors[Constants.BoneNames.Wrist], opacity);
			// Draw palm position
			DrawSphere(hand.PalmPosition, Constants.PalmSphereRadius, _boneColors[Constants.BoneNames.Palm], opacity);

			foreach (var fjp in hand.FingerJointPositions_World)
			{
				var finger = fjp.Value;
				var fingerType = fjp.Key;

				Vec3 mcp = finger[Finger.FingerJoint.JOINT_MCP];
				Vec3 pip = finger[Finger.FingerJoint.JOINT_PIP];
				Vec3 dip = finger[Finger.FingerJoint.JOINT_DIP];
				Vec3 tip = finger[Finger.FingerJoint.JOINT_TIP];

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
			Vec3 indexMCP = hand.FingerJointPositions_World[Leap.Finger.FingerType.TYPE_INDEX][Finger.FingerJoint.JOINT_MCP];
			Vec3 middleMCP = hand.FingerJointPositions_World[Leap.Finger.FingerType.TYPE_MIDDLE][Finger.FingerJoint.JOINT_MCP];
			Vec3 ringMCP = hand.FingerJointPositions_World[Leap.Finger.FingerType.TYPE_RING][Finger.FingerJoint.JOINT_MCP];
			Vec3 pinkyMCP = hand.FingerJointPositions_World[Leap.Finger.FingerType.TYPE_PINKY][Finger.FingerJoint.JOINT_MCP];

			DrawCylinder(Constants.FingerTipRadius, hand.IndexBasePos_World, indexMCP, _boneColors[Constants.BoneNames.Index_Metacarpal], opacity);
			DrawCylinder(Constants.FingerTipRadius, hand.MiddleBasePos_World, middleMCP, _boneColors[Constants.BoneNames.Middle_Metacarpal], opacity);
			DrawCylinder(Constants.FingerTipRadius, hand.RingBasePos_World, ringMCP, _boneColors[Constants.BoneNames.Ring_Metacarpal], opacity);
			DrawCylinder(Constants.FingerTipRadius, hand.PinkyBasePos_World, pinkyMCP, _boneColors[Constants.BoneNames.Pinky_Metacarpal], opacity);

			// Draw base of hand
			DrawCylinder(Constants.FingerTipRadius, hand.PinkyBasePos_World, hand.ThumbBasePos_World, _boneColors[Constants.BoneNames.BaseOfHand], opacity);

			if (showArms)
			{
				// Draw elbow
				DrawSphere(hand.ElbowPos_World, Constants.WristSphereRadius, _boneColors[Constants.BoneNames.Elbow], opacity);
				// Draw center of forearm
				DrawSphere(hand.ForearmCenter_World, Constants.WristSphereRadius, _boneColors[Constants.BoneNames.ForearmCenter], opacity);

				float forearmWidth = hand.PinkyBasePos_World.DistanceTo(hand.ThumbBasePos_World);

				// Draw two cylinders for arms (Ulna - pinky side, Radius - thumb side)
				Vec3 ulnaBase = hand.WristPos_World + ((forearmWidth / 2) * hand.ArmX);
				Vec3 ulnaTop = hand.ElbowPos_World + ((forearmWidth / 2) * hand.ArmX);
				DrawSphere(ulnaBase, Constants.FingerTipRadius * 1.2, Colors.CadetBlue, opacity);
				DrawSphere(ulnaTop, Constants.FingerTipRadius * 1.2, Colors.CadetBlue, opacity);
				DrawCylinder(Constants.FingerTipRadius, ulnaBase, ulnaTop, _boneColors[Constants.BoneNames.Arm], opacity);

				Vec3 radiusBase = hand.WristPos_World - ((forearmWidth / 2) * hand.ArmX);
				Vec3 radiusTop = hand.ElbowPos_World - ((forearmWidth / 2) * hand.ArmX);
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
