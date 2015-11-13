using Leap;
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

		// TODO: Define quadrics for every sphere / cylinder so that new ones don't have to be initialized every time.

		public SharpGLHelper(OpenGL gl)
		{
			_gl = gl;
			_quadric = _gl.NewQuadric();
		}


		public void DrawFrame(Frame frame, bool showArms)
		{
			foreach (Hand hand in frame.Hands)
			{
				float opacity = hand.Confidence;

				// Draw wrist position
				DrawSphere(hand.WristPosition, Constants.WristSphereRadius, Colors.White, opacity);
				// Draw palm position
				DrawSphere(hand.PalmPosition, Constants.PalmSphereRadius, Colors.White, opacity);
				// Connect wrist to palm
				//DrawCylinder(Constants.WristSphereRadius * 0.75, wristPos, palmPos);

				foreach (Finger finger in hand.Fingers)
				{
					// Finger ID = HandID + (0-4) // Where + is concatenation
					//	0 = Thumb, 1 = Index, ..., 4 = Pinky // Can get this with finger.Id % 10
					//if (finger.Id % 10 == 4) continue;

					// Draw finger tips
					DrawSphere(finger.TipPosition, Constants.FingerTipRadius, Colors.White, opacity);

					// Draw joints
					foreach (Finger.FingerJoint jointType in (Finger.FingerJoint[])Enum.GetValues(typeof(Finger.FingerJoint)))
					{
						DrawSphere(finger.JointPosition(jointType), Constants.FingerTipRadius, Colors.White, opacity);
					}

					// Draw bones
					Bone bone;
					foreach (Bone.BoneType boneType in (Bone.BoneType[])Enum.GetValues(typeof(Bone.BoneType)))
					{
						//if (boneType == Bone.BoneType.TYPE_METACARPAL) continue;
						bone = finger.Bone(boneType);
						DrawCylinder(Constants.FingerTipRadius * 0.7, bone.PrevJoint, bone.NextJoint, Constants.BoneColors[boneType], opacity);
					}
				}

				// Draw base of hand (connects the pinky carpal bone to thumb carpal bone)
				Finger pinky = hand.Fingers.Where(f => f.Type == Finger.FingerType.TYPE_PINKY).FirstOrDefault();
				Finger thumb = hand.Fingers.Where(f => f.Type == Finger.FingerType.TYPE_THUMB).FirstOrDefault();
				Vector pinkyBase = pinky.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint;
				Vector thumbBase = thumb.Bone(Bone.BoneType.TYPE_METACARPAL).PrevJoint;
				DrawCylinder(Constants.FingerTipRadius, pinkyBase, thumbBase, Colors.White, opacity);

				if (showArms)
				{
					// Draw arm
					// Draw elbow
					Vector elbowPosition = hand.Arm.ElbowPosition;
					DrawSphere(elbowPosition, Constants.WristSphereRadius, Colors.White, opacity);
					// Draw center of forearm
					Vector centerForearmPosition = hand.Arm.Center;
					DrawSphere(centerForearmPosition, Constants.WristSphereRadius, Colors.White, opacity);

					float forearmWidth = pinkyBase.DistanceTo(thumbBase);
					// Draw two cylinders for arms (Ulna - pinky side, Radius - thumb side)
					Vector ulnaBase = hand.Arm.WristPosition + ((forearmWidth / 2) * hand.Arm.Basis.xBasis);
					Vector ulnaTop = hand.Arm.ElbowPosition + ((forearmWidth / 2) * hand.Arm.Basis.xBasis);
					DrawSphere(ulnaBase, Constants.FingerTipRadius * 1.2, Colors.CadetBlue, opacity);
					DrawSphere(ulnaTop, Constants.FingerTipRadius * 1.2, Colors.CadetBlue, opacity);
					DrawCylinder(Constants.FingerTipRadius, ulnaBase, ulnaTop, Colors.White, opacity);

					Vector radiusBase = hand.Arm.WristPosition - ((forearmWidth / 2) * hand.Arm.Basis.xBasis);
					Vector radiusTop = hand.Arm.ElbowPosition - ((forearmWidth / 2) * hand.Arm.Basis.xBasis);
					DrawSphere(radiusBase, Constants.FingerTipRadius * 1.2, Colors.CadetBlue, opacity);
					DrawSphere(radiusTop, Constants.FingerTipRadius * 1.2, Colors.CadetBlue, opacity);
					DrawCylinder(Constants.FingerTipRadius, radiusBase, radiusTop, Colors.White, opacity);

					// Connect the forearm bones at top and bottom
					DrawCylinder(Constants.FingerTipRadius, ulnaBase, radiusBase, Colors.White, opacity);
					DrawCylinder(Constants.FingerTipRadius, ulnaTop, radiusTop, Colors.White, opacity);
				}
			}
		}

		public void DrawAxes()
		{
			int axisLength = 300;
			double axisRadius = 2;
			Vector origin = new Vector(0,0,0);
			Vector xAxis = new Vector(1,0,0);
			Vector yAxis = new Vector(0,1,0);
			Vector zAxis = new Vector(0,0,1);

			// +X = red, +Y = green, +Z = blue
			DrawCylinder(axisRadius, origin, xAxis * axisLength, Colors.Red, 1.0f);
			DrawCylinder(axisRadius, origin, yAxis * axisLength, Colors.Green, 1.0f);
			DrawCylinder(axisRadius, origin, zAxis * axisLength, Colors.Blue, 1.0f);
		}

		public void DrawCylinder(double radius, Vector basePosition, Vector topPosition, Color color, float opacity)
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


		public void DrawSphere(Vector position, double radius, Color color, float opacity) 
		{
			_gl.LoadIdentity();
			_gl.Color(color.R, color.G, color.B, opacity);
			//position = MapLeapCoordinateToWorldSpace(position);
			_gl.Translate(position.x, position.y, position.z);
			
			IntPtr sphereQuadric = _gl.NewQuadric(); //TODO: Define these somewhere else so new ones aren't initialized every time
			_gl.Sphere(sphereQuadric, radius, 25, 25);
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

		public void DrawInteractionBox(InteractionBox iBox)
		{
			_gl.LoadIdentity();

			float tempX = iBox.Center.x - (iBox.Width / 2.0f);
			float tempY = iBox.Center.y - (iBox.Height / 2.0f);
			float tempZ = iBox.Center.z - (iBox.Depth / 2.0f);
			Vector corner = new Vector(tempX, tempY, tempZ);

			tempX += iBox.Width;
			tempY += iBox.Height;
			tempZ += iBox.Depth;
			Vector oppositeCorner = new Vector(tempX, tempY, tempZ);

			int scaleFactor = 1;
			float xDist = scaleFactor * (iBox.Width / 2.0f);
			float yDist = scaleFactor * (iBox.Height / 2.0f);
			float zDist = scaleFactor * (iBox.Depth / 2.0f);
			

			_gl.Begin(OpenGL.GL_QUADS);
			//_gl.Vertex(xDist, yDist, -zDist);
			// Back face
			_gl.Color(1.0f, 0.0f, 0.0f, 0.1f);
			_gl.Vertex(-xDist, yDist, -zDist);
			_gl.Vertex(-xDist, -yDist, -zDist);
			_gl.Vertex(+xDist, -yDist, -zDist);
			_gl.Vertex(xDist, yDist, -zDist);
			// Left face
			_gl.Color(0.0f, 1.0f, 0.0f, 0.1f);
			_gl.Vertex(-xDist, yDist, -zDist);
			_gl.Vertex(-xDist, -yDist, -zDist);
			_gl.Vertex(-xDist, -yDist, zDist);
			_gl.Vertex(-xDist, yDist, zDist);
			// Right face
			_gl.Color(0.0f, 0.0f, 1.0f, 0.1f);
			_gl.Vertex(xDist, yDist, -zDist);
			_gl.Vertex(xDist, -yDist, -zDist);
			_gl.Vertex(xDist, -yDist, zDist);
			_gl.Vertex(xDist, yDist, zDist);
			// Bottom face
			_gl.Color(0.0f, 1.0f, 1.0f, 0.1f); // cyan
			_gl.Vertex(-xDist, -yDist, -zDist);
			_gl.Vertex(-xDist, -yDist, zDist);
			_gl.Vertex(xDist, -yDist, zDist);
			_gl.Vertex(xDist, -yDist, -zDist);
			// Top face
			//_gl.Color(1.0f, 1.0f, 0.0f, 0.1f); // yellow
			//_gl.Vertex(-xDist, yDist, -zDist);
			//_gl.Vertex(-xDist, yDist, zDist);
			//_gl.Vertex(xDist, yDist, zDist);
			//_gl.Vertex(xDist, yDist, -zDist);
			// Front face
			//_gl.Color(1.0f, 0.0f, 1.0f, 0.1f); // magenta
			//_gl.Vertex(-xDist, yDist, zDist);
			//_gl.Vertex(-xDist, -yDist, zDist);
			//_gl.Vertex(+xDist, -yDist, zDist);
			//_gl.Vertex(xDist, yDist, zDist);

			_gl.End();
		}

		public Vector MapLeapCoordinateToWorldSpace(Vector pos)
		{
			InteractionBox interactionBox = MainViewModel.CurrentFrame.InteractionBox;
			pos = interactionBox.NormalizePoint(pos, false);
			pos.x = (2 * pos.x) - 1;
			pos.y = (2 * pos.y) - 1;
			pos.z = (2 * pos.z) - 1;

			int scaleFactor = 500;
			return scaleFactor * pos;
		}

	}
}
