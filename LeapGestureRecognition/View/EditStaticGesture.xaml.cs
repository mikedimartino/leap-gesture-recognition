using LeapGestureRecognition.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Globalization;

namespace LeapGestureRecognition.View
{
	public partial class EditStaticGesture : UserControl
	{
		private MainViewModel _mvm;
		private EditStaticGestureViewModel _vm; // Might be able to not use this and just use mvm instead

		public EditStaticGesture() 
		{
			InitializeComponent();
		}

		public EditStaticGesture(MainViewModel mvm)
		{
			InitializeComponent();
			_mvm = mvm;
		}

		public EditStaticGestureViewModel VM
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
			if (_vm.RecordingInProgress) return;
			_vm.SaveGesture();
			_mvm.Mode = LGR_Mode.Recognize;
		}

		private void Cancel_Button_Click(object sender, RoutedEventArgs e)
		{
			_vm.CancelEdit();
		}

		private void RecordNewInstances_Button_Click(object sender, RoutedEventArgs e)
		{
			_vm.RecordGestureInstances();
		}

		private void DeleteInstance(object sender, RoutedEventArgs e)
		{
			var gesture = (SGInstanceWrapper)(e.Source as FrameworkElement).Tag;
			_vm.DeleteInstance(gesture);
		}

		private void ViewInstance(object sender, RoutedEventArgs e)
		{
			var instance = (SGInstanceWrapper)(e.Source as FrameworkElement).Tag;
			_vm.ViewInstance(instance);
		}
		
		private void InstanceMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount >= 2) // Double click
			{
				var instance = (SGInstanceWrapper)(sender as FrameworkElement).Tag;
				_vm.ViewInstance(instance);
			}
		}

		private void Instance_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var instance = ((sender as ListBox).SelectedItem as SGInstanceWrapper);
			  _vm.ViewInstance(instance);
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


		private void InstanceKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Delete:
					if (_vm.SelectedInstance != null) _vm.DeleteInstance(_vm.SelectedInstance);
					break;
			}
		}
		
	}

}
