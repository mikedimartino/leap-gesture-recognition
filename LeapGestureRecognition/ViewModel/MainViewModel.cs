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
using LeapGestureRecognition.Model;
using System.Collections.ObjectModel;

namespace LeapGestureRecognition.ViewModel
{
	public class MainViewModel : INotifyPropertyChanged
	{
		private static OpenGL _gl;
		private static Controller _controller;
		private System.Windows.Controls.ScrollViewer _scrollViewer;

		private static Camera _camera;

		private CustomLeapListener _listener;
		private SharpGLHelper _glHelper;

		public MainViewModel(OpenGL gl, System.Windows.Controls.ScrollViewer scrollViewer, Controller controller, CustomLeapListener listener)
		{
			_gl = gl;
			_controller = controller;
			_listener = listener;
			_controller.AddListener(_listener);
			_glHelper = new SharpGLHelper(_gl);
			_camera = new Camera(_gl);
			_scrollViewer = scrollViewer;

			initMenuBar();
		}

		#region Public Properties
		public static double OpenGLWindowWidth { get; set; }
		public static double OpenGLWindowHeight { get; set; }
		public static Frame CurrentFrame { get; set; }
		public ObservableCollection<CustomMenuItem> MenuBar { get; set; }

		private string _outputWindowContent;
		public string OutputWindowContent
		{
			get { return _outputWindowContent; }
			set
			{
				if (_outputWindowContent == value) return;
				_outputWindowContent = value;
				_scrollViewer.ScrollToBottom();
				OnPropertyChanged("OutputWindowContent");
			}
		}

		// Options
		private bool _showAxes = true;
		public bool ShowAxes
		{
			get { return _showAxes; }
			set { _showAxes = value; }
		}

		private bool _showOutputWindow = true;
		public bool ShowOutputWindow
		{
			get { return _showOutputWindow; }
			set 
			{
				_showOutputWindow = value;
				OnPropertyChanged("ShowOutputWindow");
			}
		}

		private bool _showArms = true;
		public bool ShowArms
		{
			get { return _showArms; }
			set { _showArms = value; }
		}
		// END Options
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


		private Point initMiddleClickPosition = new Point();
		// I need to look at the relative position (angle) and not just equally rotate if any X / Y change. 
		public void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				UIElement openGLWindow = e.Source as UIElement;
				if (openGLWindow == null) return; // This probably shouldn't happen, but just in case.

				// Check if cursor has gone off screen
				if (!openGLWindow.IsMouseDirectlyOver)
				{
					_camera.Yaw = 0;
					_camera.Pitch = 0;
				}

				//Point center = new Point(openGLWindow.RenderSize.Width / 2.0, openGLWindow.RenderSize.Height / 2.0);
				Point center = initMiddleClickPosition;
				Point position = e.GetPosition(openGLWindow);

				float deltaX = (float) Math.Abs(initMiddleClickPosition.X - position.X);
				float deltaY = (float) Math.Abs(initMiddleClickPosition.Y - position.Y);

				float scaleFactor = 0.05f;
				deltaX *= scaleFactor;
				deltaY *= scaleFactor;

				// Handle X movement
				if (position.X < center.X) 
				{
					_camera.Yaw = -1;
				}
				else if (position.X > center.X)
				{
					_camera.Yaw = 1;
				}
				else
				{
					_camera.Yaw = 0;
				}
				_camera.Yaw *= deltaX;

				// Handle Y movement
				if (position.Y < center.Y)
				{
					_camera.Pitch = -1;
				}
				else if (position.Y > center.Y)
				{
					_camera.Pitch = 1;
				}
				else
				{
					_camera.Pitch = 0;
				}
				_camera.Pitch *= deltaY;
			}
		}

		public void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Middle)
			{
				WriteToOutputWindow("You clicked the middle mouse button.\n");
				UIElement openGLWindow = e.Source as UIElement;
				initMiddleClickPosition = e.GetPosition(openGLWindow);
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

		public void OnMouseLeaveOpenGLWindow(object sender, MouseEventArgs e)
		{
			_camera.Yaw = 0;
			_camera.Pitch = 0;
		}

		public void OnMouseEnterOpenGLWindow(object sender, MouseEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				UIElement openGLWindow = e.Source as UIElement;
				initMiddleClickPosition = e.GetPosition(openGLWindow);
			}
		}

		public void OnKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Left:
					_camera.Yaw = 1;
					break;
				case Key.Right:
					_camera.Yaw = -1;
					break;
				case Key.Up:
					_camera.Pitch = 1;
					break;
				case Key.Down:
					_camera.Pitch = -1;
					break;
				case Key.LeftCtrl:
					_camera.Roll = 1;
					break;
				case Key.RightCtrl:
					_camera.Roll = -1;
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

			if (_camera.ShouldRotate) _camera.Revolve();
			if (ShowAxes) _glHelper.DrawAxes();

			CurrentFrame = _controller.Frame();
			_glHelper.DrawFrame(CurrentFrame, ShowArms);
		}

		public void WriteToOutputWindow(string message)
		{
			// TODO: Might want to restrict the size of log
			OutputWindowContent += "> " + message;
		}

		public void InitOpenGL(OpenGL openGL)
		{
			//  Set the clear color.
			_gl.ClearColor(0, 0, 0, 0);

			_gl.Enable(OpenGL.GL_BLEND);
			_gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
		}

		public void HandleResize(double width, double height)
		{
			_camera.Width = width;
			_camera.Height = height;
			_camera.UpdateView();
		}
		#endregion

		#region Private Methods
		private void initMenuBar()
		{
			MenuBar = new ObservableCollection<CustomMenuItem>();

			#region OPTIONS
			CustomMenuItem options = new CustomMenuItem("Options");

			CustomMenuItem showAxes = new CustomMenuItem("Show Axes");
			showAxes.IsCheckable = true;
			showAxes.IsChecked = ShowAxes;
			showAxes.Command = new CustomCommand(() => ShowAxes = !ShowAxes);
			options.Items.Add(showAxes);

			//CustomMenuItem showOutputWindow = new CustomMenuItem("Show Output Window");
			//showOutputWindow.IsCheckable = true;
			//showOutputWindow.IsChecked = ShowOutputWindow;
			//showOutputWindow.Command = new CustomCommand(() => ShowOutputWindow = !ShowOutputWindow);
			//options.Items.Add(showOutputWindow);

			CustomMenuItem showArms = new CustomMenuItem("Show Arms");
			showArms.IsCheckable = true;
			showArms.IsChecked = ShowAxes;
			showArms.Command = new CustomCommand(() => ShowArms = !ShowArms);
			options.Items.Add(showArms);
			
			MenuBar.Add(options);
			#endregion

			#region RESET CAMERA
			CustomMenuItem resetCamera = new CustomMenuItem("Reset Camera");
			resetCamera.Command = new CustomCommand(() => _camera.Reset());
			MenuBar.Add(resetCamera);
			#endregion

		}
		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
	}

}
