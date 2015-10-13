using Leap;
using LeapGestureRecognition.Model;
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

		// TODO: Define quadrics for every sphere / cylinder so that new ones don't have to be initialized every time.

		public SharpGLHelper(OpenGL gl)
		{
			_gl = gl;
			_quadric = _gl.NewQuadric();
		}


		public void DrawFrame(Frame frame)
		{
			foreach (Hand hand in frame.Hands)
			{
				// Draw wrist position
				DrawSphere(hand.WristPosition, Constants.WristSphereRadius, Colors.White);
				// Draw palm position
				DrawSphere(hand.PalmPosition, Constants.PalmSphereRadius, Colors.White);
				// Connect wrist to palm
				//DrawCylinder(Constants.WristSphereRadius * 0.75, wristPos, palmPos);
				
				foreach (Finger finger in hand.Fingers)
				{
					// Draw finger tips
					DrawSphere(finger.TipPosition, Constants.FingerTipRadius, Colors.White);

					// Draw joints
					foreach (Finger.FingerJoint jointType in (Finger.FingerJoint[]) Enum.GetValues(typeof(Finger.FingerJoint)))
					{
						DrawSphere(finger.JointPosition(jointType), Constants.FingerTipRadius, Colors.White);
					}

					// Draw bones
					Bone bone;
					foreach (Bone.BoneType boneType in (Bone.BoneType[])Enum.GetValues(typeof(Bone.BoneType)))
					{
						//if (boneType == Bone.BoneType.TYPE_METACARPAL) continue;
						bone = finger.Bone(boneType);
						DrawCylinder(Constants.FingerTipRadius * 0.7, bone.PrevJoint, bone.NextJoint, Constants.BoneColors[boneType]);
					}
				}
			}
		}

		public void DrawCylinder(double radius, Vector basePosition, Vector topPosition, Color color)
		{
			_gl.LoadIdentity();
			_gl.Color(color.R, color.G, color.B);

			basePosition = MapLeapCoordinateToWorldSpace(basePosition);
			topPosition = MapLeapCoordinateToWorldSpace(topPosition);

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


		public void DrawSphere(Vector position, double radius, Color color) // Leap Vector
		{
			_gl.LoadIdentity();
			_gl.Color(color.R, color.G, color.B);

			position = MapLeapCoordinateToWorldSpace(position);
			_gl.Translate(position.x, position.y, position.z);
			
			IntPtr sphereQuadric = _gl.NewQuadric(); //TODO: Define these somewhere else so new ones aren't initialized every time
			_gl.Sphere(sphereQuadric, radius, 25, 25);
		}

		public Vector MapLeapCoordinateToWorldSpace(Vector pos)
		{
			InteractionBox interactionBox = GRApp.CurrentFrame.InteractionBox;
			pos = interactionBox.NormalizePoint(pos);
			pos.x = (2 * pos.x) - 1;
			pos.y = (2 * pos.y) - 1;
			pos.z = (2 * pos.z) - 1;

			int scaleFactor = 500;
			return scaleFactor * pos;
		}

	}
}
