﻿using System;
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

namespace LeapGestureRecognition.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		MainViewModel _vm;

		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
			_vm = new MainViewModel(openGLControl.OpenGL, outputWindowScrollViewer, outputWindowTextBox, 
				gestureLibraryControl, editStaticGestureControl, editDynamicGestureControl, 
				recognitionMonitorControl, new Controller());
			DataContext = _vm;
			Closing += _vm.OnClosing;
			// Handle mouse wheel event on OpenGl window to zoom
			openGLControl.MouseWheel += _vm.OnMouseWheel;
			KeyDown += _vm.OnKeyDown;
			KeyUp += _vm.OnKeyUp;
			openGLControl.MouseMove += _vm.OnMouseMoveOverOpenGLWindow;
			MouseUp += _vm.OnMouseUp;
			openGLControl.MouseDown += _vm.OnMouseDown;
			openGLControl.MouseLeave += _vm.OnMouseLeaveOpenGLWindow;
			openGLControl.MouseEnter += _vm.OnMouseEnterOpenGLWindow;
			openGLControl.FrameRate = Constants.FrameRate;

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

		private void StaticTabClicked(object sender, MouseButtonEventArgs e)
		{
			staticTab.Background = Constants.ActiveTabBrush;
			dynamicTab.Background = Brushes.Transparent;
			_vm.OnStaticTabClicked();
		}

		private void DynamicTabClicked(object sender, MouseButtonEventArgs e)
		{
			dynamicTab.Background = Constants.ActiveTabBrush;
			staticTab.Background = Brushes.Transparent;
			_vm.OnDynamicTabClicked();
		}
		
	}
}
