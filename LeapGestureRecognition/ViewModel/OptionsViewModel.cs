using LGR;
using LeapGestureRecognition.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LeapGestureRecognition.ViewModel
{
	public class OptionsViewModel : INotifyPropertyChanged
	{
		private MainViewModel _mvm;

		public OptionsViewModel(MainViewModel mvm)
		{
			_mvm = mvm;
			Config = _mvm.Config;
			Changeset = new OptionsChangeset();
			Users = new ObservableCollection<User>(_mvm.Config.AllUsers.Select(user => new User(user))); // Need to make copy, not reference.
		}

		#region Public Properties
		public LGR_Configuration Config { get; set; }
		public OptionsChangeset Changeset { get; set; }

		private ObservableCollection<User> _Users;
		public ObservableCollection<User> Users // A copy (not reference) of _mvm's Config.AllUsers
		{
			get { return _Users; }
			set
			{
				_Users = value;
				OnPropertyChanged("Users");
			}
		} 
		#endregion

		#region Public Methods
		public void BoolOptionChanged(string optionName, bool newValue)
		{
			if (Changeset.BoolOptionsChangeset.ContainsKey(optionName))
			{
				Changeset.BoolOptionsChangeset[optionName] = newValue;
			}
			else
			{
				Changeset.BoolOptionsChangeset.Add(optionName, newValue);
			}
		}

		public void BoneColorChanged(string boneName, Color? newValue)
		{
			if (Changeset.BoneColorsChangeset.ContainsKey(boneName))
			{
				Changeset.BoneColorsChangeset[boneName] = newValue ?? Colors.White;
			}
			else
			{
				Changeset.BoneColorsChangeset.Add(boneName, newValue ?? Colors.White);
			}
		}

		public void ActiveUserChanged(User user)
		{
			Changeset.ActiveUser = user;
		}

		public void DeleteUser(User user)
		{
			// Might want to add some logic for when active user is deleted // Maybe disable delete?
			Users.Remove(user);
			Changeset.NewUsers.Remove(user);
			Changeset.ModifiedUsers.Remove(user);
			Changeset.DeletedUserIds.Add(user.Id);
		}

		public void UserEdited(User user)
		{
			// Might want to add some logic for when active user is deleted // Maybe disable delete?
			if (Changeset.ModifiedUsers.Contains(user)) return;
			Changeset.ModifiedUsers.Add(user);
		}

		public void RemeasureHand(User user)
		{
			MeasureHandDialog measureHandDialog = new MeasureHandDialog(_mvm);
			while (measureHandDialog.ShowDialog() == true)
			{
				HandMeasurements newMeasurements = _mvm.MeasureHand();
				if (newMeasurements == null)
				{
					string errorMessage = "Unable to measure hand. Please try again.";
					measureHandDialog = new MeasureHandDialog(_mvm, errorMessage);
					continue;
				}
				user.UpdateHandMeasurements(newMeasurements);
				UserEdited(user);
				break;
			}
		}

		public void CreateNewUser(string name = "New User")
		{
			User newUser = new User() { Name = name };
			Users.Add(newUser);
			Changeset.NewUsers.Add(newUser);
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
