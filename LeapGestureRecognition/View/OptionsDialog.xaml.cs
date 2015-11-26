using LeapGestureRecognition.Model;
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
using System.Windows.Shapes;

namespace LeapGestureRecognition.View
{
	/// <summary>
	/// Interaction logic for OptionsDialog.xaml
	/// </summary>
	public partial class OptionsDialog : Window
	{
		private OptionsViewModel _vm;
		private MainViewModel _mvm;

		public OptionsDialog(MainViewModel mvm)
		{
			InitializeComponent();
			_vm = new OptionsViewModel(mvm);
			DataContext = _vm;
			ActiveTab = OptionsTab.General;
			generalListBoxItem.IsSelected = true;
			userRows.ItemsSource = _vm.Users;
		}

		#region Public Properties
		public enum OptionsTab 
		{ 
			General, 
			BoneColors,
			Users
		}

		private OptionsTab _ActiveTab;
		public OptionsTab ActiveTab
		{
			get { return _ActiveTab; }
			set
			{
				FrameworkElement oldTab = getFrameworkElementFromTab(_ActiveTab);
				oldTab.Visibility = Visibility.Hidden;
				_ActiveTab = value;
				FrameworkElement newTab = getFrameworkElementFromTab(_ActiveTab);
				newTab.Visibility = Visibility.Visible;
			}
		}

		public OptionsDialogChangeset Changeset
		{
			get { return _vm.Changeset; }
		}
		#endregion

		#region Helper Methods
		private FrameworkElement getFrameworkElementFromTab(OptionsTab tab)
		{ 
			return (FrameworkElement) OptionsTabsListBox.FindName(tab.ToString() + "Options"); 
		}
		#endregion


		#region Event Handling Methods
		//private void BoolOptionToggled(object sender, Routed)
		private void BoolOption_CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			string optionName = (string)(e.Source as FrameworkElement).Tag;
			_vm.BoolOptionChanged(optionName, true);
		}

		private void BoolOption_CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			string optionName = (string)(e.Source as FrameworkElement).Tag;
			_vm.BoolOptionChanged(optionName, false);
		}

		private void SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
		{
			string boneName = (string)(e.Source as FrameworkElement).Tag;
			_vm.BoneColorChanged(boneName, e.NewValue);
		}


		private void Save_Button_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			base.Close();
		}

		private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
		{
			// I call it a tab, but it's not really a tab appearance wise. It functions the same way though.
			string tabName = (string)(e.Source as FrameworkElement).Tag;
			OptionsTab selectedTab = (OptionsTab) Enum.Parse(typeof(OptionsTab), tabName);
			ActiveTab = selectedTab;
		}

		private void User_RadioButton_Click(object sender, RoutedEventArgs e)
		{
			LGR_User activeUser = (LGR_User)(e.Source as FrameworkElement).Tag;
			_vm.ActiveUserChanged(activeUser);
		}

		private void Edit_Button_Click(object sender, RoutedEventArgs e)
		{
			Button editButton = e.Source as Button;
			LGR_User user = (LGR_User) editButton.Tag;
			user.ShowEditInfo = !user.ShowEditInfo;
			editButton.Content = user.ShowEditInfo ? "Done" : "Edit";
		}

		private void Delete_Button_Click(object sender, RoutedEventArgs e)
		{
			LGR_User user = (LGR_User)(e.Source as FrameworkElement).Tag;
			_vm.DeleteUser(user);
		}

		private void UserName_TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			LGR_User user = (LGR_User)(e.Source as FrameworkElement).Tag;
			_vm.UserEdited(user);
		}

		private void UserName_TextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			LGR_User user = (LGR_User)(e.Source as FrameworkElement).Tag;
			_vm.UserEdited(user);
		}

		private void Remeasure_Hands_Button_Click(object sender, RoutedEventArgs e)
		{
			LGR_User user = (LGR_User)(e.Source as FrameworkElement).Tag;
			_vm.RemeasureHand(user);
		}

		private void NewUser_Button_Click(object sender, RoutedEventArgs e)
		{
			_vm.CreateNewUser();
		}
		#endregion

	}
}
