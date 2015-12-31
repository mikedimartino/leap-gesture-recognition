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
using System.Threading;
using System.Windows.Threading;
using LGR_Controls;

namespace LeapGestureRecognition.ViewModel
{
	public class MainViewModel : INotifyPropertyChanged
	{
		private static OpenGL _gl;
		private static Controller _controller;
		private System.Windows.Controls.ScrollViewer _scrollViewer;
		private GestureLibrary _gestureLibraryControl;
		private EditGesture _editGestureControl;

		private static Camera _camera;

		private CustomLeapListener _listener;
		private SharpGLHelper _glHelper;
		private SQLiteProvider _sqliteProvider;
		LGR_Configuration _config;


		public MainViewModel(OpenGL gl, System.Windows.Controls.ScrollViewer scrollViewer, GestureLibrary gestureLibraryControl, EditGesture editGestureControl, Controller controller, CustomLeapListener listener)
		{
			_gl = gl;
			_controller = controller;
			_listener = listener;
			_controller.AddListener(_listener);
			_camera = new Camera(_gl);
			_scrollViewer = scrollViewer;
			_gestureLibraryControl = gestureLibraryControl;
			_gestureLibraryControl.SetMvm(this);
			_editGestureControl = editGestureControl;
			_editGestureControl.SetMvm(this);
			_sqliteProvider = new SQLiteProvider(Constants.SQLiteFileName);
			_config = new LGR_Configuration(_sqliteProvider);
			_glHelper = new SharpGLHelper(_gl, _config.BoneColors);

			UpdateGestureLibrary();
			initMenuBar();
		}

		

		#region Public Properties
		public SQLiteProvider SQLiteProvider { get { return _sqliteProvider; } }

		public static double OpenGLWindowWidth { get; set; }
		public static double OpenGLWindowHeight { get; set; }
		public static Frame CurrentFrame { get; set; }
		public ObservableCollection<CustomMenuItem> MenuBar { get; set; }

		private bool _CurrentlyEditingGesture = false;
		public bool CurrentlyEditingGesture 
		{ 
			get { return _CurrentlyEditingGesture; }
			set
			{
				_CurrentlyEditingGesture = value;
				OnPropertyChanged("CurrentlyEditingGesture");
			}
		}

		private LGR_StaticGesture _SelectedGesture = null;
		public LGR_StaticGesture SelectedGesture
		{
			get { return _SelectedGesture; }
			set
			{
				_SelectedGesture = value;
				OnPropertyChanged("SelectedGesture");
				CurrentlyEditingGesture = _SelectedGesture != null;
			}
		}


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
			set { _Mode = value; }
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

		private ObservableCollection<LGR_StaticGesture> _staticGestures = new ObservableCollection<LGR_StaticGesture>();
		public ObservableCollection<LGR_StaticGesture> StaticGestures
		{
			get { return _staticGestures; }
			set
			{
				_staticGestures = value;
				OnPropertyChanged("StaticGestures");
				UpdateGestureLibraryMenu(); // Should move this... will continuously clear GestureLibraryMenuItems unnecessarily 
			}
		}

		public void UpdateGestureLibraryMenu()
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


		#region RecordNewGestureInstance
		private int recordNewGestureInstanceTimerCount = -1;
		private int newGestureInstanceDelaySeconds = 5;
		public void RecordNewGestureInstance(EditGestureViewModel editGestureVM)
		{
			Mode = LGR_Mode.Default;
			recordNewGestureInstanceTimerCount = newGestureInstanceDelaySeconds;
			WriteLineToOutputWindow("Taking snapshot in " + recordNewGestureInstanceTimerCount);

			DispatcherTimer recordNewGestureInstanceTimer = new DispatcherTimer();
			recordNewGestureInstanceTimer.Tick += recordNewGestureInstanceTimer_Tick;
			recordNewGestureInstanceTimer.Tag = editGestureVM;
			recordNewGestureInstanceTimer.Interval = new TimeSpan(0, 0, 1);
			recordNewGestureInstanceTimer.Start();
		}

		private void recordNewGestureInstanceTimer_Tick(object sender, EventArgs e)
		{
			if (--recordNewGestureInstanceTimerCount > 0)
			{
				WriteLineToOutputWindow("Taking snapshot in " + recordNewGestureInstanceTimerCount);
			}
			else
			{
				var editGestureVM = (EditGestureViewModel)(sender as DispatcherTimer).Tag;
				var newInstance = new LGR_StaticGesture(_controller.Frame(), name: editGestureVM.Name);
				// Update instances in VM
				editGestureVM.AddInstance(newInstance);
				((DispatcherTimer)sender).Stop();
				WriteLineToOutputWindow("Snapshot recorded");
				DisplayGesture(newInstance);
			}
		}
		#endregion

