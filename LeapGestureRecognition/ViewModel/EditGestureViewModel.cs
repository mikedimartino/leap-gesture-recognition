using LeapGestureRecognition.Model;
using LeapGestureRecognition.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LeapGestureRecognition.ViewModel
{
	public class EditGestureViewModel : INotifyPropertyChanged
	{
		private int _gestureId;
		private MainViewModel _mvm;

		public EditGestureViewModel(int gestureId, MainViewModel mvm)
		{
			_gestureId = gestureId;
			_mvm = mvm;
			Name = (gestureId == -1) ? "New Gesture" : _mvm.GetGestureName(gestureId);
			Instances = _mvm.GetGestureInstances(gestureId);
			Changeset = new EditGestureChangeset();
		}

		public EditGestureViewModel(LGR_StaticGesture gesture, MainViewModel mvm)
		{
			_mvm = mvm;
			Id = gesture.Id;
			Name = gesture.Name;
			Instances = _mvm.GetGestureInstances(gesture.Id);
			Changeset = new EditGestureChangeset();
		}

		#region Public Properties
		public int Id { get; set; }
		public string Name { get; set; }
		public ObservableCollection<LGR_StaticGesture> Instances { get; set; }
		public EditGestureChangeset Changeset { get; set; }
		#endregion

		#region Public Methods
		public void SaveGesture()
		{
			SQLiteProvider provider = _mvm.SQLiteProvider;
			// Save first instance as the gesture (later I will deal with mean/median)
			var editedGesture = Instances.FirstOrDefault();
			if(editedGesture != null) {
				editedGesture.Id = Id; // Not sure if needed
				editedGesture.ClassId = Id; // Not sure if needed
			}
			Id = provider.SaveGesture(editedGesture);
			foreach (var instanceId in Changeset.DeletedGestureInstances)
			{
				provider.DeleteGestureInstance(instanceId);
			}
			foreach (var instance in Changeset.NewGestureInstances)
			{
				instance.ClassId = Id;
				provider.SaveNewGestureInstance(instance);
			}
			_mvm.UpdateGestureLibrary();
		}

		public void AddInstance(LGR_StaticGesture instance)
		{
			Instances.Add(instance);
			Changeset.NewGestureInstances.Add(instance);
		}

		public void DeleteInstance(LGR_StaticGesture instance)
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

		public void ViewInstance(LGR_StaticGesture instance)
		{
			_mvm.DisplayGesture(instance);
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
