using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Leap;

namespace LeapGestureRecognition.Util
{
	public class CustomLeapListener : Listener
	{
		public override void OnFrame(Controller controller)
		{
			var frame = controller.Frame();
			if (frame.Hands.Count > 0)
			{
				string test = "this is a test string";
			}
		}
	}
}