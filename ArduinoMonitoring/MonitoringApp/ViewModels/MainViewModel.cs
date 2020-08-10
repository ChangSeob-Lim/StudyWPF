using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using LiveCharts;
using MahApps.Metro.Controls;
using MonitoringApp.Helpers;
using MonitoringApp.Models;
using System.Windows;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;

namespace MonitoringApp.ViewModels
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

        #region 기본 공통 속성

        bool DoNotTwice = false;

        // 단축키 -> DoNotTwice 릴레이 커멘드를 걸면 직접 눌렀을때 왜 두번 실행되는지 모름...
        RelayCommand startSimulationCommand;
        public ICommand StartSimulationCommand
        {
            get
            {
                if(startSimulationCommand == null)
                {
                    startSimulationCommand = new RelayCommand(StartSimulation);
                    
                }

                return startSimulationCommand;
            }
        }

        RelayCommand stopSimulationCommand;
        public ICommand StopSimulationCommand
        {
            get
            {
                if (stopSimulationCommand == null)
                {
                    stopSimulationCommand = new RelayCommand(StopSimulation);
                }

                return stopSimulationCommand;
            }
        }

        RelayCommand programExitCommand;
        public ICommand ProgramExitCommand
        {
            get
            {
                if(programExitCommand == null)
                {
                    programExitCommand = new RelayCommand(ProgramExit);
                }

                return programExitCommand;
            }
        }

        RelayCommand openAllDataCommand;
        public ICommand OpenAllDataCommand
        {
            get
            {
                if (openAllDataCommand == null)
                {
                    openAllDataCommand = new RelayCommand(OpenAllData);
                }

                return openAllDataCommand;
            }
        }

        //RelayCommand saveMenuCommand;
        //public ICommand SaveMenuCommand
        //{
        //    get
        //    {
        //        return saveMenuCommand;
        //    }
        //}

        List<SensorData> SensorDataList = new List<SensorData>();
        List<SensorData> AllSensorDataList;

        Timer timer = new Timer(); // 타이머
        Random rand = new Random(); // 랜덤 변수

        // 모드 선택 
        enum Mode
        {
            Photo, // 조도
            TempHumi, // 온습도
        }

        // 모드 (기본값 -> 조도센서)
        Mode mode = Mode.Photo;

        string selectedMode = "Photo";
        public string SelectedMode
        {
            get => selectedMode;
            set
            {
                selectedMode = value;
                NotifyOfPropertyChange(() => SelectedMode);
            }
        }

        // 시뮬레이션
        bool isSimulation;
        public bool IsSimulation
        {
            get => isSimulation;
            set
            {
                isSimulation = value;
                NotifyOfPropertyChange(() => IsSimulation);
                NotifyOfPropertyChange(() => CanChangeMode);
            }
        }

        // 연결된 시리얼 포트
        SerialPort serial;

        // 모든 포트
        public BindableCollection<string> AllSerialPort { get; set; }

        // 포트 선택
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

        // 연결 시간
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

        // 연결 횟수
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

        // 로그 기록
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

        // 차트 x축 최소값
        int xMinValue = 0;
        public int XMinValue
        {
            get => xMinValue;
            set
            {
                xMinValue = value;
                NotifyOfPropertyChange(() => XMinValue);
            }
        }

        // 차트 x축 최대값
        //int xMaxValue = 5;
        //public int XMaxValue
        //{
        //    get => xMaxValue;
        //    set
        //    {
        //        xMaxValue = value;
        //        NotifyOfPropertyChange(() => XMaxValue);
        //    }
        //}

        // 차트 y축 최소값
        int yMinValue = -5;
        public int YMinValue
        {
            get => yMinValue;
            set
            {
                yMinValue = value;
                NotifyOfPropertyChange(() => YMinValue);
            }
        }

        // 차트 y축 최대값
        int yMaxValue = 1024;
        public int YMaxValue
        {
            get => yMaxValue;
            set
            {
                yMaxValue = value;
                NotifyOfPropertyChange(() => YMaxValue);
            }
        }

        // Zoom 모드 확인
        bool isZoom = false;
        public bool IsZoom
        {
            get => isZoom;
            set
            {
                isZoom = value;
            }
        }

        public MenuItem SaveMenu;

        #endregion 기본 공통 속성

        #region 조도센서 속성

        // 조도센서 최대값
        private short maxPhotoVal = 1023;

        // 조도센서 프로그레스바 값
        ushort photoPrgValue;
        public ushort PhotoPrgValue
        {
            get => photoPrgValue;
            set
            {
                photoPrgValue = value;
                NotifyOfPropertyChange(() => PhotoPrgValue);
            }
        }

        // 조도센서 프로그레스바 Visible
        Visibility photoPrgVisible;
        public Visibility PhotoPrgVisible
        {
            get => photoPrgVisible;
            set
            {
                photoPrgVisible = value;
                NotifyOfPropertyChange(() => PhotoPrgVisible);
            }
        }

        // 조도센서 값
        string photoStr;
        public string PhotoStr
        {
            get => photoStr;
            set
            {
                photoStr = value;
                NotifyOfPropertyChange(() => PhotoStr);
            }
        }

        // 조도센서 값 Visible
        Visibility photoStrVisible;
        public Visibility PhotoStrVisible
        {
            get => photoStrVisible;
            set
            {
                photoStrVisible = value;
                NotifyOfPropertyChange(() => PhotoStrVisible);
            }
        }

        // 포토 차트 값
        ChartValues<int> photoValues;
        public ChartValues<int> PhotoValues
        {
            get => photoValues;
            set
            {
                photoValues = value;
                NotifyOfPropertyChange(() => PhotoValues);
            }
        }

        // 포토 차트 Visible
        Visibility photoChartVisible;
        public Visibility PhotoChartVisible
        {
            get => photoChartVisible;
            set
            {
                photoChartVisible = value;
                NotifyOfPropertyChange(() => PhotoChartVisible);
            }
        }

        #endregion 조도센서 속성

        #region 온습도센서 속성

        // 온도센서 최대값
        private short maxTempVal = 50;

        // 온도센서 프로그레스바 값
        double tempPrgValue;
        public double TempPrgValue
        {
            get => tempPrgValue;
            set
            {
                tempPrgValue = value;
                NotifyOfPropertyChange(() => TempPrgValue);
            }
        }

        // 온도센서 프로그레스바 Visible
        Visibility tempPrgVisible;
        public Visibility TempPrgVisible
        {
            get => tempPrgVisible;
            set
            {
                tempPrgVisible = value;
                NotifyOfPropertyChange(() => TempPrgVisible);
            }
        }

        // 온도센서 값
        string tempStr;
        public string TempStr
        {
            get => tempStr;
            set
            {
                tempStr = value;
                NotifyOfPropertyChange(() => TempStr);
            }
        }

        // 온도센서 값 Visible
        Visibility tempStrVisible;
        public Visibility TempStrVisible
        {
            get => tempStrVisible;
            set
            {
                tempStrVisible = value;
                NotifyOfPropertyChange(() => TempStrVisible);
            }
        }

        // 습도센서 최대값
        private short maxHumiVal = 100;

        // 습도센서 프로그레스바 값
        double humiPrgValue;
        public double HumiPrgValue
        {
            get => humiPrgValue;
            set
            {
                humiPrgValue = value;
                NotifyOfPropertyChange(() => HumiPrgValue);
            }
        }

        // 습도센서 프로그레스바 Visible
        Visibility humiPrgVisible;
        public Visibility HumiPrgVisible
        {
            get => humiPrgVisible;
            set
            {
                humiPrgVisible = value;
                NotifyOfPropertyChange(() => HumiPrgVisible);
            }
        }

        // 습도센서 값
        string humiStr;
        public string HumiStr
        {
            get => humiStr;
            set
            {
                humiStr = value;
                NotifyOfPropertyChange(() => HumiStr);
            }
        }

        // 습도센서 값 Visible
        Visibility humiStrVisible;
        public Visibility HumiStrVisible
        {
            get => humiStrVisible;
            set
            {
                humiStrVisible = value;
                NotifyOfPropertyChange(() => HumiStrVisible);
            }
        }

        // 온도 차트 값
        ChartValues<double> tempValues;
        public ChartValues<double> TempValues
        {
            get => tempValues;
            set
            {
                tempValues = value;
                NotifyOfPropertyChange(() => TempValues);
            }
        }

        // 온도 차트 Visible
        Visibility tempChartVisible;
        public Visibility TempChartVisible
        {
            get => tempChartVisible;
            set
            {
                tempChartVisible = value;
                NotifyOfPropertyChange(() => TempChartVisible);
            }
        }

        // 습도 차트 값
        ChartValues<double> humiValues;
        public ChartValues<double> HumiValues
        {
            get => humiValues;
            set
            {
                humiValues = value;
                NotifyOfPropertyChange(() => HumiValues);
            }
        }

        // 습도 차트 Visible
        Visibility humiChartVisible;
        public Visibility HumiChartVisible
        {
            get => humiChartVisible;
            set
            {
                humiChartVisible = value;
                NotifyOfPropertyChange(() => HumiChartVisible);
            }
        }

        #endregion 온습도센서 속성

        #region 버튼 속성

        // Connect 버튼
        bool isBtnConnected = false;
        public bool IsBtnConnected
        {
            get => isBtnConnected;
            set
            {
                isBtnConnected = value;
                NotifyOfPropertyChange(() => IsBtnConnected);
                NotifyOfPropertyChange(() => CanConnetPort);
                NotifyOfPropertyChange(() => CanChangeMode);
            }
        }

        // Disconnect 버튼
        bool isBtnDisConnected = false;
        public bool IsBtnDisConnected
        {
            get => isBtnDisConnected;
            set
            {
                isBtnDisConnected = value;
                NotifyOfPropertyChange(() => IsBtnDisConnected);
                NotifyOfPropertyChange(() => CanDisconnetPort);
                NotifyOfPropertyChange(() => CanChangeMode);
            }
        }

        //LiveCharts.ZoomingOptions enableZoom;
        //public LiveCharts.ZoomingOptions EnableZoom
        //{
        //    get => enableZoom;
        //    set
        //    {
        //        enableZoom = value;
        //        NotifyOfPropertyChange(() => EnableZoom);
        //    }
        //}

        #endregion 버튼 속성

        #endregion 속성값

        private void InitControls()
        {
            // 시리얼 포트
            AllSerialPort = new BindableCollection<string>();

            AllSerialPort.Add("선택");
            foreach (var item in SerialPort.GetPortNames())
            {
                AllSerialPort.Add(item);
            }
            SelectedSerialPort = "선택";

            // 프로그래스 and 값 Visible
            PhotoPrgVisible = Visibility.Visible;
            TempPrgVisible = Visibility.Hidden;
            HumiPrgVisible = Visibility.Hidden;

            PhotoStrVisible = Visibility.Visible;
            TempStrVisible = Visibility.Hidden;
            HumiStrVisible = Visibility.Hidden;

            // 차트 값 초기화
            PhotoValues = new ChartValues<int>();
            TempValues = new ChartValues<double>();
            HumiValues = new ChartValues<double>();

            // 차트 Visible -> 기본 조도센서
            PhotoChartVisible = Visibility.Visible;
            TempChartVisible = Visibility.Hidden;
            HumiChartVisible = Visibility.Hidden;
        }

        private void SerialSetting()
        {
            serial = new SerialPort(SelectedSerialPort);
            serial.DataReceived += Serial_DataReceived;

            IsBtnConnected = true;
        }
        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string sVal = serial.ReadLine();

            string[] ReceiveData = new string[3];
            ReceiveData = sVal.Split('/');

            (GetView() as MetroWindow).BeginInvoke(new System.Action(delegate { DisplayValue(ReceiveData[0], ReceiveData[1], ReceiveData[2]); }));
        }

        private void DisplayValue(string vP, string vT, string vH)
        {
            try
            {
                ushort photo = ushort.Parse(vP);
                double temp = double.Parse(vT);
                double humi = double.Parse(vH);
                if (photo < 0 || photo > maxPhotoVal || temp < 0 || temp > 50 || humi < 0 || humi > 100)
                    return;

                DateTime now = DateTime.Now;
                
                SensorData data = new SensorData(DateTime.Now, photo, temp, humi);
                SensorDataList.Add(data);

                ConnectedCount = SensorDataList.Count.ToString();
                PhotoPrgValue = photo;
                PhotoStr = data.PhotoValue.ToString();
                TempPrgValue = temp;
                TempStr = temp.ToString();
                HumiPrgValue = humi;
                HumiStr = humi.ToString();

                if(!IsSimulation)
                    InsertDataToDB(data);

                string item;

                if (mode == Mode.Photo)
                {
                    item = $"{DateTime.Now.ToString()}\t조도 : {photo}";
                    PhotoValues.Add(photo);
                }
                else if (mode == Mode.TempHumi)
                {
                    item = $"{DateTime.Now.ToString()}\t온도 : {temp}   습도 : {humi}";
                    TempValues.Add(temp);
                    HumiValues.Add(humi);
                }
                else
                {
                    item = string.Empty;
                }

                DataLog += ($"{item}\n");

                if (IsZoom)
                    XMinValue = SensorDataList.Count <= 5 ? 0 : SensorDataList.Count - 5;

                //XMaxValue = SensorDataList.Count <= 5 ? 5 : SensorDataList.Count - 1;
            }
            catch (Exception ex)
            {
                DataLog += ($"Error : {ex.Message}\n");
            }
        }

        private void InsertDataToDB(SensorData data)
        {
            using (MySqlConnection conn = new MySqlConnection(Commons.CONNSTR))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(Commons.strInsertQuery, conn);
                MySqlParameter paramDate = new MySqlParameter("@Date", MySqlDbType.DateTime)
                {
                    Value = data.Date
                };
                cmd.Parameters.Add(paramDate);
                MySqlParameter paramPhotoValue = new MySqlParameter("@PhotoValue", MySqlDbType.Int32)
                {
                    Value = data.PhotoValue
                };
                cmd.Parameters.Add(paramPhotoValue);
                MySqlParameter paramTempValue = new MySqlParameter("@TempValue", MySqlDbType.Double)
                {
                    Value = data.TempValue
                };
                cmd.Parameters.Add(paramTempValue);
                MySqlParameter paramHumiValue = new MySqlParameter("@HumiValue", MySqlDbType.Double)
                {
                    Value = data.HumiValue
                };
                cmd.Parameters.Add(paramHumiValue);
                cmd.ExecuteNonQuery();
            }
        }

        public bool CanConnetPort
        {
            get => (serial.PortName != "선택") && IsBtnConnected;
        }

        public void ConnetPort()
        {
            timer.Stop();
            IsSimulation = false;
            timer = new Timer();
            if (serial.PortName != "선택")
                serial.Open();

            ConnectedTime = $"연결시간 : {DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}";

            XMinValue = 0;
            //XMaxValue = 5;
            SensorDataList.Clear();
            PhotoValues.Clear();
            TempValues.Clear();
            HumiValues.Clear();

            IsBtnConnected = false;
            IsBtnDisConnected = true;
        }

        public bool CanDisconnetPort
        {
            get => (serial.PortName != "선택") && IsBtnDisConnected;
        }

        public void DisconnetPort()
        {
            if (serial.PortName != "선택")
                serial.Close();

            //ConnectedTime = string.Empty;
            //ConnectedCount = string.Empty;

            //PhotoPrgValue = 0;
            //PhotoStr = string.Empty;
            //TempPrgValue = 0;
            //TempStr = string.Empty;
            //HumiPrgValue = 0;
            //HumiStr = string.Empty;

            IsBtnConnected = true;
            IsBtnDisConnected = false;
        }

        public bool CanChangeMode
        {
            get => (serial.PortName == "선택") ? !IsSimulation : (!IsBtnConnected && IsBtnDisConnected && !IsSimulation);
        }

        public void ChangeMode()
        {
            if (mode == Mode.Photo)
            {
                mode = Mode.TempHumi;
                SelectedMode = "Temp/Humi";

                SensorDataList = new List<SensorData>();
                PhotoValues = new ChartValues<int>();
                TempValues = new ChartValues<double>();
                HumiValues = new ChartValues<double>();

                YMaxValue = 105;

                PhotoPrgVisible = Visibility.Hidden;
                TempPrgVisible = Visibility.Visible;
                HumiPrgVisible = Visibility.Visible;

                PhotoStrVisible = Visibility.Hidden;
                TempStrVisible = Visibility.Visible;
                HumiStrVisible = Visibility.Visible;

                PhotoChartVisible = Visibility.Hidden;
                TempChartVisible = Visibility.Visible;
                HumiChartVisible = Visibility.Visible;
            }
            else if (mode == Mode.TempHumi)
            {
                mode = Mode.Photo;
                SelectedMode = "Photo";

                SensorDataList = new List<SensorData>();
                PhotoValues = new ChartValues<int>();
                TempValues = new ChartValues<double>();
                HumiValues = new ChartValues<double>();

                YMaxValue = 1030;

                PhotoPrgVisible = Visibility.Visible;
                TempPrgVisible = Visibility.Hidden;
                HumiPrgVisible = Visibility.Hidden;

                PhotoStrVisible = Visibility.Visible;
                TempStrVisible = Visibility.Hidden;
                HumiStrVisible = Visibility.Hidden;

                PhotoChartVisible = Visibility.Visible;
                TempChartVisible = Visibility.Hidden;
                HumiChartVisible = Visibility.Hidden;
            }
        }

        public void ProgramExit()
        {
            (GetView() as MetroWindow).Close();
        }

        public void StartSimulation()
        {
            ConnectedTime = $"연결시간 : {DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}";

            XMinValue = 0;
            //XMaxValue = 5;
            SensorDataList.Clear();
            PhotoValues.Clear();
            TempValues.Clear();
            HumiValues.Clear();

            IsSimulation = true;
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();

            // serial통신 끊기
            if (serial.PortName != "선택")
                DisconnetPort();
        }

        public void Timer_Tick(object sender, EventArgs e)
        {
            ushort vP = (ushort)rand.Next(1, 1024);
            double vT = (double)rand.Next(0, 500) / 10;
            double vH = (double)rand.Next(0, 1000) / 10;

            DisplayValue(vP.ToString(), vT.ToString(), vH.ToString());
        }

        public void StopSimulation()
        {
            timer.Stop();
            timer = new Timer();
            IsSimulation = false;

            //ConnectedTime = string.Empty;
            //ConnectedCount = string.Empty;

            //PhotoPrgValue = 0;
            //PhotoStr = string.Empty;
            //TempPrgValue = 0;
            //TempStr = string.Empty;
            //HumiPrgValue = 0;
            //HumiStr = string.Empty;
        }

        // 정보창으로 이동
        public void InfoOpen()
        {
            IWindowManager btninfo = new WindowManager();
            btninfo.ShowDialog(new InfoViewModel(), null, null);
        }

        public void ViewAll()
        {
            IsZoom = false;
            XMinValue = 0;
            //EnableZoom = ZoomingOptions.None;
            //XMaxValue = SensorDataList.Count <= 5 ? 5 : SensorDataList.Count - 1;
        }

        public void ZoomIn()
        {
            IsZoom = true;
            //EnableZoom = ZoomingOptions.X;
            XMinValue = SensorDataList.Count <= 5 ? 0 : SensorDataList.Count - 5;
            //XMaxValue = SensorDataList.Count <= 5 ? 5 : SensorDataList.Count - 1;
        }

        private void SelectDataDB()
        {
            using (MySqlConnection conn = new MySqlConnection(Commons.CONNSTR))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(Commons.strSelectQuery, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                AllSensorDataList = new List<SensorData>();
                while (reader.Read())
                {
                    var temp = new SensorData
                    {
                        Date = (DateTime)reader["Date"],
                        PhotoValue = (int)reader["PhotoValue"],
                        TempValue = (double)reader["TempValue"],
                        HumiValue = (double)reader["HumiValue"]
                    };
                    AllSensorDataList.Add(temp);
                }
            }
        }

        public void OpenAllData()
        {
            ShowViewModel openMenu = new ShowViewModel();
            IWindowManager windowManager = new WindowManager();
            windowManager.ShowDialog(openMenu, null, null);
        }

        // CSV 파일로 저장하기
        public void SaveCSVFile()
        {
            // 경로 지정 - 내 문서
            string mydoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folderPath = mydoc + @"\SaveData"; 
            string path = mydoc + @"\SaveData\SensorData.csv";

            // 폴더 만들기
            DirectoryInfo directory = new DirectoryInfo(folderPath);

            if (!directory.Exists)
                directory.Create();

            using (System.IO.StreamWriter csvFile = 
                new System.IO.StreamWriter(path, false, System.Text.Encoding.GetEncoding("utf-8"))) // 파일 경로 / 추가 / 인코딩
            {
                SelectDataDB();

                csvFile.WriteLine("Date,PhotoValue,TempValue,HumiValue");
                foreach (var sensorData in AllSensorDataList)
                {
                    csvFile.WriteLine($"{sensorData.Date},{sensorData.PhotoValue},{sensorData.TempValue},{sensorData.HumiValue}");
                }
            }

            System.Windows.MessageBox.Show("CSV파일 저장 완료!");
        }

        // Json 파일로 저장하기
        public void SaveJsonFile()
        {
            // 경로 지정 - 내 문서
            string mydoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folderPath = mydoc + @"\SaveData";
            string path = mydoc + @"\SaveData\SensorData.json";

            // 폴더 만들기
            DirectoryInfo directory = new DirectoryInfo(folderPath);

            if (!directory.Exists)
                directory.Create();

            string jsonStr;
            JArray jArray = new JArray();

            using (System.IO.StreamWriter jsonFile =
                new System.IO.StreamWriter(path, false, System.Text.Encoding.GetEncoding("utf-8")))
            {
                SelectDataDB();

                foreach (var sensorData in AllSensorDataList)
                {
                    jArray.Add((JObject)(JToken.FromObject(sensorData)));
                }

                jsonStr = jArray.ToString();
                jsonFile.WriteLine(jsonStr);
            }

            System.Windows.MessageBox.Show("Json파일 저장 완료!");
        }
    }
}
