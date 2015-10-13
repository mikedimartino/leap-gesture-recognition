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

namespace LeapGestureRecognition.ViewModel
{
	public class MainViewModel
	{
		private GRApp _grApp;

		public MainViewModel(OpenGL gl, Controller controller, CustomLeapListener listener)
		{
			_grApp = new GRApp(gl, controller, listener);
		}

		public void OnClosing(object sender, CancelEventArgs cancelEventArgs)
		{
			_grApp.OnClosing();
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
