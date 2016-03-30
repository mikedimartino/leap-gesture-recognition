using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LeapGestureRecognition.ViewModel
{
	public class OutputWindowViewModel
	{
		// Inspired by http://stackoverflow.com/questions/936304/binding-to-static-property
		public static readonly DependencyProperty TextProperty =
				DependencyProperty.Register("Text", typeof(string),
        typeof( OutputWindowViewModel ), new UIPropertyMetadata( "no version!" ) );

		public string Text
		{
			get;
			set;
			//get { return (string) TextProperty }
			//set;
		} 

    public static OutputWindowViewModel Instance { get; private set; }

		static OutputWindowViewModel()
		{
			Instance = new OutputWindowViewModel();
    }
	}
}
