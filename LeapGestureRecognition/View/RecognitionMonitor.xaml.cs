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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LGR_Controls
{
	/// <summary>
	/// Interaction logic for RecognitionMonitor.xaml
	/// </summary>
	public partial class RecognitionMonitor : UserControl
	{
		private MainViewModel _mvm;

		public RecognitionMonitor()
		{
			InitializeComponent();
		}

		public void SetMvm(MainViewModel mvm) // Make property instead?
		{
			_mvm = mvm;
			DataContext = _mvm;
		}
	}
}
