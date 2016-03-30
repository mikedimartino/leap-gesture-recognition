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

		public GestureLibrary()
		{
			InitializeComponent();
		}

		public GestureLibrary(MainViewModel mvm)
		{
			InitializeComponent();
			_mvm = mvm;
		}

		public void SetMvm(MainViewModel mvm) // Make property instead?
		{
			_mvm = mvm;
		}

		private void ViewGesture(object sender, RoutedEventArgs e)
		{
			object gesture = (e.Source as FrameworkElement).Tag;
			if (gesture is StaticGestureClassWrapper)
			{
				_mvm.ViewStaticGesture(((StaticGestureClassWrapper)gesture).SampleInstance);
			}
			else if (gesture is DynamicGestureClassWrapper)
			{
				_mvm.ViewDynamicGesture(((DynamicGestureClassWrapper)gesture).SampleInstance);
			}
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
			object gesture = (e.Source as FrameworkElement).Tag;
			if (gesture is StaticGestureClassWrapper)
			{
				_mvm.SQLiteProvider.DeleteStaticGestureClass(((StaticGestureClassWrapper)gesture).Id);
				_mvm.UpdateStaticGestureLibrary();
			}
			else if (gesture is DynamicGestureClassWrapper)
			{
				_mvm.SQLiteProvider.DeleteDynamicGestureClass(((DynamicGestureClassWrapper)gesture).Id);
				_mvm.UpdateDynamicGestureLibrary();
			}
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
			if (e.ClickCount >= 2) // Double click    
			{
				var gesture = (DynamicGestureClassWrapper)(sender as FrameworkElement).Tag;
				_mvm.EditDynamicGesture(gesture);
			}
		}

		public void ShowStaticGestures()
		{
			staticGesturesList.Visibility = Visibility.Visible;
			dynamicGesturesList.Visibility = Visibility.Collapsed;
		}

		public void ShowDynamicGestures()
		{
			dynamicGesturesList.Visibility = Visibility.Visible;
			staticGesturesList.Visibility = Visibility.Collapsed;
		}

	}
}
