using System;
using System.Windows.Input;

namespace LSLCCEditor.Utility
{
    public class RelayCommand : ICommand
    {
        private Predicate<object> _canExecute;
        private Action<object> _execute;



        public RelayCommand(Action<object> execute)
            : this(execute, DefaultCanExecute)
        {
        }



        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }

            _execute = execute;
            _canExecute = canExecute;
        }



        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                CanExecuteChangedInternal += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
                CanExecuteChangedInternal -= value;
            }
        }



        public bool CanExecute(object parameter)
        {
            return _canExecute != null && _canExecute(parameter);
        }



        public void Execute(object parameter)
        {
            _execute(parameter);
        }



        private event EventHandler CanExecuteChangedInternal;



        public void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChangedInternal;
            if (handler != null)
            {
                //DispatcherHelper.BeginInvokeOnUIThread(() => handler.Invoke(this, EventArgs.Empty));
                handler.Invoke(this, EventArgs.Empty);
            }
        }



        public void Destroy()
        {
            _canExecute = _ => false;
            _execute = _ => { };
        }



        private static bool DefaultCanExecute(object parameter)
        {
            return true;
        }
    }
}