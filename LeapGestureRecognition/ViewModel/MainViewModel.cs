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
using LeapGestureRecognition.View;

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
		private SQLiteProvider _sqliteProvider;
		LGR_Configuration _config;


		public MainViewModel(OpenGL gl, System.Windows.Controls.ScrollViewer scrollViewer, Controller controller, CustomLeapListener listener)
		{
			_gl = gl;
			_controller = controller;
			_listener = listener;
			_controller.AddListener(_listener);
			_camera = new Camera(_gl);
			_scrollViewer = scrollViewer;
			_sqliteProvider = new SQLiteProvider(Constants.SQLiteFileName);
			_config = new LGR_Configuration(_sqliteProvider);
			_glHelper = new SharpGLHelper(_gl, _config.BoneColors);

			updateGestureLibrary();
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

		private LGR_Mode _Mode = LGR_Mode.Default;
		public LGR_Mode Mode
		{
			get { return _Mode; }
			set { _Mode = value; } // May want to add OnPropertyChanged()
		}

		public LGR_Configuration Config
		{
			get { return _config; }
			set
			{
				_config = value;
				//OnPropertyChanged("Config");
			}
		}

		private ObservableCollection<SingleHandGestureStatic> _staticGestures = new ObservableCollection<SingleHandGestureStatic>();
		public ObservableCollection<SingleHandGestureStatic> StaticGestures 
		{
			get { return _staticGestures; }
			set
			{
				_staticGestures = value;
				OnPropertyChanged("StaticGestures");
				updateGestureLibraryMenu(); // Should move this... will continuously clear GestureLibraryMenuItems unnecessarily 
			}
		}

		private void updateGestureLibraryMenu()
		{
			GestureLibraryMenuItems.Clear();
			foreach (var gesture in StaticGestures)
			{
				GestureLibraryMenuItems.Add(new CustomMenuItem(gesture.Name));
			}
		}

		private ObservableCollection<CustomMenuItem> _GestureLibraryMenuItems = new ObservableCollection<CustomMenuItem>();
		public ObservableCollection<CustomMenuItem> GestureLibraryMenuItems
		{
			get { return _GestureLibraryMenuItems; }
			set
			{
				_GestureLibraryMenuItems = value;
				OnPropertyChanged("GestureLibraryMenuItems");
			}
		}

		public LGR_User ActiveUser { get { return _config.ActiveUser; } }


		#region Options
		public bool ShowAxes
		{
			get { return _config.BoolOptions[Constants.BoolOptionsNames.ShowAxes]; }
		}

		public bool ShowArms
		{
			get { return _config.BoolOptions[Constants.BoolOptionsNames.ShowArms]; }
		}

		public bool ShowOutputWindow
		{
			get { return _config.BoolOptions[Constants.BoolOptionsNames.ShowOutputWindow]; }
		}

		public bool ShowGestureLibrary
		{
			get { return _config.BoolOptions[Constants.BoolOptionsNames.ShowGestureLibrary]; }
		}
		#endregion

		#endregion

		#region Public Methods

		#region Event Handling Methods
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

				case Key.A:
					takeSnapshotOfHands();
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
		#endregion

		private SingleHandGestureStatic SelectedGesture = null;

		private void takeSnapshotOfHands()
		{
			Hand hand = _controller.Frame().Hands.FirstOrDefault();
			if (hand == null) return;

			//LGR_SingleHandStaticGesture test = new LGR_SingleHandStaticGesture(hand);
			//int avgId = 1;
			//_sqliteProvider.SaveNewGestureInstance(test, avgId);

			SelectedGesture = new SingleHandGestureStatic(hand);
			Mode = LGR_Mode.Playback;
			DisplaySaveGestureDialog(SelectedGesture);
			Mode = LGR_Mode.Default;
		}

		// Measures hand on screen
		public LGR_HandMeasurements MeasureHand()
		{
			Hand hand = _controller.Frame().Hands.FirstOrDefault();
			if (hand == null) return null;

			return new SingleHandGestureStatic(hand).GetMeasurements();


			//SingleHandGestureStatic handGesture = new SingleHandGestureStatic(hand);
			//LGR_HandMeasurements measurements = new LGR_HandMeasurements();
			//measurements.PinkyLength = handGesture.PalmCenter.DistanceTo(handGesture.FingerJointPositions[Finger.FingerType.TYPE_PINKY][Finger.FingerJoint.JOINT_TIP]);
			//measurements.RingLength = handGesture.PalmCenter.DistanceTo(handGesture.FingerJointPositions[Finger.FingerType.TYPE_RING][Finger.FingerJoint.JOINT_TIP]);
			//measurements.MiddleLength = handGesture.PalmCenter.DistanceTo(handGesture.FingerJointPositions[Finger.FingerType.TYPE_MIDDLE][Finger.FingerJoint.JOINT_TIP]);
			//measurements.IndexLength = handGesture.PalmCenter.DistanceTo(handGesture.FingerJointPositions[Finger.FingerType.TYPE_INDEX][Finger.FingerJoint.JOINT_TIP]);
			//measurements.ThumbLength = handGesture.PalmCenter.DistanceTo(handGesture.FingerJointPositions[Finger.FingerType.TYPE_THUMB][Finger.FingerJoint.JOINT_TIP]);
			//return measurements;
		}

		public void DrawScene()
		{
			//  Clear the color and depth buffer.
			_gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

			if (_camera.ShouldRotate) _camera.Revolve();
			if (ShowAxes) _glHelper.DrawAxes();

			CurrentFrame = _controller.Frame();

			switch (Mode)
			{
				case LGR_Mode.Playback:
					_glHelper.DrawHand(SelectedGesture, ShowArms);
					break;
				default:
					_glHelper.DrawFrame(CurrentFrame, ShowArms);
					break;
			}
		}

		public void DisplayGesture(SingleHandGestureStatic gesture)
		{
			Mode = LGR_Mode.Playback;
			SelectedGesture = gesture;
		}

		public void DeleteGesture(string name)
		{
			_sqliteProvider.DeleteGesture(name);
			updateGestureLibrary();
			updateGestureLibraryMenu();
		}

		public void DeleteUser(LGR_User user)
		{
			_sqliteProvider.DeleteUser(user.Id);
		}

		#region Dialog Windows
		public void DisplaySaveGestureDialog(SingleHandGestureStatic gesture)
		{
			SaveGestureDialog saveNewGestureDialog = new SaveGestureDialog();
			while (saveNewGestureDialog.ShowDialog() == true)
			{
				string name = saveNewGestureDialog.EnteredName;
				if (string.IsNullOrWhiteSpace(name))
				{
					string errorMsg = String.Format("Error: Name cannot be whitespace.", name);
					WriteLineToOutputWindow(errorMsg);
					saveNewGestureDialog = new SaveGestureDialog(errorMsg);
					continue;
				}
				if (_sqliteProvider.GestureExists(name))
				{
					string errorMsg = String.Format("Error: A gesture named '{0}' already exists.", name);
					WriteLineToOutputWindow(errorMsg);
					saveNewGestureDialog = new SaveGestureDialog(errorMsg);
				}
				else
				{
					_sqliteProvider.SaveGesture(name, gesture);
					StaticGestures.Add(gesture);
					WriteLineToOutputWindow(String.Format("Successfully saved gesture '{0}'.", name));
					break;
				}
			}
		}

		public void DisplayRenameGestureDialog(string oldName)
		{
			RenameGestureDialog renameGestureDialog = new RenameGestureDialog(oldName);
			while (renameGestureDialog.ShowDialog() == true)
			{
				string newName = renameGestureDialog.EnteredName;
				if (newName == oldName) return;

				if (string.IsNullOrWhiteSpace(newName))
				{
					string errorMsg = String.Format("Error: Name cannot be whitespace.", newName);
					WriteLineToOutputWindow(errorMsg);
					renameGestureDialog = new RenameGestureDialog(oldName, errorMsg);
					continue;
				}

				if (_sqliteProvider.GestureExists(newName))
				{
					string errorMsg = String.Format("Error: A gesture named '{0}' already exists.", newName);
					WriteLineToOutputWindow(errorMsg);
					renameGestureDialog = new RenameGestureDialog(oldName, errorMsg);
				}
				else
				{
					_sqliteProvider.RenameGesture(oldName, newName);
					WriteLineToOutputWindow(String.Format("Successfully renamed '{0}' to '{1}'.", oldName, newName));
					updateGestureLibrary();
					//updateGestureLibraryMenu(); // Handled in StaticGestures setter
					break;
				}
			}
		}

		public void DisplayOptionsDialog()
		{
			OptionsDialog optionsDialog = new OptionsDialog(this);
			if (optionsDialog.ShowDialog() == true)
			{
				// General Options
				foreach (var boolOption in optionsDialog.Changeset.BoolOptionsChangeset)
				{
					_sqliteProvider.UpdateBoolOption(boolOption.Key, boolOption.Value);
					_config.BoolOptions[boolOption.Key] = boolOption.Value;
				}
				// Bone Colors
				foreach (var boneColor in optionsDialog.Changeset.BoneColorsChangeset)
				{
					_sqliteProvider.UpdateBoneColor(boneColor.Key, boneColor.Value);
					_config.BoneColors[boneColor.Key] = boneColor.Value;
				}
				// Users
				LGR_User newActiveUser = optionsDialog.Changeset.ActiveUser;
				if (newActiveUser != null && newActiveUser.Id != _config.ActiveUser.Id)
				{
					_sqliteProvider.SetActiveUser(newActiveUser.Id);
					_config.ActiveUser = newActiveUser;
					OnPropertyChanged("ActiveUser");
				}
				foreach (var newUser in optionsDialog.Changeset.NewUsers)
				{
					_sqliteProvider.SaveNewUser(newUser);
				}
				foreach (var modifiedUser in optionsDialog.Changeset.ModifiedUsers)
				{
					_sqliteProvider.UpdateUser(modifiedUser);
				}
				foreach (int userId in optionsDialog.Changeset.DeletedUserIds)
				{
					_sqliteProvider.DeleteUser(userId);
				}

				_config.AllUsers = _sqliteProvider.GetAllUsers(); // Just to make sure it's up to date  
			}
			//OnPropertyChanged(""); // Refresh all bindings (needed for ShowGestureLibrary and ShowOutputWindow).
		}
		#endregion

		#region Output Window
		public void WriteLineToOutputWindow(string message)
		{
			WriteToOutputWindow(message + "\n");
		}

		public void WriteToOutputWindow(string message)
		{
			// TODO: Might want to restrict the size of log
			OutputWindowContent += "> " + message;
		}
		#endregion

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

		/// <summary>
		/// Saves the new gesture if name is unique.
		/// </summary>
		/// <param name="name">The name of the gesture.</param>
		/// <param name="gesture">The gesture to be saved.</param>
		/// <param name="errorMessage">Error message.</param>
		/// <returns></returns>
		public bool SaveNewGesture(string name, SingleHandGestureStatic gesture, out string errorMessage)
		{
			errorMessage = "";
			if (_sqliteProvider.GestureExists(name))
			{
				errorMessage = String.Format("A gesture named '{0}' already exists.", name);
				return false;
			}
			_sqliteProvider.SaveGesture(gesture);
			updateGestureLibrary();
			updateGestureLibraryMenu();
			return true;
		}
		#endregion

		#region Private Methods
		private void initMenuBar()
		{
			MenuBar = new ObservableCollection<CustomMenuItem>();

			#region OPTIONS
			//CustomMenuItem options = new CustomMenuItem("Options");

			//CustomMenuItem showAxes = new CustomMenuItem("Show Axes");
			//showAxes.IsCheckable = true;
			//showAxes.IsChecked = ShowAxes;
			//showAxes.Command = new CustomCommand(a => ShowAxes = !ShowAxes);
			//options.Items.Add(showAxes);

			////CustomMenuItem showOutputWindow = new CustomMenuItem("Show Output Window");
			////showOutputWindow.IsCheckable = true;
			////showOutputWindow.IsChecked = ShowOutputWindow;
			////showOutputWindow.Command = new CustomCommand(() => ShowOutputWindow = !ShowOutputWindow);
			////options.Items.Add(showOutputWindow);

			//CustomMenuItem showArms = new CustomMenuItem("Show Arms");
			//showArms.IsCheckable = true;
			//showArms.IsChecked = ShowAxes;
			//showArms.Command = new CustomCommand(a => ShowArms = !ShowArms);
			//options.Items.Add(showArms);
			
			//MenuBar.Add(options);
			#endregion

			#region RESET CAMERA
			CustomMenuItem resetCamera = new CustomMenuItem("Reset Camera");
			resetCamera.Command = new CustomCommand(a => _camera.Reset());
			MenuBar.Add(resetCamera);
			#endregion

			#region DEFAULT MODE
			// Might want to rename to "Live Mode"
			CustomMenuItem defaultMode = new CustomMenuItem("Default Mode");
			defaultMode.Command = new CustomCommand(a => Mode = LGR_Mode.Default);
			MenuBar.Add(defaultMode);
			#endregion

			#region OPTIONS
			CustomMenuItem options = new CustomMenuItem("Options");
			options.Command = new CustomCommand(a => DisplayOptionsDialog());
			MenuBar.Add(options);
			#endregion

		}

		private void updateGestureLibrary()
		{
			StaticGestures = _sqliteProvider.GetAllGestures();
		}
		#endregion

		#region PropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
		#endregion

	}

}
