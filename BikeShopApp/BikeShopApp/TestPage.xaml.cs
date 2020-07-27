using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BusinessLogic;

namespace BikeShopApp
{
    /// <summary>
    /// TestPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TestPage : Page
    {
        public TestPage()
        {
            InitializeComponent();

            InitListBox();
        }

        private void InitListBox()
        {
            Random rand = new Random();
            string[] names = { "Hugo", "Ted", "Ashley", "Jeff", "Dave", "Allen" };

            List<Car> lists = new List<Car>();
            for (int i = 0; i < 10; i++)
            {
                byte red = (byte)(i % 3 == 0 ? 255 : (i * 50) % 255);
                byte green = (byte)(i % 4 == 0 ? 255 : (i * 40) % 255);
                byte blue = (byte)(i % 5 == 0 ? 255 : (i * 70) % 255);
                int index = rand.Next(names.Length);
                
                lists.Add(new Car()
                {
                    Speed = i * 20,
                    Color = Color.FromRgb(red, green, blue),
                    Driver = new Human{
                        Name = names[index],
                        HasDrivingLicense = true
                    }
                });
            }
            //ListCar.DataContext = lists; // ListCar에서만 사용가능
            //ComboCar.DataContext = lists;
            this.DataContext = lists;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //Human h = new Human();
            //h.Name = "Nick";
            //h.HasDrivingLicense = true;

            //Car car2 = new Car();
            //car2.Speed = 50.4;
            //car2.Color = Colors.Red;
            //car2.Driver = h;

            //MessageBox.Show($"car1 : {car1.Speed}, {car1.Color}");
            //MessageBox.Show($"car2 : {car2.Speed}, {car2.Color}");

            Car car = new Car();
            car.Speed = 100;
            car.Color = Colors.Blue;
            car.Driver = new Human { Name = "Ted", HasDrivingLicense = true };

            //this.DataContext = car; // TestPage에서 전부 사용 가능
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("HEll World!");
        }
    }
}
