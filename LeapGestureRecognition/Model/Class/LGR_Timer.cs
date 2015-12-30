using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace LeapGestureRecognition.Model
{
	public class LGR_Timer
	{
		DispatcherTimer _timer;

		public LGR_Timer() 
		{
			_timer = new DispatcherTimer();
			//_timer.Tick += snapshotTimer_Tick;
			//_timer.Interval = new TimeSpan(0, 0, 1);
			//_timer.Start();
		}


	}
}
