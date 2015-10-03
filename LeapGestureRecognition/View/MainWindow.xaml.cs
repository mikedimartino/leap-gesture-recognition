using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpGL.SceneGraph;
using SharpGL;
using LeapGestureRecognition.ViewModel;
using Leap;
using LeapGestureRecognition.Util;

namespace LeapGestureRecognition
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private MainViewModel _vm;

		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
			_vm = new MainViewModel(new Controller(), new CustomLeapListener());
			DataContext = _vm;
			//DataContext = new MainViewModel(new Controller(), new CustomLeapListener());
			//_vm = (MainViewModel)this.DataContext;
			Closing += _vm.OnClosing;
		}

		/// <summary>
		/// Handles the OpenGLDraw event of the openGLControl1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
		private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
		{
			_vm.DrawScene();
		}

		/// <summary>
		/// Handles the OpenGLInitialized event of the openGLControl1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
		private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
		{
			_vm.InitOpenGL(openGLControl.OpenGL);
		}

		/// <summary>
		/// Handles the Resized event of the openGLControl1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
		private void openGLControl_Resized(object sender, OpenGLEventArgs args)
		{
			//  TODO: Set the projection matrix here.

			//  Get the OpenGL object.
			OpenGL gl = openGLControl.OpenGL;

			//  Set the projection matrix.
			gl.MatrixMode(OpenGL.GL_PROJECTION);

			//  Load the identity.
			gl.LoadIdentity();

			//  Create a perspective transformation.
			gl.Perspective(60.0f, (double)Width / (double)Height, 0.01, 100.0);

			//  Use the 'look at' helper function to position and aim the camera.
			gl.LookAt(-5, 5, -5, 0, 0, 0, 0, 1, 0);

			//  Set the modelview matrix.
			gl.MatrixMode(OpenGL.GL_MODELVIEW);
		}
	}
}
