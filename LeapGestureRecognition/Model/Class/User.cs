using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LGR
{
	public class User : INotifyPropertyChanged
	{
		public User() { }

		public User(User user)
		{
			Name = user.Name;
			IsActive = user.IsActive;
			Id = user.Id;
			PinkyLength = user.PinkyLength;
			RingLength = user.RingLength;
			MiddleLength = user.MiddleLength;
			IndexLength = user.IndexLength;
			ThumbLength = user.ThumbLength;
		}


		private string _Name;
		public string Name
		{
			get { return _Name; }
			set
			{
				_Name = value;
				OnPropertyChanged("Name");
			}
		}

		public bool IsActive { get; set; }
		public int Id { get; set; }

		// Lengths are measured in millimeters from hand center to finger tip.
		public float PinkyLength { get; set; }
		public float RingLength { get; set; }
		public float MiddleLength { get; set; }
		public float IndexLength { get; set; }
		public float ThumbLength { get; set; }

		private bool _ShowEditInfo = false;
		public bool ShowEditInfo
		{
			get { return _ShowEditInfo; }
			set
			{
				_ShowEditInfo = value;
				OnPropertyChanged("ShowEditInfo");
			}
		}

		public bool HideEditInfo
		{
			get { return !ShowEditInfo; }
		}

		public void UpdateHandMeasurements(HandMeasurements newMeasurements)
		{
			PinkyLength = newMeasurements.PinkyLength; OnPropertyChanged("PinkyLength");
			RingLength = newMeasurements.RingLength; OnPropertyChanged("RingLength");
			MiddleLength = newMeasurements.MiddleLength; OnPropertyChanged("MiddleLength");
			IndexLength = newMeasurements.IndexLength; OnPropertyChanged("IndexLength");
			ThumbLength = newMeasurements.ThumbLength; OnPropertyChanged("ThumbLength");
		}


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
