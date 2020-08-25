using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using PrismEvent.Views;
using System.Windows;

namespace PrismEvent
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            //base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<LeftModule.LeftModuleLoader>();
            moduleCatalog.AddModule<RightModule.RightModuleLoader>();
        }
    }
}
