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
using System.Windows.Input;
using System.Windows;

namespace LeapGestureRecognition.ViewModel
{
	public class MainViewModel
	{
		private static OpenGL _gl;
		private static Controller _controller;

		private static CameraController _camera;

		private CustomLeapListener _listener;
		private SharpGLHelper _glHelper;

		public MainViewModel(OpenGL gl, Controller controller, CustomLeapListener listener)
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
		public void OnClosing(object sender, CancelEventArgs e)
		{
			_controller.RemoveListener(_listener);
			_controller.Dispose();
			_listener.Dispose();
		}

		public void OnMouseWheel(object sender, MouseWheelEventArgs e)
		{
			_camera.Zoom(e.Delta);
		}


		// I need to look at the relative position (angle) and not just equally rotate if any X / Y change. 
		public void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				UIElement openGLWindow = e.Source as UIElement;
				if (openGLWindow == null) return; // This probably shouldn't happen, but just in case.
				Point center = new Point(openGLWindow.RenderSize.Width / 2.0, openGLWindow.RenderSize.Height / 2.0);
				Point position = e.GetPosition(openGLWindow);
				// Handle X movement
				if (position.X < center.X) _camera.Yaw = -1;
				else if (position.X > center.X) _camera.Yaw = 1;
				else _camera.Yaw = 0;
				// Handle Y movement
				if (position.Y < center.Y) _camera.Pitch = -1;
				else if (position.Y > center.Y) _camera.Pitch = 1;
				else _camera.Yaw = 0;
			}
		}

		public void OnMouseUp(object sender, MouseButtonEventArgs e) 
		{
			if (e.ChangedButton == MouseButton.Middle)
			{
				// Might want to check if the arrow (left, right, up, down) keys are pressed, and ignore if so.
				_camera.Yaw = 0;
				_camera.Pitch = 0;
			}
		}


		public void OnKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Left:
					_camera.Yaw = -1;
					break;
				case Key.Right:
					_camera.Yaw = 1;
					break;
				case Key.Up:
					_camera.Pitch = -1;
					break;
				case Key.Down:
					_camera.Pitch = 1;
					break;
				case Key.LeftCtrl:
					_camera.Roll = -1;
					break;
				case Key.RightCtrl:
					_camera.Roll = 1;
					break;
			}
		}

		public void OnKeyUp(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Left:
				case Key.Right:
					_camera.Yaw = 0;
					break;
				case Key.Up:
				case Key.Down:
					_camera.Pitch = 0;
					break;
				case Key.LeftCtrl:
				case Key.RightCtrl:
					_camera.Roll = 0;
					break;
			}
		}

		public void DrawScene()
		{
			//  Clear the color and depth buffer.
			_gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

			if (_camera.ShouldRotate) _camera.Rotate();

			CurrentFrame = _controller.Frame();
			_glHelper.DrawFrame(CurrentFrame);
		}

		public void InitOpenGL(OpenGL openGL)
		{
			//  Set the clear color.
			_gl.ClearColor(0, 0, 0, 0);
			// For making InteractionBox walls transparent
			//_gl.Enable(OpenGL.GL_BLEND);
			//_gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
		}

		public void HandleResize(double width, double height)
		{
			_camera.Width = width;
			_camera.Height = height;
			_camera.UpdateView();
		}
		#endregion

	}

}
