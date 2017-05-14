using System;
using System.Windows.Input;


namespace progress_bar1
{
    class RelayCommand : ICommand
    {
        private Action<object> _action;

        public RelayCommand(Action<object> action)
        {
            _action = action;   // binder objekt _action til funksjon som tar 'hvasomhelst' som argument
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (parameter != null)
            {
                _action(parameter);   // bruker default-parameter som følger med objekt
            }
            else
            {
                _action("Hello World");  // bruker en egendefinert parameter til metodekallet
            }
        }

        #endregion
    }
}
