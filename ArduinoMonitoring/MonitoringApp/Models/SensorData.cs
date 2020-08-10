using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringApp.Models
{
    public class SensorData
    {
        //public DateTime Date { get; set; }
        DateTime date;
        public DateTime Date
        { 
            get => date; 
            set
            {
                date = value;
            }
        }
        public int PhotoValue { get; set; }
        public double TempValue { get; set; }
        public double HumiValue { get; set; }

        public SensorData()
        {

        }

        public SensorData(DateTime date, int p, double t, double h)
        {
            Date = date;
            PhotoValue = p;
            TempValue = t;
            HumiValue = h;
        }
    }
}
