namespace rejestrator.Models
{
    using MySql.Data.MySqlClient;
    using System.Collections.Generic;
    using rejestrator.Database;
    using System;

    public class EmployeeModel
    {
        #region Singleton
        private static EmployeeModel _instance = null;
        public static EmployeeModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EmployeeModel();
                }

                return _instance;
            }
        }
        #endregion

        #region Methods

        public void GetTasksAvailable(ref List<TaskAvailableModel> tasksAvailable, string id)
        {
            string query = @"SELECT id, task FROM `tasks` WHERE `employeeID`=@id";
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
                        tasksAvailable.Add(new TaskAvailableModel(result.GetInt32(0), result.GetString(1)));
                    }
                }
                Database.CloseConnection();
            }
        }

        public void GetTasksInProgress(ref List<TaskInProgressModel> tasksInProgress, string id)
        {

            string query = @"SELECT id, task, date FROM `tasksinprogress` WHERE `employeeID`=@id ORDER BY date DESC ";
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
                        tasksInProgress.Add(new TaskInProgressModel(result.GetInt32(0), result.GetString(1), result.GetString(2)));
                    }
                }
                Database.CloseConnection();
            }
        }

        public void GetTasksDone(ref List<TaskDoneModel> tasksDone, string id)
        {

            string query = @"SELECT id, task, startdate, enddate, time FROM `tasksdone` WHERE `employeeID`=@id ORDER BY enddate DESC ";
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
                        tasksDone.Add(new TaskDoneModel(result.GetInt32(0), result.GetString(1), result.GetString(2), result.GetString(3), result.GetString(4)));
                    }
                }
                Database.CloseConnection();
            }
        }

        public void StartTask(TaskAvailableModel task, string employeeID)
        {
            string query1 = @"DELETE FROM `tasks` WHERE `id`=@id";
            using (MySqlCommand myCommand = new MySqlCommand(query1, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", task.ID);
                myCommand.CommandText = query1;
                MySqlDataReader result = myCommand.ExecuteReader();
                Database.CloseConnection();
            }

            string query2 = @"INSERT INTO `tasksinprogress`(`employeeID`, `task`, `date`) VALUES (@employeeID, @task, @date)";
            using (MySqlCommand myCommand = new MySqlCommand(query2, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@employeeID", employeeID);
                myCommand.Parameters.AddWithValue("@task", task.Task);
                myCommand.Parameters.AddWithValue("@date", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                myCommand.CommandText = query2;
                MySqlDataReader result = myCommand.ExecuteReader();
                Database.CloseConnection();
            }
        }

        public void EndTask(TaskInProgressModel task, string employeeID)
        {
            string query1 = @"DELETE FROM `tasksinprogress` WHERE `id`=@id";
            using (MySqlCommand myCommand = new MySqlCommand(query1, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", task.ID);
                myCommand.CommandText = query1;
                MySqlDataReader result = myCommand.ExecuteReader();
                Database.CloseConnection();
            }

            string query2 = @"INSERT INTO `tasksdone`(`employeeID`, `task`, `startdate`, `enddate`, `time`) VALUES (@employeeID, @task, @startDate, @endDate, @time)";
            using (MySqlCommand myCommand = new MySqlCommand(query2, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@employeeID", employeeID);
                myCommand.Parameters.AddWithValue("@task", task.Task);
                myCommand.Parameters.AddWithValue("@startDate", task.Date);
                myCommand.Parameters.AddWithValue("@endDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

                //TIME NEEDS TO BE CHANGED HERE
                myCommand.Parameters.AddWithValue("@time", "0");
                myCommand.CommandText = query2;
                MySqlDataReader result = myCommand.ExecuteReader();
                Database.CloseConnection();
            }
        }

        #endregion
    }
}
