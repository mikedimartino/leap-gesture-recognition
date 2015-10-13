using Leap;
using LeapGestureRecognition.Util;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeapGestureRecognition.Model
{
	public class GRApp
	{
		private static OpenGL _gl;
		private static Controller _controller;

		private static CameraController _camera;

		private CustomLeapListener _listener;
		private SharpGLHelper _glHelper;

		public GRApp(OpenGL gl, Controller controller, CustomLeapListener listener)
		{
			_gl = gl;
			_controller = controller;
			_listener = listener;
			_controller.AddListener(_listener);
			_glHelper = new SharpGLHelper(_gl);
			_camera = new CameraController(_gl);
		}

		#region Public Properties
		public static double OpenGLWindowWidth { get; set; }
		public static double OpenGLWindowHeight { get; set; }
		public static Frame CurrentFrame { get; set; }
		#endregion

		#region Public Methods
		public void DrawScene()
		{
			//  Clear the color and depth buffer.
			_gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

			CurrentFrame = _controller.Frame();
			_glHelper.DrawFrame(CurrentFrame);
		}

		public void InitOpenGL(OpenGL openGL)
		{
			//  Set the clear color.
			_gl.ClearColor(0, 0, 0, 0);
			// For making InteractionBox walls transparent
			_gl.Enable(OpenGL.GL_BLEND);
			_gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
		}

		public void HandleResize(double width, double height)
		{
			OpenGLWindowWidth = width;
			OpenGLWindowHeight = height;
			_camera.UpdateView();
		}

		public void OnClosing()
		{
			_controller.RemoveListener(_listener);
			_controller.Dispose();
			_listener.Dispose();
		}

		public void Zoom(int delta)
		{
			_camera.Zoom(delta);
		}
		#endregion

	}
}
