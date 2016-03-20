using LGR;
using LeapGestureRecognition.Util;
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
	/// Interaction logic for GestureLibrary.xaml
	/// </summary>
	public partial class GestureLibrary : UserControl
	{
		private MainViewModel _mvm;

		private Brush backgroundColor;

		public GestureLibrary()
		{
			InitializeComponent();
		}

		public GestureLibrary(MainViewModel mvm)
		{
			InitializeComponent();
			_mvm = mvm;
			Brush backGroundColor = this.Background;
		}

		public void SetMvm(MainViewModel mvm) // Make property instead?
		{
			_mvm = mvm;
		}

		private void ViewGesture(object sender, RoutedEventArgs e)
		{
			var gesture = (StaticGestureClassWrapper)(e.Source as FrameworkElement).Tag;
			_mvm.DisplayStaticGesture(gesture.SampleInstance);
		}

		private void EditGesture(object sender, RoutedEventArgs e)
		{
			object gesture = (e.Source as FrameworkElement).Tag;
			if (gesture is StaticGestureClassWrapper)
			{
				_mvm.EditStaticGesture((StaticGestureClassWrapper)gesture);
			}
			else if (gesture is DynamicGestureClassWrapper)
			{
				_mvm.EditDynamicGesture((DynamicGestureClassWrapper)gesture);
			}
		}

		private void DeleteGesture(object sender, RoutedEventArgs e)
		{
			var gesture = (StaticGestureClassWrapper)(e.Source as FrameworkElement).Tag;
			_mvm.SQLiteProvider.DeleteStaticGestureClass(gesture.Id);
			_mvm.UpdateStaticGestureLibrary();
		}

		private void StaticGestureMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount >= 2) // Double click
			{
				var gesture = (StaticGestureClassWrapper)(sender as FrameworkElement).Tag;
				_mvm.EditStaticGesture(gesture);
			}
		}

		private void DynamicGestureMouseDown(object sender, MouseButtonEventArgs e)
		{
			//if (e.ClickCount >= 2) // Double click    
			//{
			//	var gesture = (DynamicGestureClassWrapper)(sender as FrameworkElement).Tag;
			//	_mvm.EditStaticGesture(gesture);
			//}
		}



		Brush activeTabBrush = new SolidColorBrush(Colors.LightBlue);

		private void StaticTabClicked(object sender, MouseButtonEventArgs e)
		{
			staticTab.Background = activeTabBrush;
			dynamicTab.Background = backgroundColor;
			staticGesturesList.Visibility = Visibility.Visible;
			dynamicGesturesList.Visibility = Visibility.Collapsed;
		}

		private void DynamicTabClicked(object sender, MouseButtonEventArgs e)
		{
			dynamicTab.Background = activeTabBrush;
			staticTab.Background = backgroundColor;
			dynamicGesturesList.Visibility = Visibility.Visible;
			staticGesturesList.Visibility = Visibility.Collapsed;
		}
	}
}
