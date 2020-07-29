using System;

namespace ArduinoMonitoring
{
    public class SensorData
    {
        public DateTime Date { get; set; }
        public ushort Value { get; set; }

        public SensorData(DateTime date, ushort value)
        {
            Date = date;
            Value = value;
        }
    }
}
