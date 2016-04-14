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
		}

		#region Public Properties
		public LGR_Configuration Config { get; set; }
		public OptionsChangeset Changeset { get; set; }
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
