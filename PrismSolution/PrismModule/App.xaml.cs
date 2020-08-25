using Prism;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using PrismModule.Modules;
using PrismModule.ViewModels;
using PrismModule.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Unity.Injection;

namespace PrismModule
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
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

        //protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        //{
        //    //moduleCatalog.AddModule<ModulesLoader>();
        //    var subModuleType = typeof(ModulesLoader);
        //    moduleCatalog.AddModule(new ModuleInfo()
        //    {
        //        ModuleName = subModuleType.Name,
        //        ModuleType = subModuleType.AssemblyQualifiedName,
        //        InitializationMode = InitializationMode.OnDemand
        //    });

        //}

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            //ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            //{
            //    var viewName = viewType.GetTypeInfo().FullName;
            //    var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            //    var viewModelName = $"{viewName.Replace("Views", "ViewModels")}Model, {viewAssemblyName}";

            //    return Type.GetType(viewModelName);
            //});

            ViewModelLocationProvider.Register<MainView, MainViewModel>();
        }
    }
}
