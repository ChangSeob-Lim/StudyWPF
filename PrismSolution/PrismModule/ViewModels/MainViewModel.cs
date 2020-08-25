using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PrismModule.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private string title = "Prism Unity Application";
        public string Title
        {
            get => title;
            set
            {
                SetProperty(ref title, value);
            }
        }

        public MainViewModel()
        {
            ExecuteCommand = new DelegateCommand(Execute, CanExecute);
            ObservesPropertyCommand = new DelegateCommand(Execute, CanExecute).ObservesProperty(() => IsEnabled);
            ObservesCanExecuteCommand = new DelegateCommand(Execute).ObservesCanExecute(() => IsEnabled);
            ExecuteGenericCommand = new DelegateCommand<string>(ExecuteGeneric).ObservesCanExecute(() => IsEnabled);
        }

        bool isEnabled;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                SetProperty(ref isEnabled, value);
                ExecuteCommand.RaiseCanExecuteChanged();
            }
        }

        string updateText;
        public string UpdateText
        {
            get { return updateText; }
            set { SetProperty(ref updateText, value); }
        }

        public DelegateCommand ExecuteCommand { get; private set; }
        public DelegateCommand ObservesPropertyCommand { get; private set; }
        public DelegateCommand ObservesCanExecuteCommand { get; private set; }
        public DelegateCommand<string> ExecuteGenericCommand { get; private set; }

        private void ExecuteGeneric(string param)
        {
            UpdateText = param;
        }

        private bool CanExecute()
        {
            return IsEnabled;
        }

        private void Execute()
        {
            UpdateText = $"Updated: {DateTime.Now}";
        }

    }
}
