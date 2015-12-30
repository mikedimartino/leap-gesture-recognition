using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeapGestureRecognition.Model
{
	public interface IGesture
	{
		int Id { get; set; }
		string Name { get; set; }
		GestureType Type { get; set; }
	}
}
