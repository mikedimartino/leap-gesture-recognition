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
using LeapGestureRecognition.Model;
using System.Windows.Input;

namespace LeapGestureRecognition.ViewModel
{
	public class MainViewModel
	{
		private GRApp _grApp;

		public MainViewModel(OpenGL gl, Controller controller, CustomLeapListener listener)
		{
			_grApp = new GRApp(gl, controller, listener);
		}

		public void OnClosing(object sender, CancelEventArgs e)
		{
			_grApp.OnClosing();
		}

		public void OnMouseWheel(object sender, MouseWheelEventArgs e)
		{
			_grApp.Zoom(e.Delta);
		}

		public void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Middle)
			{
				// TODO: Start moving the camera according to mouse movement.
			}
		}

		public void DrawScene()
		{
			_grApp.DrawScene();
		}

		public void InitOpenGL(OpenGL openGL)
		{
			_grApp.InitOpenGL(openGL);
		}

		public void HandleResize(double width, double height)
		{
			_grApp.HandleResize(width, height);
		}

	}

}
