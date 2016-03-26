using LeapGestureRecognition.ViewModel;
using LGR;
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
		private RecognitionMonitorViewModel _vm;

		public RecognitionMonitor()
		{
			InitializeComponent();
		}


		public RecognitionMonitorViewModel VM
		{
			get { return _vm; }
			set
			{
				_vm = value;
				this.DataContext = _vm;
			}
		}

		public void SwitchToStaticMode()
		{
			staticRecognitionMonitor.Visibility = Visibility.Visible;
			dynamicRecognitionMonitor.Visibility = Visibility.Collapsed;
			_vm.Mode = GestureType.Static;
		}

		public void SwitchToDynamicMode()
		{
			dynamicRecognitionMonitor.Visibility = Visibility.Visible;
			staticRecognitionMonitor.Visibility = Visibility.Collapsed;
			_vm.Mode = GestureType.Dynamic;
		}
	}
}
