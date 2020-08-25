using Prism.Ioc;
using Prism.Regions;
using PrismPractice.Views;
using SubModule.Views;
using System.Windows;

namespace Regions.Views
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainView : Window
    {
        //public MainView(IRegionManager regionManager)
        //{
        //    InitializeComponent();
        //    regionManager.RegisterViewWithRegion("ContentRegion", typeof(SubView));
        //}

        IContainerExtension _container;
        IRegionManager _regionManager;
        IRegion _region;
        ViewA _viewA;
        ViewB _viewB;

        public MainView(IContainerExtension container, IRegionManager regionManager)
        {
            InitializeComponent();
            _container = container;
            _regionManager = regionManager;
            this.Loaded += MainView_Loaded;
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    var view = _container.Resolve<SubView>();
        //    IRegion region = _regionManager.Regions["ContentRegion"];
        //    region.Add(view);
        //}

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            _viewA = _container.Resolve<ViewA>();
            _viewB = _container.Resolve<ViewB>();
            _region = _regionManager.Regions["ContentRegion"];
            _region.Add(_viewA);
            _region.Add(_viewB);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _region.Activate(_viewA);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _region.Deactivate(_viewA);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _region.Activate(_viewB);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            _region.Deactivate(_viewB);
        }
    }
}
