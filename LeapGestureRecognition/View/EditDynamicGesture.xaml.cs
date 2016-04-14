using LeapGestureRecognition.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	/// Interaction logic for EditDynamicGesture.xaml
	/// </summary>
	public partial class EditDynamicGesture : UserControl
	{
		private MainViewModel _mvm;
		private EditDynamicGestureViewModel _vm; // Might be able to not use this and just use mvm instead

		public EditDynamicGesture()
		{
			InitializeComponent();
		}

		public EditDynamicGesture(MainViewModel mvm)
		{
			InitializeComponent();
			_mvm = mvm;
		}

		public EditDynamicGestureViewModel VM
		{
			get { return _vm; }
			set
			{
				_vm = value;
				this.DataContext = _vm;
			}
		}

		public void SetMvm(MainViewModel mvm) // Make property instead?
		{
			_mvm = mvm;
		}

		private void Save_Button_Click(object sender, RoutedEventArgs e)
		{
			_vm.SaveGesture();
			_mvm.Mode = LGR_Mode.Recognize;
		}

		private void Cancel_Button_Click(object sender, RoutedEventArgs e)
		{
			_vm.CancelEdit();
		}

		private void StartRecordingSession_Button_Click(object sender, RoutedEventArgs e)
		{
			_vm.StartRecordingSession();
		}

		private void EndRecordingSession_Button_Click(object sender, RoutedEventArgs e)
		{
			_vm.EndRecordingSession();
		}

		private void DeleteInstance(object sender, RoutedEventArgs e)
		{
			var instance = (DGInstanceWrapper)(e.Source as FrameworkElement).Tag;
			_vm.DeleteInstance(instance);
		}

		private void ViewInstance(object sender, RoutedEventArgs e)
		{
			var instance = (DGInstanceWrapper)(e.Source as FrameworkElement).Tag;
			_vm.ViewInstance(instance);
		}

		private void InstanceMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount >= 2) // Double click
			{
				var instance = (DGInstanceWrapper)(sender as FrameworkElement).Tag;
				_vm.ViewInstance(instance);
			}
		}

		private void Instance_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var instance = ((sender as ListBox).SelectedItem as DGInstanceWrapper);
			if(instance != null) _vm.ViewInstance(instance);
		}

		#region PropertyChanged
		// This should probably be in ViewModel. Oh well..
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
		#endregion
	}
}
