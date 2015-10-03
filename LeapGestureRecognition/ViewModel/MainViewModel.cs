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

namespace LeapGestureRecognition.ViewModel
{
	public class MainViewModel
	{
		private float _rotation = 0.0f;
		private OpenGL _gl = null;
		private IntPtr _sphereQuadric;
		private IntPtr _cylinderQuadric;

		private Controller _controller;
		private CustomLeapListener _listener;

		public MainViewModel (Controller controller, CustomLeapListener listener)
		{
			_controller = controller;
			_listener = listener;
			_controller.AddListener(_listener);
		}

		~MainViewModel()
		{
			_controller.Dispose();
		}

		public void OnClosing(object sender, CancelEventArgs cancelEventArgs)
		{
			_controller.RemoveListener(_listener);
			_controller.Dispose();
			_listener.Dispose();
		}

		 
		#region Properties
		private string _testString = "this a test string";
		public string TestString
		{ 
			get { return _testString; }
			set { _testString = value; }
		}
		#endregion

		public void DrawScene()
		{
			//  Clear the color and depth buffer.
			_gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

			Frame frame = _controller.Frame();

			int numHands = frame.Hands.Count;
			if (numHands == 0) return;
			else if (numHands == 2)
			{
				drawTwoCylinders();
				return;
			}

			Hand hand = frame.Hands.Rightmost;
			var pn = hand.PalmNormal;
			bool palmFacingUp = pn.y > 0;
			if (palmFacingUp) drawRotatingPyramid();
			else drawSphere();
		}

		#region temp draw methods
		private void drawRotatingPyramid()
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

		private void drawSphere()
		{
			_gl.LoadIdentity();
			_gl.Color(0.0, 0.0, 1.0);
			_gl.Sphere(_sphereQuadric, 1.0, 25, 25);
		}

		private void drawTwoCylinders()
		{
			// Left cylinder
			_gl.LoadIdentity();
			_gl.Translate(-2, 0, 0);
			_gl.Color(0.0, 0.0, 1.0);
			_gl.Cylinder(_cylinderQuadric, 1.0, 1.0, 3.0, 20, 20);

			// Right cylinder
			_gl.LoadIdentity();
			_gl.Translate(2, 0, 0);
			_gl.Color(0.0, 1.0, 0.0);
			_gl.Cylinder(_cylinderQuadric, 1.5, 1.0, 3.0, 20, 20);
		}
		#endregion


		public void InitOpenGL(OpenGL openGL)
		{
			_gl = openGL;
			_sphereQuadric = _gl.NewQuadric();
			_cylinderQuadric = _gl.NewQuadric();
			//  Set the clear color.
			_gl.ClearColor(0, 0, 0, 0);
		}
	}

}
