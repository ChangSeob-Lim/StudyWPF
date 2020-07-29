using SecondCaliburnApp.ViewModels;
using System.Windows;

namespace SecondCaliburnApp.Views
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();
            this.DataContext = new ShellViewModel();
        }
    }
}