		// Measures hand on screen
		public LGR_HandMeasurements MeasureHand()
		{
			Hand hand = _controller.Frame().Hands.FirstOrDefault();
			if (hand == null) return null;
			return new LGR_SingleHandStaticGesture(hand).GetMeasurements();
		}

		#region Measure Hand
		//private int measureHandTimerCount = -1;
		//private int measureHandDelaySeconds = 5;
		//public LGR_HandMeasurements MeasureHand()
		//{
		//	Mode = LGR_Mode.Default;
		//	measureHandTimerCount = measureHandDelaySeconds;
		//	WriteLineToOutputWindow("Measuring hand in " + measureHandTimerCount);

		//	DispatcherTimer measureHandTimer = new DispatcherTimer();
		//	measureHandTimer.Tick += measureHandTimer_Tick;
		//	measureHandTimer.Interval = new TimeSpan(0, 0, 1);
		//	measureHandTimer.Start();
		//}

		//private void measureHandTimer_Tick(object sender, EventArgs e)
		//{
		//	if (--measureHandTimerCount > 0)
		//	{
		//		WriteLineToOutputWindow("Measuring hand in " + measureHandTimerCount);
		//	}
		//	else
		//	{
		//		// Update instances in VM
		//		Hand hand = _controller.Frame().Hands.FirstOrDefault();
		//		if (hand == null) return null;
		//		return new LGR_SingleHandStaticGesture(hand).GetMeasurements();
		//		((DispatcherTimer)sender).Stop();
		//		WriteLineToOutputWindow("Hand measured.");
		//	}
		//}
		#endregion

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
					_glHelper.DrawStaticGesture(SelectedGesture, ShowArms);
					break;
				default:
					_glHelper.DrawFrame(CurrentFrame, ShowArms);
					break;
			}
		}

		public void DisplayGesture(LGR_StaticGesture gesture)
		{
			Mode = LGR_Mode.Playback;
			SelectedGesture = gesture;
		}

		public void DeleteGesture(LGR_StaticGesture gesture)
		{
			_sqliteProvider.DeleteGesture(gesture.Id);
			UpdateGestureLibrary();
			UpdateGestureLibraryMenu();
		}

		public void DeleteUser(LGR_User user)
		{
			_sqliteProvider.DeleteUser(user.Id);
		}

		public string GetGestureName(int id) // The id of the gesture class (id in Gestures table, not GestureInstances).
		{
			return _sqliteProvider.GetGestureName(id);
		}

		public ObservableCollection<LGR_StaticGesture> GetGestureInstances(int classId)
		{
			return _sqliteProvider.GetGestureInstances(classId);
		}

		#region Dialog Windows
		public void DisplaySaveGestureDialog(LGR_StaticGesture gesture)
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
					UpdateGestureLibrary();
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

		public void NewStaticGesture()
		{
			Mode = LGR_Mode.Default;
			EditGesture(new LGR_StaticGesture(), newGesture: true);
		}

		public void EditGesture(LGR_StaticGesture gesture, bool newGesture = false)
		{
			_editGestureControl.VM = new EditGestureViewModel(gesture, this);
			_editGestureControl.Visibility = Visibility.Visible;
			if(!newGesture) DisplayGesture(gesture);
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

		public bool SaveGesture(LGR_StaticGesture gesture)
		{
			_sqliteProvider.SaveGesture(gesture);
			UpdateGestureLibrary();
			UpdateGestureLibraryMenu();
			return true;
		}

		public bool SaveNewGesture(string name, LGR_StaticGesture gesture)
		{
			_sqliteProvider.SaveGesture(gesture);
			UpdateGestureLibrary();
			UpdateGestureLibraryMenu();
			return true;
		}

		public void RecordNewInstance(LGR_StaticGesture gesture)
		{
			// Record new instance

			// Update averaged gesture
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

			#region NEW STATIC GESTURE
			CustomMenuItem newStaticGesture = new CustomMenuItem("New Static Gesture");
			newStaticGesture.Command = new CustomCommand(a => NewStaticGesture());
			MenuBar.Add(newStaticGesture);
			#endregion
		}

		public void UpdateGestureLibrary()
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
