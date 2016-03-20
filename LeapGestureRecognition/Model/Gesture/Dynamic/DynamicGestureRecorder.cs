using Leap;
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
		private const int _maxStillFramesCount = 3; //10; // If hands are still for this many frames, change state.
		private Frame _lastFrame;
		private int _frameRate;
		private MainViewModel _mvm;
		#endregion

		#region Constructor
		public DynamicGestureRecorder(int frameRate)
		{
			_frameRate = frameRate;
			_lastFrame = null;
			State = DGRecorderState.WaitingForHands;
			DebugMessage = "";
			Instances = new List<DynamicGestureInstance>();
		}
		#endregion

		#region Public Properties
		public DGRecorderState State { get; set; }
		public string DebugMessage { get; set; }

		public List<DynamicGestureInstance> Instances { get; set; }
		#endregion

		#region Public Methods
		DynamicGestureInstanceSample _startOfGesture;
		List<DynamicGestureInstanceSample> _gestureSamples;
		public void ProcessFrame(Frame frame)
		{
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
						_startOfGesture = new DynamicGestureInstanceSample(frame);
						_gestureSamples = new List<DynamicGestureInstanceSample>();
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
						Instances.Add(new DynamicGestureInstance(_gestureSamples));
						State = DGRecorderState.InEndPosition;
					}
					else
					{
						_gestureSamples.Add(new DynamicGestureInstanceSample(frame, _startOfGesture));
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

		float _stillSeconds = 1.0f; // Number of seconds hands must be still
		float _stillDistance = 1f;
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

			if (palmsAreMoving(frame) || _stillGesture == null || liveStaticGesture.DistanceTo(_stillGesture) > _stillDistance)
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
