using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArdMoni_mvvm.Helpers
{
    public class Commons
    {
        public static string CONNSTR =
            "Server=localhost;Port=3306;Database=iot_sensordata;Uid=root;Pwd=mysql_p@ssw0rd";

        public static string strQuery = "INSERT INTO sensordatatbl " +
                                        " (Date, Value) " +
                                        " VALUES " +
                                        " (@Date, @Value) ";
    }
}
