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
            DBConnect = new MySqlConnection(ConnectString);

            return DBConnect;
        }
    }
}
