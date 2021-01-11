namespace rejestrator.Database
{
    using System;
    using System.IO;
    using MySql.Data.MySqlClient;
    class Database
    {
        private static MySqlConnection DBConnect;

        public static void OpenConnection()
        {
            if(DBConnect.State != System.Data.ConnectionState.Open)
            {
                DBConnect.Open();
            }
        }

        public static void CloseConnection()
        {
            if (DBConnect.State != System.Data.ConnectionState.Closed)
            {
                DBConnect.Close();
            }
        }

        public static MySqlConnection DBConnection()
        {
            string ConnectString = "datasource = localhost; username = root; password=; database = rejestrator";
            //ConnectString = "datasource = sql7.freemysqlhosting.net; username = sql7386276; password = Fa8xS3LndM; database = sql7386276";
            DBConnect = new MySqlConnection(ConnectString);

            return DBConnect;
        }
    }
}
