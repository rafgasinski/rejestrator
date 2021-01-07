namespace rejestrator.Models
{
    using LiveCharts.Defaults;
    using MySql.Data.MySqlClient;
    using rejestrator.Database;
    using System;
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

        public void GetLogsDatesForEmployee(List<string> dates, string id)
        {

            string query = @"SELECT date FROM `logs` WHERE `employeeID`=@id ORDER BY date DESC ";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        dates.Add(result.GetString(0));
                    }
                }
                Database.CloseConnection();
            }
        }

        public void GetLogsTasksForEmployee(List<string> tasks, string id)
        {

            string query = @"SELECT task FROM `tasks` WHERE `employeeID`=@id";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        tasks.Add(result.GetString(0));
                    }
                }
                Database.CloseConnection();
            }
        }

        public void GetLogsTasksInProgressForEmployee(List<TaskInProgressModel> tasks, string id)
        {

            string query = @"SELECT task, date FROM `tasksinprogress` WHERE `employeeID`=@id ORDER BY date DESC ";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        var temp = new TaskInProgressModel(result.GetString(0), result.GetString(1));
                        tasks.Add(temp);
                    }
                }
                Database.CloseConnection();
            }
        }

        public void GetLogsTasksDoneForEmployee(List<TaskDoneModel> tasks, string id)
        {

            string query = @"SELECT task, startdate, enddate, time FROM `tasksdone` WHERE `employeeID`=@id ORDER BY enddate DESC ";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        var temp = new TaskDoneModel(result.GetString(0), result.GetString(1), result.GetString(2), result.GetString(3));
                        tasks.Add(temp);
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
                        names.Add($"{result.GetString(0)} {result.GetString(1)}");
                    }
                }
                Database.CloseConnection();
            }
        }

        public void GetEmployeesFullNames(List<string> names)
        {
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
                        names.Add($"{result.GetString(0)} {result.GetString(1)}");
                    }
                }
                Database.CloseConnection();
            }
        }

        public void GetEmployeesFullNamesandID(List<string> names)
        {
            string query = @"SELECT employeeID,name,surname FROM `employees`";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        names.Add($"{result.GetString(0)} {result.GetString(1)} {result.GetString(2)}");
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

        public void InsertEmployee(string id, string pin, string name, string surname, string shift)
        {
            string query = @"INSERT INTO `employees`(`employeeID`, `pin`, `name`, `surname`, `shift`) VALUES(@id,@pin,@name,@surname,@shift)";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.Parameters.AddWithValue("@pin", pin);
                myCommand.Parameters.AddWithValue("@name", name);
                myCommand.Parameters.AddWithValue("@surname", surname);
                myCommand.Parameters.AddWithValue("@shift", shift);
                myCommand.CommandText = query;
                myCommand.ExecuteNonQuery();
                Database.CloseConnection();
            }
        }

        public void InsertTask(string id, string task)
        {
            string query = @"INSERT INTO `tasks`(`employeeID`, `task`) VALUES(@id,@task)";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id); 
                myCommand.Parameters.AddWithValue("@task", task);
                myCommand.CommandText = query;
                myCommand.ExecuteNonQuery();
                Database.CloseConnection();
            }
        }

        public int GetEmployeeLogsCount(string id)
        {
            int count = 0;

            string query = @"SELECT COUNT(`employeeID`) FROM `logs` WHERE `employeeID`=@id GROUP BY `employeeID`";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    count = result.GetInt32(0);
                }
                Database.CloseConnection();
            }
            return count;
        }

        public int GetEmployeeLogsCountToday(string id)
        {
            int count = 0;

            string query = @"SELECT COUNT(`employeeID`) FROM `logs` WHERE `employeeID`=@id AND date LIKE @pattern GROUP BY `employeeID`";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.Parameters.AddWithValue("@pattern", $"{DateTime.Now.ToString("dd/MM/yyyy")}{"%"}");        
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    count = result.GetInt32(0);
                }
                Database.CloseConnection();
            }
            return count;
        }

        public int GetEmployeeTasksDoneCountToday(string id)
        {
            int count = 0;

            string query = @"SELECT COUNT(`employeeID`) FROM `tasksdone` WHERE `employeeID`=@id AND enddate LIKE @pattern GROUP BY `employeeID`";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.Parameters.AddWithValue("@pattern", $"{DateTime.Now.ToString("dd/MM/yyyy")}{"%"}");
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    count = result.GetInt32(0);
                }
                Database.CloseConnection();
            }
            return count;
        }

        public int GetEmployeeCount()
        {
            int count = 0;

            string query = @"SELECT COUNT(`employeeID`) FROM `employees`";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    count = result.GetInt32(0);
                }
                Database.CloseConnection();
            }
            return count;
        }

        public void GetEmployeesIDs(List<string> ids)
        {
            string query = @"SELECT employeeID, name, surname FROM `employees`";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        ids.Add($"{result.GetString(0)} {result.GetString(1)} {result.GetString(2)}");
                    }
                }
                Database.CloseConnection();
            }
        }

        public int GetEmployeeTasksCount(string id)
        {
            int count = 0;

            string query = @"SELECT COUNT(`employeeID`) FROM `tasks` WHERE `employeeID`=@id GROUP BY `employeeID`";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    count = result.GetInt32(0);
                }
                Database.CloseConnection();
            }
            return count;
        }

        public int GetEmployeeTasksInProgressCount(string id)
        {
            int count = 0;

            string query = @"SELECT COUNT(`employeeID`) FROM `tasksinprogress` WHERE `employeeID`=@id GROUP BY `employeeID`";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    count = result.GetInt32(0);
                }
                Database.CloseConnection();
            }
            return count;
        }

        public int GetEmployeeTasksDoneCount(string id)
        {
            int count = 0;

            string query = @"SELECT COUNT(`employeeID`) FROM `tasksdone` WHERE `employeeID`=@id GROUP BY `employeeID`";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    count = result.GetInt32(0);
                }
                Database.CloseConnection();
            }
            return count;
        }
    }
}
