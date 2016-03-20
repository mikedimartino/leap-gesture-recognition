using LeapGestureRecognition.ViewModel;
using LGR;
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

namespace LGR_Controls
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

		}

		private void Cancel_Button_Click(object sender, RoutedEventArgs e)
		{

		}

		private void StartRecordingSession_Button_Click(object sender, RoutedEventArgs e)
		{
			_vm.StartRecordingSession();
			//_vm.RecordingInProgress = true;
		}

		private void EndRecordingSession_Button_Click(object sender, RoutedEventArgs e)
		{
			_vm.EndRecordingSession();
			//_vm.RecordingInProgress = false;
		}

		private void DeleteInstance(object sender, RoutedEventArgs e)
		{
			var instance = (DynamicGestureInstanceWrapper)(e.Source as FrameworkElement).Tag;
			_vm.DeleteInstance(instance);
		}

		private void ViewInstance(object sender, RoutedEventArgs e)
		{
			var instance = (DynamicGestureInstanceWrapper)(e.Source as FrameworkElement).Tag;
			_vm.ViewInstance(instance);
		}

		private void InstanceMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount >= 2) // Double click
			{
				var instance = (DynamicGestureInstanceWrapper)(sender as FrameworkElement).Tag;
				_vm.ViewInstance(instance);
			}
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
