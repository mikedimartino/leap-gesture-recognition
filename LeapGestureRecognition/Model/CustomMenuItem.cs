using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace LeapGestureRecognition.Model
{
	public class CustomMenuItem : MenuItem
	{

		public CustomMenuItem(string header, Action clickAction = null)
		{
			Header = header;
			Command = new CustomCommand(clickAction);
		}
	}

	public class CustomCommand : ICommand
	{
		Action _command;

		public CustomCommand(Action command)
		{
			_command = command;
		}

		#region ICommand Members
		public bool CanExecute(object parameter)
		{
			return true;
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public void Execute(object parameter)
		{
			try
			{
				_command();
			}
			catch(Exception)
			{
				throw new Exception("Error executing command " + _command);
			}
		}
		#endregion
	}
}
