using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringApp.Helpers
{
    public class Commons
    {
        public static string CONNSTR =
            "Server=localhost;Port=3306;Database=iot_sensordata;Uid=root;Pwd=mysql_p@ssw0rd";

        public static string strInsertQuery = "INSERT INTO arduinodatatbl " +
                                        " (Date, PhotoValue, TempValue, HumiValue) " +
                                        " VALUES " +
                                        " (@Date, @PhotoValue, @TempValue , @HumiValue) ";

        public static string strSelectQuery = "SELECT " +
                                              "     Date, " +
                                              "     PhotoValue, " +
                                              "     TempValue, " +
                                              "     HumiValue " +
                                              "  FROM arduinodatatbl ";
    }
}
