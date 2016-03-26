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

		Brush _backgroundColor;

		public RecognitionMonitor()
		{
			InitializeComponent();
			_backgroundColor = this.Background;
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

		Brush activeTabBrush = new SolidColorBrush(Colors.LightBlue);

		private void StaticTabClicked(object sender, MouseButtonEventArgs e)
		{
			staticTab.Background = activeTabBrush;
			dynamicTab.Background = _backgroundColor;
			staticRecognitionMonitor.Visibility = Visibility.Visible;
			dynamicRecognitionMonitor.Visibility = Visibility.Collapsed;
			_vm.Mode = GestureType.Static;
		}

		private void DynamicTabClicked(object sender, MouseButtonEventArgs e)
		{
			dynamicTab.Background = activeTabBrush;
			staticTab.Background = _backgroundColor;
			dynamicRecognitionMonitor.Visibility = Visibility.Visible;
			staticRecognitionMonitor.Visibility = Visibility.Collapsed;
			_vm.Mode = GestureType.Dynamic;
		}
	}
}
