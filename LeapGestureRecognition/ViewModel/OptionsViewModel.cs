using LeapGestureRecognition.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LeapGestureRecognition.ViewModel
{
	public class OptionsViewModel
	{
		public OptionsViewModel(LGR_Configuration config)
		{
			Config = config;
			Changeset = new OptionsDialogChangeset();
		}

		#region Public Properties
		public LGR_Configuration Config { get; set; }
		public OptionsDialogChangeset Changeset { get; set; }
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



	}
}
