using Leap;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LeapGestureRecognition.Model
{
	[DataContract]
	public class LGR_StaticGesture 
	{
		public LGR_StaticGesture()
		{
			Hands = new List<LGR_SingleHandStaticGesture>();
		}

		public LGR_StaticGesture(Frame frame)
		{
			Hands = new List<LGR_SingleHandStaticGesture>();
			foreach (var hand in frame.Hands)
			{
				Hands.Add(new LGR_SingleHandStaticGesture(hand));
			}

		}

		[DataMember]
		public List<LGR_SingleHandStaticGesture> Hands { get; set; } // Might not need this eventually, but maybe for displaying live hands

		public LGR_StaticGesture DeepCopy()
		{
			string json = JsonConvert.SerializeObject(this);
			return JsonConvert.DeserializeObject<LGR_StaticGesture>(json);
		}

	}
}
