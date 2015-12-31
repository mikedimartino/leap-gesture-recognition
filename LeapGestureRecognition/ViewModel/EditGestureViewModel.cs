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
		SQLiteProvider _provider;
		bool _newGesture;

		// NOTE: A new EditGestureViewModel is instantiated on every call to EditGesture()
		public EditGestureViewModel(MainViewModel mvm, BayesStaticGestureWrapper gesture = null, bool newGesture = false)
		{
			_mvm = mvm;
			_newGesture = newGesture;
			_provider = _mvm.SQLiteProvider;

			if (newGesture)
			{
				Name = "New Gesture";
				Instances = new ObservableCollection<StaticGestureInstanceWrapper>();
			}
			else
			{
				Name = gesture.Name;
				Instances = _provider.GetStaticGestureInstances(gesture.Id);
			}
			
			Changeset = new EditGestureChangeset();
		}

		#region Public Properties
		public int Id { get; set; }
		public string Name { get; set; }
		public ObservableCollection<StaticGestureInstanceWrapper> Instances { get; set; }
		public EditGestureChangeset Changeset { get; set; }
		#endregion

		#region Public Methods
		public void SaveGesture()
		{
			if (_newGesture)
			{
				Id = _provider.SaveNewBayesStaticGesture(Name, null); // Need to get id
			}

			var editedGesture = new BayesStaticGesture(Instances);
			var sampleInstance = (Instances.Any()) ? Instances.FirstOrDefault().Gesture : null;

			var gestureWrapper = new BayesStaticGestureWrapper()
			{
				Id = this.Id,
				Name = this.Name,
				Gesture = editedGesture,
				SampleInstance = sampleInstance
			};

			_provider.SaveBayesStaticGesture(gestureWrapper);

			// Update the StaticGestureInstances table
			foreach (int instanceId in Changeset.DeletedGestureInstances)
			{
				_provider.DeleteStaticGestureInstance(instanceId);
			}
			foreach (var instance in Changeset.NewGestureInstances)
			{
				_provider.SaveNewStaticGestureInstance(Id, instance.Gesture);
			}

			_mvm.UpdateGestureLibrary();
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
			_mvm.DisplayGesture(instance.Gesture);
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
