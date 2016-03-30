using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leap;

namespace LGR
{
	public class CustomLeapListener : Listener
	{
		public const int FRAMEBUFFER_MAX = 500;

		

		public CustomLeapListener() : base()
		{
			FrameBuffer = new List<Frame>();
			RecordFrames = false;
		}

		#region Public Properties
		public bool RecordFrames { get; set; }
		public List<Frame> FrameBuffer { get; set; }

		//private List<Frame> _FrameBuffer = new List<Frame>();
		//public List<Frame> FrameBuffer // locks may be unnecessary
		//{
		//	get
		//	{
		//		lock (this)
		//		{
		//			return _FrameBuffer;
		//		}
		//	}
		//	set 
		//	{
		//		lock (this)
		//		{
		//			_FrameBuffer = value;
		//		}
		//	}
		//}
		#endregion

		public override void OnFrame(Controller controller)
		{
			if (RecordFrames && FrameBuffer.Count < FRAMEBUFFER_MAX)
			{
				FrameBuffer.Add(controller.Frame());
			}
		}

	}
}