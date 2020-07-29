namespace SecondCaliburnApp.Helpers
{
    public class Commons
    {
        public static string STRCONNSTRING =
            "Server=localhost;Port=3306;Database=testdb;uid=root;password=mysql_p@ssw0rd"; // Server = Data Source

        public static string SELECTPEOPLETBLQUERY = "SELECT id, FirstName, LastName " +
                                                    "  FROM peopletbl " +
                                                    " ORDER BY id";
    }
}
