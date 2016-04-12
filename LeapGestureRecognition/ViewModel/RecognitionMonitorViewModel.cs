using Leap;
using LGR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LeapGestureRecognition.ViewModel
{
	public class RecognitionMonitorViewModel : INotifyPropertyChanged
	{
		private StatisticalClassifier _classifier;
		private ObservableCollection<GestureDistance> _rankedStaticGestures;
		private ObservableCollection<GestureDistance> _rankedDynamicGestures;
		private DGRecorder _dgRecorder;



		public RecognitionMonitorViewModel(StatisticalClassifier classifier)
		{
			_classifier = classifier;
			_dgRecorder = new DGRecorder(inRecordMode: false);
			CurrentState = _dgRecorder.State;
			RankedStaticGestures = new ObservableCollection<GestureDistance>();
			RankedDynamicGestures = new ObservableCollection<GestureDistance>();
			Mode = GestureType.Static; // Probably not necessary to initialize
			Active = true;
		}

		#region Public Properties
		public GestureType Mode { get; set; }

		public ObservableCollection<GestureDistance> RankedStaticGestures
		{
			get { return _rankedStaticGestures; }
			set
			{
				if (_rankedStaticGestures == value) return;
				_rankedStaticGestures = value;
				OnPropertyChanged("RankedStaticGestures");
			}
		}

		public ObservableCollection<GestureDistance> RankedDynamicGestures
		{
			get { return _rankedDynamicGestures; }
			set
			{
				if (_rankedDynamicGestures == value) return;
				_rankedDynamicGestures = value;
				OnPropertyChanged("RankedDynamicGestures");
			}
		}

		private DGRecorderState _CurrentState;
		public DGRecorderState CurrentState
		{
			get { return _CurrentState; }
			set
			{
				_CurrentState = value;
				OnPropertyChanged("CurrentState");
			}
		}

		public bool Active { get; set; }
		#endregion

		#region Public Methods
		public void OnFrameReceived(object source, FrameEventArgs args)
		{
			 if(Active) ProcessFrame(args.Frame);
		}

		public void ProcessFrame(Frame frame)
		{
			if (Mode == GestureType.Static)
			{
				var distances = _classifier.GetDistancesFromAllClasses(new SGInstance(frame));
				RankedStaticGestures = new ObservableCollection<GestureDistance>(distances.OrderBy(g => g.Value).Select(g => new GestureDistance(g.Key.Name, g.Value)));
			}
			else
			{
				_dgRecorder.ProcessFrame(frame);
				CurrentState = _dgRecorder.State;
				switch (CurrentState)
				{
					case DGRecorderState.RecordingJustFinished:
						if (_dgRecorder.MostRecentInstance.Samples.Count == 0) break;

						var distances = _classifier.GetDistancesFromAllClasses(_dgRecorder.MostRecentInstance);
						RankedDynamicGestures = new ObservableCollection<GestureDistance>(distances.OrderBy(g => g.Value).Select(g => new GestureDistance(g.Key.Name, g.Value)));
						break;
				}
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
