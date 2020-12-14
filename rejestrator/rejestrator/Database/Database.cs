namespace rejestrator.Database
{
    using System.Data.SQLite;
    using System.IO;
    class Database
    {
        private static SQLiteConnection myConnection;
        public static SQLiteConnection MyConnection()
        {
            if (!File.Exists("./database.sqlite3"))
            {
                SQLiteConnection.CreateFile("database.sqlite3");
            }
            myConnection = new SQLiteConnection("Data Source=database.sqlite3");
            return myConnection;
        }
        public static void OpenConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Open)
            {
                myConnection.Open();
            }
        }

        public static void CloseConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Closed)
            {
                myConnection.Close();
            }
        }
    }
}
