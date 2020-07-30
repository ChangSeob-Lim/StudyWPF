using InteractiveDataDisplay.WPF;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ArduinoMonitoring
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        SerialPort serial;
        private short xCount = 200;
        private short maxPhotoVal = 1023;
        List<SensorData> photoDatas = new List<SensorData>();

        string strConnString = "Server=localhost;Port=3306;" +
            "Database=iot_sensordata;Uid=root;Pwd=mysql_p@ssw0rd";

        public bool IsSimulation { get; set; }

        DispatcherTimer timer = new DispatcherTimer();
        Random rand = new Random();
        
        List<int> timeValue = new List<int>();
        List<int> photoValue = new List<int>();

        public static RoutedCommand MyCommand_AltF4 = new RoutedCommand();
        public static RoutedCommand MyCommand_AltS = new RoutedCommand();
        public static RoutedCommand MyCommand_AltT = new RoutedCommand();
        
        public MainWindow()
        {
            InitializeComponent();
            InitControls();
            MakeShortcut();
        }

        private void MakeShortcut()
        {
            MyCommand_AltF4.InputGestures.Add(new KeyGesture(Key.F4, ModifierKeys.Alt));
            MyCommand_AltS.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Alt));
            MyCommand_AltT.InputGestures.Add(new KeyGesture(Key.T, ModifierKeys.Alt));

            CommandBindings.Add(new CommandBinding(MyCommand_AltF4, MenuSubItemExit_Click));
            CommandBindings.Add(new CommandBinding(MyCommand_AltS, MenuSubItemStart_Click));
            CommandBindings.Add(new CommandBinding(MyCommand_AltT, MenuSubItemStop_Click));
        }

        private void InitControls()
        {
            foreach (var item in SerialPort.GetPortNames())
            {
                CboSerialPort.Items.Add(item);
            }
            CboSerialPort.Text = "Select Port";

            PgbPhotoRegistor.Minimum = 0;
            PgbPhotoRegistor.Maximum = maxPhotoVal;

            BtnConnect.IsEnabled = BtnDisconnect.IsEnabled = false;
        }

        private void CboSerialPort_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var portName = CboSerialPort.SelectedItem.ToString();
            serial = new SerialPort(portName);
            serial.DataReceived += Serial_DataReceived;

            BtnConnect.IsEnabled = true;
        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string sVal = serial.ReadLine();
            this.BeginInvoke(new Action(delegate { DisplayValue(sVal); }));
        }

        private void DisplayValue(string sVal)
        {
            try
            {
                ushort v = ushort.Parse(sVal);
                if (v < 0 || v > maxPhotoVal)
                    return;

                SensorData data = new SensorData(DateTime.Now, v);
                photoDatas.Add(data);
                InsertDataToDB(data);

                TxtSensorCount.Text = photoDatas.Count.ToString();
                PgbPhotoRegistor.Value = v;
                TxtPhotoRegistor.Text = v.ToString();

                string item = $"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}\t{v}";

                TxtLog.AppendText($"{item}\n");
                TxtLog.ScrollToEnd();

                timeValue.Add(photoDatas.Count);
                photoValue.Add(v);

                PhotoGraph.Plot(timeValue, photoValue);

                if (IsSimulation == false)
                    BtnPortValue.Content = $"{serial.PortName}\n{sVal}";
                else
                    BtnPortValue.Content = $"{sVal}";
            }
            catch (Exception ex)
            {
                TxtLog.AppendText($"Error : {ex.Message}\n");
                TxtLog.ScrollToEnd();
            }
        }

        private void InsertDataToDB(SensorData data)
        {
            string strQuery = "INSERT INTO sensordatatbl " +
                " (Date, Value) " +
                " VALUES " +
                " (@Date, @Value) ";

            using (MySqlConnection conn = new MySqlConnection(strConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(strQuery, conn);
                MySqlParameter paramDate = new MySqlParameter("@Date", MySqlDbType.DateTime)
                {
                    Value = data.Date
                };
                cmd.Parameters.Add(paramDate);
                MySqlParameter paramValue = new MySqlParameter("@Value", MySqlDbType.Int32)
                {
                    Value = data.Value
                };
                cmd.Parameters.Add(paramValue);
                cmd.ExecuteNonQuery();
            }
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (serial != null)
                serial.Open();
            TxtConnectionTime.Text = $"연결시간 : {DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}";
            BtnConnect.IsEnabled = false;
            BtnDisconnect.IsEnabled = true;
        }

        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            if (serial != null)
                serial.Close();
            BtnConnect.IsEnabled = true;
            BtnDisconnect.IsEnabled = false;
            BtnPortValue.Content = "PORT";
        }

        //private void BtnViewAll_Click(object sender, EventArgs e)
        //{
            
        //}

        //private void BtnZoom_Click(object sender, EventArgs e)
        //{
            
        //}

        private void MenuSubItemExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MenuSubItemStart_Click(object sender, EventArgs e)
        {
            IsSimulation = true;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            // serial통신 끊기
            if (serial != null)
                BtnDisconnect_Click(sender, e);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ushort value = (ushort)rand.Next(1, 1024);
            DisplayValue(value.ToString());
        }

        private void MenuSubItemStop_Click(object sender, EventArgs e)
        {
            timer.Stop();
            IsSimulation = false;
            BtnPortValue.Content = "PORT";
            TxtConnectionTime.Text = "";

            // serial 통신 재시작
            //if (serial != null)
            //    BtnConnect_Click(sender, e);
        }

        private void MenuSubItemInfo_Click(object sender, RoutedEventArgs e)
        {
            ThisProgramForm form = new ThisProgramForm();
            form.WindowStartupLocation = this.WindowStartupLocation;
            form.ShowDialog();
        }
    }
}
