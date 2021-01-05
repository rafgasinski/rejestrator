namespace rejestrator.Models
{
    using MySql.Data.MySqlClient;
    using rejestrator.Database;
    using System.Collections.Generic;

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
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.Parameters.AddWithValue("@pin", pin);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
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
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@username", username);
                myCommand.Parameters.AddWithValue("@password", password);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                    canLogin = true;
                Database.CloseConnection();
            }
            return canLogin;
        }

        public string GetAdminFullName(string username)
        {
            string adminFullName = string.Empty;

            string query = @"SELECT name,surname FROM `administrators` WHERE `username`=@username";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@username", username);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    adminFullName = $"{result.GetString(0)} {result.GetString(1)}";
                }
                Database.CloseConnection();
            }

            return adminFullName;
        }

        public string GetAdminID(string username, string password)
        {
            string id = string.Empty;

            string query = @"SELECT COUNT(`id`) FROM `administrators` WHERE `username`=@username AND `password`=@password GROUP BY `id`";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@username", username);
                myCommand.Parameters.AddWithValue("@password", password);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    id = result.GetString(0);
                }
                Database.CloseConnection();
            }
            return id;
        }

        public string GetEmployeeID(string id, string pin)
        {
            string employeeId = string.Empty;

            string query = @"SELECT COUNT(`id`) FROM `employees` WHERE `employeeID`=@id AND `pin`=@pin GROUP BY `id`";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.Parameters.AddWithValue("@pin", pin);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    employeeId = result.GetString(0);
                }
                Database.CloseConnection();
            }
            return employeeId;
        }


        public string GetEmployeeFullName(string id)
        {
            string employeeFullName = string.Empty;

            string query = @"SELECT name,surname FROM `employees` WHERE `employeeID`=@employeeID";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@employeeID", id);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    employeeFullName = $"{result.GetString(0)} {result.GetString(1)}";
                }
                Database.CloseConnection();
            }

            return employeeFullName;
        }

        public string GetEmployeeName(string id)
        {
            string employeeName = string.Empty;

            string query = @"SELECT name FROM `employees` WHERE `employeeID`=@employeeID";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@employeeID", id);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    employeeName = result.GetString(0);
                }
                Database.CloseConnection();
            }

            return employeeName;
        }

        public string GetEmployeeSurname(string id)
        {
            string employeeSurnname = string.Empty;

            string query = @"SELECT surname FROM `employees` WHERE `employeeID`=@employeeID";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@employeeID", id);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    employeeSurnname = result.GetString(0);
                }
                Database.CloseConnection();
            }

            return employeeSurnname;
        }

        public void InsertLoginDate(string id, string name, string surname, string date)
        {
            string query = @"INSERT INTO `logs`(`employeeID`, `name`, `surname`, `date`) VALUES(@id,@name,@surname,@date)";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.Parameters.AddWithValue("@name", name);
                myCommand.Parameters.AddWithValue("@surname", surname);
                myCommand.Parameters.AddWithValue("@date", date);
                myCommand.CommandText = query;
                myCommand.ExecuteNonQuery();
                Database.CloseConnection();
            }
        }
    }
}
