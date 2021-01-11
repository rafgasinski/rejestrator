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

        #region Properties

        public string ID { get; set; }
        public string Name { get; set; }
        public string Shift { get; set; }

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

        public void GetTasksDone(ref List<TaskDoneModel> tasksDone, string id, string date, string date2)
        {
       
            string query1 = @"SELECT id, task, startdate, enddate, time FROM `tasksdone` WHERE `employeeID`=@id AND `enddate` LIKE @pattern ORDER BY enddate DESC ";
            using (MySqlCommand myCommand = new MySqlCommand(query1, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.Parameters.AddWithValue("@pattern", $"{date2}{'%'}");
                myCommand.CommandText = query1;
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

            string query2 = @"SELECT id, task, startdate, enddate, time FROM `tasksdone` WHERE `employeeID`=@id AND `enddate` LIKE @pattern ORDER BY enddate DESC ";
            using (MySqlCommand myCommand = new MySqlCommand(query2, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.Parameters.AddWithValue("@pattern", $"{date}{'%'}");
                myCommand.CommandText = query2;
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
            DateTime DateStart = DateTime.Parse(task.Date);

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

                if(Shift == "Dzienny")
                    myCommand.Parameters.AddWithValue("@time", Calc(DateStart, DateTime.Now, 8, 0, 18, 0));
                else
                    myCommand.Parameters.AddWithValue("@time", Calc(DateStart, DateTime.Now, 18, 0, 4, 0));
                myCommand.CommandText = query2;
                MySqlDataReader result = myCommand.ExecuteReader();
                Database.CloseConnection();
            }

            var lol = Calc(DateStart, DateTime.Now, 18, 0, 4, 0);
        }

        public string CanEmployeeStartEndTask(string id)
        {
            string canEmployeeStartEndTask = string.Empty;

            string query = @"SELECT pin FROM `employees` WHERE `employeeID`=@id";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    canEmployeeStartEndTask = result.GetString(0);
                }
                Database.CloseConnection();
            }
            return canEmployeeStartEndTask;
        }

        public bool CheckIfLoggedOnThisDay(string date, string employeeID)
        {
            bool loggenIn = false;

            string query = @"SELECT date FROM `logs` WHERE `employeeID`=@id AND date LIKE @pattern ORDER BY date ASC LIMIT 1";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", employeeID);
                myCommand.Parameters.AddWithValue("@pattern", $"{date}{'%'}");
                myCommand.CommandText = query;
                MySqlDataReader result = myCommand.ExecuteReader();
                if (result.HasRows)
                {
                    loggenIn = true;
                }
                Database.CloseConnection();
            }

            return loggenIn;
        }

        public string Calc(DateTime start, DateTime end, int startHour, int startMin, int endHour, int endMin)
        {
            if (start > end)
                throw new Exception();

            TimeSpan shfitStart = new TimeSpan(startHour, startMin, 0);
            TimeSpan shiftEnd = new TimeSpan(endHour, endMin, 0);


            if (start.Date == end.Date && start.TimeOfDay >= shiftEnd && end.TimeOfDay <= shfitStart)
            {
                return "0";
            }

            if (start.Date == end.Date)
            {
                if (start.TimeOfDay > shfitStart || end.TimeOfDay < shiftEnd)
                {
                    var totalMin = (end - start).TotalMinutes;

                    var hours1 = 0;
                    for (double i = totalMin; i >= 60; i -= 60)
                    {
                        hours1++;
                        totalMin -= 60;
                    }

                    if (hours1 != 0 && Math.Round(totalMin, 0) != 0)
                    {
                        return $"{hours1}{"h"} {Math.Round(totalMin, 0)}{"min."}";
                    }
                    else if (hours1 != 0 && Math.Round(totalMin, 0) == 0)
                    {
                        return $"{hours1}{"h"}";
                    }
                    else if (Math.Round(totalMin, 0) != 0)
                    {
                        return $"{Math.Round(totalMin, 0)}{"min."}";
                    }
                    else
                    {
                        return $"{Math.Round(totalMin * 60, 0)} {"sek."}";
                    }
                }

                double total = 0;
                if (start.TimeOfDay < shiftEnd)
                {
                    total += (shiftEnd - start.TimeOfDay).TotalMinutes;
                }
                if (end.TimeOfDay > shfitStart)
                {
                    total += (end.TimeOfDay - shfitStart).TotalMinutes;
                }

                var hours = 0;
                for (double i = total; i >= 60; i -= 60)
                {
                    hours++;
                    total -= 60;
                }

                if (hours != 0 && Math.Round(total, 0) != 0)
                {
                    return $"{hours}{"h"} {Math.Round(total, 0)}{"min."}";
                }
                else if (hours != 0 && Math.Round(total, 0) == 0)
                {
                    return $"{hours}{"h"}";
                }
                else if (Math.Round(total, 0) != 0)
                {
                    return $"{Math.Round(total, 0)}{"min."}";
                }
                else
                {
                    return $"{Math.Round(total * 60, 0)} {"sek."}";
                }
            }
            else
            {
                double total = 0;

                if (CheckIfLoggedOnThisDay(start.ToString("dd/MM/yyyy"), ID))
                {
                    if (start.TimeOfDay < shiftEnd)
                    {
                        total += (shiftEnd - start.TimeOfDay).TotalMinutes;
                    }
                    if (start.TimeOfDay < shfitStart)
                    {
                        total += ((new TimeSpan(24, 0, 0)) - shfitStart).TotalMinutes;
                    }
                    else
                    {
                        total += ((new TimeSpan(24, 0, 0)) - start.TimeOfDay).TotalMinutes;
                    }
                }

                if (CheckIfLoggedOnThisDay(end.ToString("dd/MM/yyyy"), ID))
                {
                    if (end.TimeOfDay > shfitStart)
                    {
                        total += (end.TimeOfDay - shfitStart).TotalMinutes;
                    }
                    if (end.TimeOfDay > shiftEnd)
                    {
                        total += shiftEnd.TotalMinutes;
                    }
                    else
                    {
                        total += end.TimeOfDay.TotalMinutes;
                    }
                }
                   
                int numberOfFullDays = (end - start).Days;
                if (end.TimeOfDay > start.TimeOfDay)
                {
                    numberOfFullDays--;
                }
                if (numberOfFullDays > 0)
                {
                    for(int i = 1; i <= numberOfFullDays; i++)
                    {
                        double hoursInFullDay = ((new TimeSpan(24, 0, 0)) - shfitStart).TotalMinutes + shiftEnd.TotalMinutes;

                        var nextDay = start.AddDays(i);
                        if (CheckIfLoggedOnThisDay(nextDay.ToString("dd/MM/yyyy"), ID))
                        {
                            total += hoursInFullDay;
                        }
                    }                
                }

                var hours = 0;
                for (double i = total; i >= 60; i -= 60)
                {
                    hours++;
                    total -= 60;
                }

                if (hours != 0 && Math.Round(total, 0) != 0)
                {
                    return $"{hours}{"h"} {Math.Round(total, 0)}{"min."}";
                }
                else if(hours != 0 && Math.Round(total, 0) == 0)
                {
                    return $"{hours}{"h"}";
                }
                else if(Math.Round(total, 0) != 0)
                {
                    return $"{Math.Round(total, 0)}{"min."}";
                }
                else
                {
                    return $"{Math.Round(total*60, 0)} {"sek."}";
                }
            }
        }

        #endregion
    }
}
