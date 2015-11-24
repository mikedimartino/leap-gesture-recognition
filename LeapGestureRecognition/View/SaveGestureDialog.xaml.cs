using LeapGestureRecognition.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LeapGestureRecognition.View
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class SaveGestureDialog : Window
	{
		public SaveGestureDialog(string errorMessage = "")
		{
			InitializeComponent();
			errorMessageTB.Text = errorMessage;
		}

		public string EnteredName { get; set; }

		private void Save_Button_Click(object sender, RoutedEventArgs e)
		{
			EnteredName = gestureNameTB.Text;
			DialogResult = true;
			base.Close();
		}

	}
}
