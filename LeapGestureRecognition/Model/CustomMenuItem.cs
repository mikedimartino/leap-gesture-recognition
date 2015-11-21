using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace LeapGestureRecognition.Model
{
	public class CustomMenuItem : MenuItem
	{
		public CustomMenuItem(string header = "", Action<object> clickAction = null)
		{
			Header = header;
			Command = new CustomCommand(clickAction);
		}
	}
}
