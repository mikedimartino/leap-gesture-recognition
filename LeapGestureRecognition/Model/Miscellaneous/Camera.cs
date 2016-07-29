using Leap;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlmNet;

namespace LeapGestureRecognition
{
	public class Camera
	{
		private OpenGL _gl;

		private float _radians = -0.02f;

		private float _initYawRadians = 0;
		private float _initPitchRadians = -0.19f;

		private mat4 _orientationMatrix;
		private mat4 OrientationMatrix
		{
			get { return _orientationMatrix; }
			set { _orientationMatrix = value; }
		}

		public static float FOVY = 60.0f;
		public static float NearClipPlaneDist = 0.01f;
		public static float FarClipPlaneDist = 10000;
		public static vec4 RightDefault = new vec4(1, 0, 0, 0);
		public static vec4 UpDefault = new vec4(0, 1, 0, 0);
		public static vec4 AtDefault = new vec4(0, 0, 0, 0);
		public static vec4 EyeDefault = new vec4(0, 300, 825.6f, 1);//new vec4(0, 202, 825.6f, 1);
		public static int ZoomStep = 50;

		public Camera(OpenGL gl)
		{
			_gl = gl;
			OrientationMatrix = new mat4(RightDefault, UpDefault, AtDefault , EyeDefault);
			yawRadians = _initYawRadians;
			pitchRadians = _initPitchRadians;
			Revolve();
		}

		public double Width { get; set; }
		public double Height { get; set; }

		public float Pitch { get; set; }
		public float Yaw { get; set; }
		public float Roll { get; set; }

		public bool ShouldRotate
		{
			get { return Pitch != 0 || Yaw != 0 || Roll != 0; }
		}

		
		public vec4 Right
		{
			get { return _orientationMatrix[0]; }
			set { _orientationMatrix[0] = value; }
		}
		public vec4 Up
		{
			get { return _orientationMatrix[1]; }
			set { _orientationMatrix[1] = value; }
		}
		public vec4 At
		{
			get { return _orientationMatrix[2]; }
			set { _orientationMatrix[2] = value; }
		}
		public vec4 Eye
		{
			get { return _orientationMatrix[3]; }
			set { _orientationMatrix[3] = value; }
		}

		public float  DistanceFromOrigin
		{
			get
			{
				return (float) Math.Sqrt(Math.Pow(Eye.x, 2) + Math.Pow(Eye.y, 2) + Math.Pow(Eye.z, 2));
			}
		}

		
		public void UpdateView()
		{
			//  Set the projection matrix.
			_gl.MatrixMode(OpenGL.GL_PROJECTION);

			//  Load the identity.
			_gl.LoadIdentity();

			//  Create a perspective transformation.
			_gl.Perspective(FOVY, Width / Height, NearClipPlaneDist, FarClipPlaneDist);

			//  Use the 'look at' helper function to position and aim the camera.
			_gl.LookAt(Eye.x, Eye.y, Eye.z, At.x, At.y, At.z, Up.x, Up.y, Up.z);

			_gl.Frustum(-Width / 2, Width / 2, Height / 2, -Height / 2, -1, 1);

			//  Set the modelview matrix.
			_gl.MatrixMode(OpenGL.GL_MODELVIEW);
		}

		public void Zoom(int delta)
		{
			vec4 zoom = glm.normalize(Eye) * ZoomStep;
			// -Z is into the screen and scroll up gives positive delta, so flip sign.
			if (delta > 0) zoom *= -1;
			Eye += zoom;
			UpdateView();
		}

		private float yawRadians = 0;
		private float pitchRadians = -0.2f;
		public void Revolve()
		{
			mat4 newOrientationMatrix = mat4.identity();

			yawRadians += _radians * Yaw;
			pitchRadians += _radians * Pitch;

			newOrientationMatrix = glm.rotate(newOrientationMatrix, pitchRadians, new vec3(newOrientationMatrix[0]));
			newOrientationMatrix = glm.rotate(newOrientationMatrix, yawRadians, new vec3(0, 1, 0));
			
			newOrientationMatrix = glm.translate(newOrientationMatrix, DistanceFromOrigin * new vec3(0, 0, 1));
			// Update camera position (since we're doing arcball)
			OrientationMatrix = newOrientationMatrix;

			UpdateView();
		}

		// Resets camera to initial orientation
		public void Reset()
		{
			OrientationMatrix = new mat4(RightDefault, UpDefault, AtDefault, Eye);
			yawRadians = _initYawRadians;
			pitchRadians = _initPitchRadians;
			Revolve();

			UpdateView();
		}

	}
}
