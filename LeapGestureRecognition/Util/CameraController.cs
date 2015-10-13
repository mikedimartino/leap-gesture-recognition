using Leap;
using LeapGestureRecognition.Model;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeapGestureRecognition.Util
{
	public class CameraController
	{
		private OpenGL _gl;
		private Vector _eye, _at, _up;

		public static float FOVY = 60.0f;
		public static double NearClipPlaneDist = 0.01;
		public static double FarClipPlaneDist = 10000;
		public static Vector EyeDefault = new Vector(0, 2000, 2000);
		public static Vector AtDefault = new Vector(0, 0, 0);
		public static Vector UpDefault = new Vector(0, 1, 0);

		public CameraController(OpenGL gl)
		{
			_gl = gl;
			_eye = EyeDefault;
			_at = AtDefault;
			_up = UpDefault;
		}

		public void UpdateView()
		{
			//  Set the projection matrix.
			_gl.MatrixMode(OpenGL.GL_PROJECTION);

			//  Load the identity.
			_gl.LoadIdentity();

			double width = GRApp.OpenGLWindowWidth;
			double height = GRApp.OpenGLWindowHeight;
			//  Create a perspective transformation.
			_gl.Perspective(FOVY, width / height, NearClipPlaneDist, FarClipPlaneDist);

			//  Use the 'look at' helper function to position and aim the camera.
			_gl.LookAt(_eye.x, _eye.y, _eye.z, _at.x, _at.y, _at.z, _up.x, _up.y, _up.z);

			_gl.Frustum(-width / 2, width / 2, height / 2, -height / 2, -1, 1);

			//  Set the modelview matrix.
			_gl.MatrixMode(OpenGL.GL_MODELVIEW);
		}

	}
}
