using Caliburn.Micro;
using LiveCharts;
using MonitoringApp.Helpers;
using MonitoringApp.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;

namespace MonitoringApp.ViewModels
{
    class ShowViewModel : Conductor<object>
    {
        public ShowViewModel()
        {
            PhotoVisible = false;
            TempVisible = false;
            HumiVisible = false;

            SelectDataDB();
            //Thread thread = new Thread(() => SelectDataDB());
            //Thread thread = new Thread(new ThreadStart(SelectDataDB));
            //thread.Start();
        }

        BindableCollection<SensorData> sensors;
        public BindableCollection<SensorData> Sensors
        {
            get => sensors;
            set
            {
                sensors = value;
                NotifyOfPropertyChange(() => Sensors);
            }
        }

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

        private bool photoVisible;
        public bool PhotoVisible
        {
            get => photoVisible;
            set
            {
                photoVisible = value;
                NotifyOfPropertyChange(() => PhotoVisible);
            }
        }
        private bool tempVisible;
        public bool TempVisible
        {
            get => tempVisible;
            set
            {
                tempVisible = value;
                NotifyOfPropertyChange(() => TempVisible);
            }
        }
        private bool humiVisible;
        public bool HumiVisible
        {
            get => humiVisible;
            set
            {
                humiVisible = value;
                NotifyOfPropertyChange(() => HumiVisible);
            }
        }

        private void SelectDataDB()
        {
            using (MySqlConnection conn = new MySqlConnection(Commons.CONNSTR))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(Commons.strSelectQuery, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                Sensors = new BindableCollection<SensorData>();
                PhotoValues = new ChartValues<int>();
                TempValues = new ChartValues<double>();
                HumiValues = new ChartValues<double>();

                while (reader.Read())
                {
                    var temp = new SensorData
                    {
                        Date = (DateTime)reader["Date"],
                        PhotoValue = (int)reader["PhotoValue"],
                        TempValue = (double)reader["TempValue"],
                        HumiValue = (double)reader["HumiValue"]
                    };
                    Sensors.Add(temp);
                    PhotoValues.Add((int)reader["PhotoValue"]);
                    TempValues.Add((double)reader["TempValue"]);
                    HumiValues.Add((double)reader["HumiValue"]);
                }
            }
        }
    }
}
