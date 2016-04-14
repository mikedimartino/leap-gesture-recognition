using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace LeapGestureRecognition.ViewModel
{
	public class EditStaticGestureViewModel : INotifyPropertyChanged
	{
		private MainViewModel _mvm;
		SQLiteProvider _provider;
		bool _newGesture;
		SGRecorder _recorder;

		// NOTE: A new EditGestureViewModel is instantiated on every call to EditGesture()
		public EditStaticGestureViewModel(MainViewModel mvm, SGClassWrapper gesture = null, bool newGesture = false)
		{
			_mvm = mvm;
			_newGesture = newGesture;
			_provider = _mvm.SQLiteProvider;
			_recorder = new SGRecorder(this, _mvm);

			_recorder.RecordingSessionFinished += OnRecordingSessionFinished;

			var featureWeightsDict = new Dictionary<string, int>();

			if (newGesture)
			{
				Name = "New Static Gesture";
				Instances = new ObservableCollection<SGInstanceWrapper>();
				featureWeightsDict = SGClass.GetDefaultFeatureWeights();
			}
			else
			{
				Name = gesture.Name;
				Id = gesture.Id;
				Instances = _provider.GetStaticGestureInstances(gesture.Id);
				featureWeightsDict = gesture.Gesture.FeatureWeights;
			}

			setFeatureWeights(featureWeightsDict);

			Changeset = new EditStaticGestureChangeset();
		}

		#region Public Properties
		public int Id { get; set; }
		public string Name { get; set; }
		public ObservableCollection<SGInstanceWrapper> Instances { get; set; }
		public SGInstanceWrapper SelectedInstance { get; set; }
		public EditStaticGestureChangeset Changeset { get; set; }
		public ObservableCollection<FeatureWeight> FeatureWeights { get; set; }

		private bool _RecordingInProgress = false;
		public bool RecordingInProgress
		{
			get { return _RecordingInProgress; }
			set
			{
				_RecordingInProgress = value;
				OnPropertyChanged("RecordingInProgress");
			}
		}
		#endregion

		#region Public Methods
		public void SaveGesture()
		{
			Dictionary<string, int> featureWeightsDict = null;
			if (_newGesture)
			{
				Id = _provider.SaveNewStaticGestureClass(Name, null); // Need to get id
			}
			else
			{
				// Fill featureWeightsDict
				featureWeightsDict = new Dictionary<string, int>();
				foreach (var fw in FeatureWeights)
				{
					featureWeightsDict.Add(fw.Name, fw.Weight);
				}
			}

			var editedGesture = new SGClass(Instances, featureWeightsDict);
			var sampleInstance = (Instances.Any()) ? Instances.FirstOrDefault().Gesture : null;

			var gestureWrapper = new SGClassWrapper()
			{
				Id = this.Id,
				Name = this.Name,
				Gesture = editedGesture,
				SampleInstance = sampleInstance
			};

			_provider.SaveStaticGestureClass(gestureWrapper);

			if (Changeset.ChangesExist())
			{
				// Update the StaticGestureInstances table
				foreach (int instanceId in Changeset.DeletedGestureInstances)
				{
					_provider.DeleteStaticGestureInstance(instanceId);
				}
				foreach (var instance in Changeset.NewGestureInstances)
				{
					_provider.SaveNewStaticGestureInstance(Id, instance.Gesture);
				}
			}

			_mvm.UpdateStaticGestureLibrary();
		}

		public void AddInstance(SGInstanceWrapper instance)
		{
			Instances.Add(instance);
			Changeset.NewGestureInstances.Add(instance);
		}

		public void DeleteInstance(SGInstanceWrapper instance)
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

		public void ViewInstance(SGInstanceWrapper instance)
		{
			SelectedInstance = instance ?? Instances.FirstOrDefault();

			if (instance != null)
			{
				_mvm.ViewStaticGesture(instance.Gesture);
			}
			else
			{
				_mvm.ViewStaticGesture(null);
			}
		}

		public void RecordGestureInstances()
		{
			_recorder.RecordGestureInstancesWithCountdown(5, 500, 10);
			RecordingInProgress = true;
		}

		public void OnRecordingSessionFinished(object source, EventArgs e)
		{
			RecordingInProgress = false;
		}

		public void CancelEdit()
		{
			if (RecordingInProgress)
			{
				MainViewModel.WriteLineToOutputWindow("Recording session cancelled.");
				_recorder.Stop();
			} 
			_mvm.Mode = LGR_Mode.Recognize;
		}
		#endregion

		#region Private Methods
		private void setFeatureWeights(Dictionary<string, int> featureWeightsDict)
		{
			FeatureWeights = new ObservableCollection<FeatureWeight>();
			foreach (var nameWeight in featureWeightsDict)
			{
				FeatureWeights.Add(new FeatureWeight(nameWeight.Key, nameWeight.Value));
			}
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
