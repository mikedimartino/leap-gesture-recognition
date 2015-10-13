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
			_vm = new MainViewModel(openGLControl.OpenGL, new Controller(), new CustomLeapListener());
			DataContext = _vm;
			//DataContext = new MainViewModel(new Controller(), new CustomLeapListener());
			//_vm = (MainViewModel)this.DataContext;
			Closing += _vm.OnClosing;
			// Handle mouse wheel event on OpenGl window to zoom
			openGLControl.MouseWheel += _vm.OnMouseWheel;
			//openGLControl.MouseDown += _vm.OnMouseDown;
			Height = 600; //SystemParameters.FullPrimaryScreenHeight;
			Width = 800; //SystemParameters.FullPrimaryScreenWidth;
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
			_vm.HandleResize(Width, Height);
		}

	}
}
