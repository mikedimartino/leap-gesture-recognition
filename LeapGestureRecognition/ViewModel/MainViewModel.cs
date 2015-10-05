using SharpGL;
using SharpGL.SceneGraph.Quadrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Leap;
using LeapGestureRecognition.Util;
using System.ComponentModel;
using GlmNet;

namespace LeapGestureRecognition.ViewModel
{
	public class MainViewModel
	{
		private static OpenGL _gl;
		private static Controller _controller;

		private CustomLeapListener _listener;
		private SharpGLHelper _glHelper;

		public MainViewModel(OpenGL gl, Controller controller, CustomLeapListener listener)
		{
			_gl = gl;
			_controller = controller;
			_listener = listener;
			_controller.AddListener(_listener);
			_glHelper = new SharpGLHelper(_gl);
		}

		#region Public Properties
		public static OpenGL GL
		{
			get { return _gl; }
		}

		public static Controller Controller
		{
			get { return _controller; }
		}
		#endregion

		public void OnClosing(object sender, CancelEventArgs cancelEventArgs)
		{
			_controller.RemoveListener(_listener);
			_controller.Dispose();
			_listener.Dispose();
		}

		public void DrawScene()
		{
			//  Clear the color and depth buffer.
			_gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

			Frame frame = _controller.Frame();
			_glHelper.DrawFrame(frame);
		}
		 


		public void InitOpenGL(OpenGL openGL)
		{
			//  Set the clear color.
			_gl.ClearColor(0, 0, 0, 0);
		}

		public void HandleResize(double width, double height)
		{

			//  Set the projection matrix.
			_gl.MatrixMode(OpenGL.GL_PROJECTION);

			//  Load the identity.
			_gl.LoadIdentity();

			//  Create a perspective transformation.
			_gl.Perspective(60.0f, width / height, 0.01, 100.0);

			//  Use the 'look at' helper function to position and aim the camera.
			//_gl.LookAt(-5, 5, -5, 0, 0, 0, 0, 1, 0);
			_gl.LookAt(0, 5, 5, 0, 0, 0, 0, 1, 0);

			//_gl.Frustum(0, width, height, 0, -1, 1);
			_gl.Frustum(-width / 2, width/2, height/2, -height/2, -1, 1);



			//  Set the modelview matrix.
			_gl.MatrixMode(OpenGL.GL_MODELVIEW);
			
		}

		//private vec3 get3dPoint(vec2 point2d, int width, int height, mat4 viewMatrix, mat4 projectionMatrix)
		//{

		//}

		
	}

}
