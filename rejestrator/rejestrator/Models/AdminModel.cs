namespace rejestrator.Models
{
    using MySql.Data.MySqlClient;
    using rejestrator.Database;
    using System.Collections.Generic;

    public class AdminModel
    {
        #region Singleton
        private static AdminModel _instance = null;
        public static AdminModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AdminModel();
                }

                return _instance;
            }
        }
        #endregion

        public void GetLogsEmployeeID(List<string> ids)
        {

            string query = @"SELECT employeeID FROM `logs` ORDER BY date DESC ";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        ids.Add(result.GetString(0));
                    }
                }
                Database.CloseConnection();
            }
        }

        public void GetLogsDates(List<string> dates)
        {

            string query = @"SELECT date FROM `logs` ORDER BY date DESC ";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    while(result.Read())
                    {
                        dates.Add(result.GetString(0));
                    }
                }
                Database.CloseConnection();
            }
        }

        public void GetLogsNames(List<string> names)
        {

            string query = @"SELECT name,surname FROM `logs` ORDER BY date DESC ";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    while(result.Read())
                    {
                        names.Add(result.GetString(0) + " " + result.GetString(1));
                    }
                }
                Database.CloseConnection();
            }
        }

        public void GetEmployeesFullNames(List<string> names)
        {
            string employeeSurnname = string.Empty;

            string query = @"SELECT name,surname FROM `employees`";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        names.Add(result.GetString(0) + " " + result.GetString(1));
                    }
                }
                Database.CloseConnection();
            }
        }

        public bool EmployeeIDUsed(string id)
        {
            bool employeeId = false;

            string query = @"SELECT COUNT(`id`) FROM `employees` WHERE `employeeID`=@id GROUP BY `id`";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    employeeId = true;
                }
                Database.CloseConnection();
            }
            return employeeId;
        }

        public bool TaskAlreadyIn(string id)
        {
            bool taskUsed = false;

            string query = @"SELECT COUNT(`task`) FROM `tasks` WHERE `employeeID`=@id GROUP BY `task`";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    taskUsed = true;
                }
                Database.CloseConnection();
            }
            return taskUsed;
        }

        public void InsertEmployee(string id, string pin, string name, string surname)
        {
            string query = @"INSERT INTO `employees`(`employeeID`, `pin`, `name`, `surname`) VALUES(@id,@pin,@name,@surname)";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.Parameters.AddWithValue("@pin", pin);
                myCommand.Parameters.AddWithValue("@name", name);
                myCommand.Parameters.AddWithValue("@surname", surname);
                myCommand.CommandText = query;
                myCommand.ExecuteNonQuery();
                Database.CloseConnection();
            }
        }

        public void InsertTask(string id, string name, string surname, string task)
        {
            string query = @"INSERT INTO `tasks`(`employeeID`, `name`, `surname`, `task`) VALUES(@id,@name,@surname,@task)";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id); 
                myCommand.Parameters.AddWithValue("@name", name);
                myCommand.Parameters.AddWithValue("@surname", surname);
                myCommand.Parameters.AddWithValue("@task", task);
                myCommand.CommandText = query;
                myCommand.ExecuteNonQuery();
                Database.CloseConnection();
            }
        }
    }
}
