using LeapGestureRecognition.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace LGR
{
	public class StaticGestureRecorder
	{
		private MainViewModel _mvm;
		private EditStaticGestureViewModel _editStaticGestureVM;
		private DispatcherTimer _startTimer;
		private DispatcherTimer _recordSnapshotTimer;

		private int _startTimerCountdown = 0;
		private int _remainingSnapshots = 0;

		public event EventHandler RecordingSessionFinished;


		public StaticGestureRecorder(EditStaticGestureViewModel editStaticGestureVM, MainViewModel mvm)
		{
			_editStaticGestureVM = editStaticGestureVM;
			_mvm = mvm;
			_startTimer = new DispatcherTimer();
			_recordSnapshotTimer = new DispatcherTimer();

			RecordingInProgress = false;
		}

		#region Public Properties
		public bool RecordingInProgress { get; set; }
		#endregion

		#region Public Methods
		public void RecordGestureInstancesWithCountdown(int countdownSeconds, int betweenDelayMilliseconds, int numInstances)
		{
			_startTimerCountdown = countdownSeconds;
			_startTimer.Tick += startTimerCountdown_Tick;
			_startTimer.Interval = new TimeSpan(0, 0, 1);
			_startTimer.Start();

			RecordingInProgress = true;
		}

		public void RecordGestureInstances(int numInstances)
		{
			_remainingSnapshots = numInstances;
			_recordSnapshotTimer.Tick += recordSnapshotTimer_Tick;
			_recordSnapshotTimer.Interval = new TimeSpan(0, 0, 1);
			_recordSnapshotTimer.Start();

			RecordingInProgress = true;
		}

		public void Stop()
		{
			_startTimer.Stop();
			_recordSnapshotTimer.Stop();
			RecordingInProgress = false;
		}
		#endregion


		#region Private Methods
		private void startTimerCountdown_Tick(object sender, EventArgs e)
		{
			if (--_startTimerCountdown > 0)
			{
				MainViewModel.WriteLineToOutputWindow("Recording gesture in " + _startTimerCountdown);
			}
			else
			{
				RecordGestureInstances(10);
				_startTimer.Stop();
			}
		}

		private void recordSnapshotTimer_Tick(object sender, EventArgs e)
		{
			if (_remainingSnapshots-- > 0)
			{
				_editStaticGestureVM.AddInstance(new StaticGestureInstanceWrapper(_mvm.CurrentFrame));
			}
			else
			{
				_recordSnapshotTimer.Stop();
				MainViewModel.WriteLineToOutputWindow("Finished recording gesture instances.");
				RecordingInProgress = false;
				OnRecordingSessionFinished();
			}
		}
		#endregion

		protected virtual void OnRecordingSessionFinished()
		{
			if (RecordingSessionFinished != null) RecordingSessionFinished(this, EventArgs.Empty);
		}

	}
}
