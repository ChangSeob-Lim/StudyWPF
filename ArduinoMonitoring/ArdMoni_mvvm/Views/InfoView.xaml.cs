using MahApps.Metro.Controls;

namespace ArdMoni_mvvm.Views
{
    /// <summary>
    /// InfoView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InfoView : MetroWindow
    {
        public InfoView()
        {
            InitializeComponent();

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            ShowTitleBar = false;
            ShowMaxRestoreButton = false;
            ShowMinButton = false;
            ShowCloseButton = false;
        }
    }
}
