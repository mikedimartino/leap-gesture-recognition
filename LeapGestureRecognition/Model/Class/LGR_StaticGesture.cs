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
			Name = "New Static Gesture";
			Id = -1;
			ClassId = -1;
			Hands = new List<LGR_SingleHandStaticGesture>();
			InstanceName = "new instance";
		}

		public LGR_StaticGesture(Frame frame, int id = -1, string name = "New Static Gesture")
		{
			Name = name;
			Id = id;
			Hands = new List<LGR_SingleHandStaticGesture>();
			foreach (Hand hand in frame.Hands)
			{
				Hands.Add(new LGR_SingleHandStaticGesture(hand));
			}
			InstanceName = "new instance";
		}

		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public int Id { get; set; }
		[DataMember]
		public List<LGR_SingleHandStaticGesture> Hands { get; set; }

		public int ClassId { get; set; }

		public string InstanceName { get; set; }

		public LGR_StaticGesture DeepCopy()
		{
			string json = JsonConvert.SerializeObject(this);
			return JsonConvert.DeserializeObject<LGR_StaticGesture>(json);
		}

	}
}
