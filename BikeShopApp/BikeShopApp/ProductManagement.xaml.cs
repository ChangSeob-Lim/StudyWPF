using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace BikeShopApp
{
    /// <summary>
    /// ProductManagement.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProductManagement : Page
    {
        ProductsFactory factory;

        public ProductManagement()
        {
            InitializeComponent();

            factory = new ProductsFactory();

            GrdProducts.ItemsSource = factory.FindProducts(TxtSearch.Text);
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            GrdProducts.ItemsSource = factory.FindProducts(TxtSearch.Text);
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                GrdProducts.ItemsSource = factory.FindProducts(TxtSearch.Text);
        }
    }
}
