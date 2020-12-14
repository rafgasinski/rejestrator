namespace rejestrator.Models
{
    using System.Data.SQLite;
    using rejestrator.Database;
    public class LoginModel
    {
        #region Singleton
        private static LoginModel _instance = null;
        public static LoginModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LoginModel();
                }

                return _instance;
            }
        }
        #endregion

        public bool LoginEmployee(string id, string pin)
        {
            bool canLogin = false;

            string query = @"SELECT COUNT(`id`) FROM `employees` WHERE `employeeID`=@id AND `pin`=@pin GROUP BY `id`";
            using (SQLiteCommand myCommand = new SQLiteCommand(query, Database.MyConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.Parameters.AddWithValue("@pin", pin);
                myCommand.CommandText = query;
                SQLiteDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                    canLogin = true;
                Database.CloseConnection();
            }
            return canLogin;
        }

        public bool LoginAdmin(string username, string password)
        {
            bool canLogin = false;

            string query = @"SELECT COUNT(`id`) FROM `administrators` WHERE `username`=@username AND `password`=@password GROUP BY `id`";
            using (SQLiteCommand myCommand = new SQLiteCommand(query, Database.MyConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@username", username);
                myCommand.Parameters.AddWithValue("@password", password);
                myCommand.CommandText = query;
                SQLiteDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                    canLogin = true;
                Database.CloseConnection();
            }
            return canLogin;
        }
    }
}
