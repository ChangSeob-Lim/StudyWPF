using ArdMoni_mvvm.Helpers;
using ArdMoni_mvvm.Models;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

namespace ArdMoni_mvvm.ViewModels
{
    public class MainViewModel : Conductor<object>
    {
        #region 생성자 영역

        public MainViewModel()
        {
            InitControls();
        }

        #endregion

        #region 속성 영역

        SerialPort serial;

        List<SensorData> photoDatas = new List<SensorData>();

        string connectedTime;
        public string ConnectedTime
        {
            get => connectedTime;
            set
            {
                connectedTime = value;
                NotifyOfPropertyChange(() => ConnectedTime);
            }
        }

        string connectedCount;
        public string ConnectedCount
        {
            get => connectedCount;
            set
            {
                connectedCount = value;
                NotifyOfPropertyChange(() => ConnectedCount);
            }
        }

        ushort photoValue;
        public ushort PhotoValue
        {
            get => photoValue;
            set
            {
                photoValue = value;
                NotifyOfPropertyChange(() => PhotoValue);
            }
        }

        string photoRegisterValue;
        public string PhotoRegisterValue
        {
            get => photoRegisterValue;
            set
            {
                photoRegisterValue = value;
                NotifyOfPropertyChange(() => PhotoRegisterValue);
            }
        }

        string photoSubInfo;
        public string PhotoSubInfo
        {
            get => photoSubInfo;
            set
            {
                photoSubInfo = value;
                NotifyOfPropertyChange(() => PhotoSubInfo);
            }
        }

        public string dataLog;
        public string DataLog
        {
            get => dataLog;
            set
            {
                dataLog = value;
                NotifyOfPropertyChange(() => DataLog);
            }
        }

        public List<int> xValue = new List<int>();
        public List<int> yValue = new List<int>();

        private short maxPhotoVal = 1023;

        public bool IsSimulation { get; set; }

        Timer timer = new Timer();
        Random rand = new Random();

        public BindableCollection<string> AllSerialPort { get; set; }
        
        string selectedSerialPort;
        public string SelectedSerialPort
        {
            get
            {
                return selectedSerialPort;
            }
            set
            {
                selectedSerialPort = value;

                SerialSetting();

                NotifyOfPropertyChange(() => SelectedSerialPort);
                NotifyOfPropertyChange(() => CanConnetPort);
                NotifyOfPropertyChange(() => CanDisconnetPort);
            }
        }

        #endregion

        private void InitControls()
        {
            // 시리얼 포트
            AllSerialPort = new BindableCollection<string>();

            foreach (var item in SerialPort.GetPortNames())
            {
                AllSerialPort.Add(item);
            }
        }

        private void SerialSetting()
        {
            serial = new SerialPort(SelectedSerialPort);
            serial.DataReceived += Serial_DataReceived;
        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string sVal = serial.ReadLine();
            (GetView() as MetroWindow).BeginInvoke(new System.Action(delegate { DisplayValue(sVal); }));
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

                ConnectedCount = photoDatas.Count.ToString();
                PhotoValue = v;
                PhotoRegisterValue = data.Value.ToString();

                InsertDataToDB(data);

                string item = $"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}\t{v}";

                DataLog += ($"{item}\n");

                xValue.Add(data.Value);
                yValue.Add(xValue.Count);
                //PhotoGraph.Plot(timeValue, photoValue);

                if (IsSimulation == false)
                    PhotoSubInfo = $"{serial.PortName}\n{sVal}";
                else
                    PhotoSubInfo = $"{sVal}";
            }
            catch (Exception ex)
            {
                DataLog += ($"Error : {ex.Message}\n");
                //TxtLog.ScrollToEnd();
            }
        }

        private void InsertDataToDB(SensorData data)
        {
            string strQuery = "INSERT INTO sensordatatbl " +
                " (Date, Value) " +
                " VALUES " +
                " (@Date, @Value) ";

            using (MySqlConnection conn = new MySqlConnection(Commons.CONNSTR))
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

        public bool CanConnetPort
        {
            get => (serial != null);
        }

        public void ConnetPort()
        {
            timer.Stop();
            if (serial != null)
                serial.Open();

            ConnectedTime = $"연결시간 : {DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}";
        }

        public bool CanDisconnetPort
        {
            get => (serial != null);
        }

        public void DisconnetPort()
        {
            if (serial != null)
                serial.Close();

            PhotoSubInfo = "PORT";
            ConnectedTime = string.Empty;
            ConnectedCount = string.Empty;
            PhotoValue = 0;
            PhotoRegisterValue = string.Empty;

            //List<int> reset = new List<int>();
            //PhotoGraph.Plot(reset, reset);
        }

        public void ProgramExit()
        {
            (GetView() as MetroWindow).Close();
        }

        public void StartSimulation()
        {
            IsSimulation = true;
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();

            // serial통신 끊기
            if (serial != null)
                DisconnetPort();
        }

        public void Timer_Tick(object sender, EventArgs e)
        {
            ushort value = (ushort)rand.Next(1, 1024);
            DisplayValue(value.ToString());
        }

        public void StopSimulation()
        {
            timer.Stop();
            IsSimulation = false;
            PhotoSubInfo = "PORT";
            ConnectedTime = string.Empty;
            ConnectedCount = string.Empty;
            PhotoValue = 0;
            PhotoRegisterValue = string.Empty;
        }

        // 정보창으로 이동
        public void InfoOpen()
        {
            IWindowManager btninfo = new WindowManager();
            btninfo.ShowDialog(new InfoViewModel(), null, null);
        }
    }
}
