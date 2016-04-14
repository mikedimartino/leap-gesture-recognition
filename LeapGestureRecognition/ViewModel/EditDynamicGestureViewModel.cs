using Leap;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LeapGestureRecognition.ViewModel
{
	public class EditDynamicGestureViewModel : INotifyPropertyChanged
	{
		private MainViewModel _mvm;
		SQLiteProvider _provider;
		bool _newGesture;
		DGRecorder _recorder;

		public EditDynamicGestureViewModel(MainViewModel mvm, DGClassWrapper gesture = null, bool newGesture = false)
		{
			_mvm = mvm;
			_newGesture = newGesture;
			_provider = _mvm.SQLiteProvider;
			_recorder = new DGRecorder(_mvm);

			if (newGesture)
			{
				Name = "New Dynamic Gesture";
				Instances = new ObservableCollection<DGInstanceWrapper>();
			}
			else
			{
				Name = gesture.Name;
				Id = gesture.Id;
				Instances = _provider.GetDynamicGestureInstances(gesture.Id);
			}

			Changeset = new EditDynamicGestureChangeset();
		}

		#region Public Properties
		public int Id { get; set; }
		public string Name { get; set; }
		public ObservableCollection<DGInstanceWrapper> Instances { get; set; }
		public EditDynamicGestureChangeset Changeset { get; set; }
		public ObservableCollection<FeatureWeight> FeatureWeights { get; set; }

		private bool _RecordingInProgress;
		public bool RecordingInProgress
		{
			get { return _RecordingInProgress; }
			set
			{
				_RecordingInProgress = value;
				OnPropertyChanged("RecordingInProgress");
			}
		}

		private DGRecorderState _CurrentDGRecorderState;
		public DGRecorderState CurrentDGRecorderState
		{
			get { return _CurrentDGRecorderState; }
			set
			{
				_CurrentDGRecorderState = value;
				OnPropertyChanged("CurrentDGRecorderState");
			}
		}

		private int _NewInstancesCount = 0;
		public int NewInstancesCount
		{
			get { return _NewInstancesCount; }
			set
			{
				_NewInstancesCount = value;
				OnPropertyChanged("NewInstancesCount");
			}
		}
		#endregion

		#region Public Methods
		public void ProcessFrame(Frame frame)
		{
			if (RecordingInProgress)
			{
				_recorder.ProcessFrame(frame);
				CurrentDGRecorderState = _recorder.State;
				NewInstancesCount = _recorder.Instances.Count;
			}
		}

		public void SaveGesture()
		{
			if (_newGesture)
			{
				Id = _provider.SaveNewDynamicGestureClass(Name, null); // Need to get id
			}

			var editedGesture = new DGClass(Instances);
			var sampleInstance = (Instances.Any()) ? Instances.FirstOrDefault().Instance : null;

			var gestureWrapper = new DGClassWrapper()
			{
				Id = this.Id,
				Name = this.Name,
				Gesture = editedGesture,
				SampleInstance = sampleInstance
			};

			_provider.SaveDynamicGestureClass(gestureWrapper);

			if (Changeset.ChangesExist())
			{
				// Update the StaticGestureInstances table
				foreach (int instanceId in Changeset.DeletedGestureInstances)
				{
					_provider.DeleteDynamicGestureInstance(instanceId);
				}
				foreach (var instance in Changeset.NewGestureInstances)
				{
					_provider.SaveNewDynamicGestureInstance(Id, instance.Instance);
				}
			}

			_mvm.UpdateDynamicGestureLibrary();
		}

		public void CancelEdit()
		{
			_mvm.Mode = LGR_Mode.Recognize;
		}

		public void DeleteInstance(DGInstanceWrapper instance)
		{
			Instances.Remove(instance);
			if (Changeset.NewGestureInstances.Contains(instance))
			{
				Changeset.NewGestureInstances.Remove(instance);
			}
			else
			{
				Changeset.DeletedGestureInstances.Add(instance.Id);
			}
		}

		public void ViewInstance(DGInstanceWrapper instance)
		{
			_mvm.ViewDynamicGesture(instance.Instance);
		}


		public void StartRecordingSession() 
		{
			RecordingInProgress = true;
		}

		public void EndRecordingSession() 
		{
			RecordingInProgress = false;
			foreach (var instance in _recorder.Instances)
			{
				if (instance.Samples.Count == 0) return;
				var instanceWrapper = new DGInstanceWrapper(instance);
				Instances.Add(instanceWrapper);
				Changeset.NewGestureInstances.Add(instanceWrapper);
			}
			_recorder.Instances.Clear();
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
