using Leap;
using LeapGestureRecognition.Util;
using LeapGestureRecognition.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LGR
{
	public enum DGRecorderState
	{
		WaitingForHands,
		WaitingToStart,
		InStartPosition,
		RecordingGesture,
		InEndPosition,
	}


	public class DynamicGestureRecorder
	{
		#region Private Variables
		MainViewModel _mvm;
		Frame _lastFrame;
		int _frameRate;
		bool _inRecordMode; // Record Mode (save multiple instances) or Recognize Mode (just store most recent instance).
		float _stillSeconds = 1.0f; // Number of seconds hands must be still
		float _stillDistance = 1f;
		#endregion

		#region Constructor
		public DynamicGestureRecorder(MainViewModel mvm = null, bool inRecordMode = true) // TODO: Remove leapListener code
		{
			_mvm = mvm;
			_frameRate = Constants.FrameRate;
			_lastFrame = null;
			_inRecordMode = inRecordMode;
			State = DGRecorderState.WaitingForHands;
			DebugMessage = "";
			Instances = new List<DynamicGestureInstance>();
		}
		#endregion

		#region Public Properties
		public DGRecorderState State { get; set; }
		public string DebugMessage { get; set; }

		public List<DynamicGestureInstance> Instances { get; set; }
		public DynamicGestureInstance MostRecentInstance { get; set; }
		#endregion


		#region Public Methods
		StaticGestureInstance _startOfGesture;
		List<StaticGestureInstance> _gestureSamples;

		public void ProcessFrame(Frame frame)
		{
			if (_mvm != null)
			{
				MainViewModel.ClearOutputWindow();
				MainViewModel.WriteLineToOutputWindow("Dynamic Gesture Recorder Debug Info:");
				MainViewModel.WriteLineToOutputWindow(DebugMessage);
			}

			if (_lastFrame == null)
			{
				_lastFrame = frame;
				return;
			}

			bool handsStill = handsAreStill(frame);

			if (frame.Hands.Count == 0) State = DGRecorderState.WaitingForHands;
			
			switch(State) 
			{
				case DGRecorderState.WaitingForHands:
					if (frame.Hands.Count > 0)
					{
						_stillGesture = new StaticGestureInstance(frame);
						State = DGRecorderState.WaitingToStart;
					}
					break;
				case DGRecorderState.WaitingToStart:
					if (handsStill)
					{
						_startOfGesture = new StaticGestureInstance(frame);
						_gestureSamples = new List<StaticGestureInstance>();
						_gestureSamples.Add(_startOfGesture);

						State = DGRecorderState.InStartPosition;
					}
					break;
				case DGRecorderState.InStartPosition:
					if (!handsStill)
					{
						State = DGRecorderState.RecordingGesture;
					}
					break;
				case DGRecorderState.RecordingGesture:
					if (handsStill)
					{
						// Trim the extra samples in back (from holding hand still for X seconds)
						int stillFrames = (int) (_frameRate * _stillSeconds);
						_gestureSamples.RemoveRange(_gestureSamples.Count - stillFrames, stillFrames);

						MostRecentInstance = new DynamicGestureInstance(_gestureSamples);
						if (_inRecordMode)
						{
							Instances.Add(MostRecentInstance);
						}

						State = DGRecorderState.InEndPosition; // Put this first so "InEndPosition" is printed while we process the frames
					}
					else
					{
						_gestureSamples.Add(new StaticGestureInstance(frame));
					}
					break;
				case DGRecorderState.InEndPosition:
					if (!handsStill)
					{
						State = DGRecorderState.WaitingToStart;
					} 
					break;
			}

		}

		#endregion

		#region Private Methods
		StaticGestureInstance _stillGesture = null;
		int _stillFramesCount = 0;

		private bool handsAreStill(Frame frame)
		{
			if (_stillGesture == null) return false;

			var liveStaticGesture = new StaticGestureInstance(frame);

			DebugMessage = String.Format("State: {0}\nDistance to still gesture: {1}\n", State, liveStaticGesture.DistanceTo(_stillGesture));
			float leftHandVelocityMagnitude = -1;
			float rightHandVelocityMagnitude = -1;
			foreach(var hand in frame.Hands) 
			{
				if(hand.IsLeft) leftHandVelocityMagnitude = hand.PalmVelocity.Magnitude;
				else rightHandVelocityMagnitude = hand.PalmVelocity.Magnitude;
			}
			DebugMessage += String.Format("Left hand velocity magnitude: {0}\nRight hand velocity magnitude: {1}", leftHandVelocityMagnitude, rightHandVelocityMagnitude);

			if (palmsAreMoving(frame) || liveStaticGesture.DistanceTo(_stillGesture) > _stillDistance)
			{// TODO: Add DistanceTo() to Instances (not just Class)
				_stillGesture = liveStaticGesture;
				_stillFramesCount = 0;
				return false;
			}
			return ++_stillFramesCount / (float)_frameRate >= _stillSeconds;
		}

		private bool palmsAreMoving(Frame frame)
		{
			return frame.Hands.Any(h => h.PalmVelocity.Magnitude > 100.0f);
		}
		#endregion
	}
}
