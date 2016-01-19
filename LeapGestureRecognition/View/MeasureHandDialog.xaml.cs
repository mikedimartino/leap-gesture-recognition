using LGR;
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
	public partial class MeasureHandDialog : Window
	{
		private MainViewModel _mvm;

		public MeasureHandDialog(MainViewModel mvm, string errorMessage = null)
		{
			InitializeComponent();
			_mvm = mvm;
			errorMessageTB.Text = errorMessage;
		}

		public HandMeasurements HandMeasurements { get; set; }

		private void Ok_Button_Click(object sender, RoutedEventArgs e)
		{
			HandMeasurements = _mvm.MeasureHand();
			DialogResult = true;
			base.Close();
		}
	}
}
