using LGR;
using LeapGestureRecognition.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LeapGestureRecognition.ViewModel
{
	public class EditStaticGestureViewModel : INotifyPropertyChanged
	{
		private MainViewModel _mvm;
		SQLiteProvider _provider;
		bool _newGesture;

		// NOTE: A new EditGestureViewModel is instantiated on every call to EditGesture()
		public EditStaticGestureViewModel(MainViewModel mvm, StaticGestureClassWrapper gesture = null, bool newGesture = false)
		{
			_mvm = mvm;
			_newGesture = newGesture;
			_provider = _mvm.SQLiteProvider;

			if (newGesture)
			{
				Name = "New Static Gesture";
				Instances = new ObservableCollection<StaticGestureInstanceWrapper>();
			}
			else
			{
				Name = gesture.Name;
				Id = gesture.Id;
				Instances = _provider.GetStaticGestureInstances(gesture.Id);
			}
			
			Changeset = new EditStaticGestureChangeset();
		}

		#region Public Properties
		public int Id { get; set; }
		public string Name { get; set; }
		public ObservableCollection<StaticGestureInstanceWrapper> Instances { get; set; }
		public EditStaticGestureChangeset Changeset { get; set; }
		public bool RecordingInProgress { get; set; } // Currently recording gesture instances?
		#endregion

		#region Public Methods
		public void SaveGesture()
		{
			if (_newGesture)
			{
				Id = _provider.SaveNewStaticGestureClass(Name, null); // Need to get id
			}

			var editedGesture = new StaticGestureClass(Instances);
			var sampleInstance = (Instances.Any()) ? Instances.FirstOrDefault().Gesture : null;

			var gestureWrapper = new StaticGestureClassWrapper()
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

		public void AddInstance(StaticGestureInstanceWrapper instance)
		{
			Instances.Add(instance);
			Changeset.NewGestureInstances.Add(instance);
		}

		public void DeleteInstance(StaticGestureInstanceWrapper instance)
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

		public void ViewInstance(StaticGestureInstanceWrapper instance)
		{
			_mvm.DisplayStaticGesture(instance.Gesture);
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
