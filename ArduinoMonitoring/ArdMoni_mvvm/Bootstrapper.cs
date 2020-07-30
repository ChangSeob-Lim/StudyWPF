using ArdMoni_mvvm.ViewModels;
using Caliburn.Micro;
using System.Windows;

namespace ArdMoni_mvvm
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
        }
    }
}
