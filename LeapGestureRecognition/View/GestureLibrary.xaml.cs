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

namespace LeapGestureRecognition.View
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
			if (gesture is SGClassWrapper)
			{
				_mvm.ViewStaticGesture(((SGClassWrapper)gesture).SampleInstance);
			}
			else if (gesture is DGClassWrapper)
			{
				_mvm.ViewDynamicGesture(((DGClassWrapper)gesture).SampleInstance);
			}
		}

		private void EditGesture(object sender, RoutedEventArgs e)
		{
			object gesture = (e.Source as FrameworkElement).Tag;
			if (gesture is SGClassWrapper)
			{
				_mvm.EditStaticGesture((SGClassWrapper)gesture);
			}
			else if (gesture is DGClassWrapper)
			{
				_mvm.EditDynamicGesture((DGClassWrapper)gesture);
			}
		}

		private void DeleteGesture(object sender, RoutedEventArgs e)
		{
			object gesture = (e.Source as FrameworkElement).Tag;
			if (gesture is SGClassWrapper)
			{
				_mvm.SQLiteProvider.DeleteStaticGestureClass(((SGClassWrapper)gesture).Id);
				_mvm.UpdateStaticGestureLibrary();
			}
			else if (gesture is DGClassWrapper)
			{
				_mvm.SQLiteProvider.DeleteDynamicGestureClass(((DGClassWrapper)gesture).Id);
				_mvm.UpdateDynamicGestureLibrary();
			}
		}

		private void StaticGestureMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount >= 2) // Double click
			{
				var gesture = (SGClassWrapper)(sender as FrameworkElement).Tag;
				_mvm.EditStaticGesture(gesture);
			}
		}

		private void DynamicGestureMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount >= 2) // Double click    
			{
				var gesture = (DGClassWrapper)(sender as FrameworkElement).Tag;
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
