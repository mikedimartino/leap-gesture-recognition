﻿using LeapGestureRecognition.Util;
using LGR;
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
		DynamicGestureRecorder _recorder;

		public EditDynamicGestureViewModel(MainViewModel mvm, DynamicGestureClassWrapper gesture = null, bool newGesture = false)
		{
			_mvm = mvm;
			_newGesture = newGesture;
			_provider = _mvm.SQLiteProvider;

			if (newGesture)
			{
				Name = "New Dynamic Gesture";
				Instances = new ObservableCollection<DynamicGestureInstanceWrapper>();
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
		public ObservableCollection<DynamicGestureInstanceWrapper> Instances { get; set; }
		public EditDynamicGestureChangeset Changeset { get; set; }


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
		#endregion

		#region Public Methods
		public void SaveGesture()
		{
			if (_newGesture)
			{
				Id = _provider.SaveNewDynamicGestureClass(Name, null); // Need to get id
			}

			var editedGesture = new DynamicGestureClass(Instances);
			var sampleInstance = (Instances.Any()) ? Instances.FirstOrDefault().Instance : null;

			var gestureWrapper = new DynamicGestureClassWrapper()
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

			_mvm.UpdateStaticGestureLibrary();
		}

		public void DeleteInstance(DynamicGestureInstanceWrapper instance)
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

		public void ViewInstance(DynamicGestureInstanceWrapper instance)
		{
			_mvm.DisplayDynamicGesture(instance.Instance);
		}


		public void StartRecordingSession() 
		{
			RecordingInProgress = true;
		}

		public void EndRecordingSession() 
		{
			RecordingInProgress = false;
			foreach (var instance in _mvm.DynamicGestureRecorder.Instances)
			{
				var instanceWrapper = new DynamicGestureInstanceWrapper(instance);
				Instances.Add(instanceWrapper);
				Changeset.NewGestureInstances.Add(instanceWrapper);
			}
			_mvm.DynamicGestureRecorder.Instances.Clear();
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
