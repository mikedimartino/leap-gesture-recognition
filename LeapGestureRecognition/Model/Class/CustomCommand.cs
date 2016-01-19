using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace LGR
{
	public class CustomCommand : ICommand
	{
		Action<object> _action;

		public CustomCommand(Action<object> action)
		{
			_action = action;
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
			_action(parameter);
		}
		#endregion
	}
}
