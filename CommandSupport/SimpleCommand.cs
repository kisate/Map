using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace WpfMap.CommandSupport
{
    /// <summary>
    /// Simplistic implementation of MVVM framework or Prism class DelegateCommand
    /// </summary>
    class SimpleCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        /// <summary>
        /// used only if overwritten in descendants
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Instatiates a command that always can be exected
        /// </summary>
        /// <param name="execute">Command exection delegate</param>
        public SimpleCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Instatiates a command that has non-empty CanExecute logic
        /// </summary>
        /// <param name="execute">Command exection delegate</param>
        /// <param name="canExecute">Command can execute logic</param>
        public SimpleCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Calls delegate if non-empty. 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>true if command can be executed.</returns>
        public virtual bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        public virtual void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Hook to be called by external logic
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
