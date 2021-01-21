namespace rejestrator.Models
{
    using System.Collections.Generic;
    using System;
    using API;
    using API.Entities;
    using Utils;
    using Newtonsoft.Json;
    using rejestrator.Converters;

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
        public string pin { get; set; }
        public string Name { get; set; }
        public string Shift { get; set; }

        #endregion

        #region Methods

        public void GetTasksAvailable(ref List<TaskAvailableModel> tasksAvailable, string id)
        {
            string response = APIService.makeRequest(HTTPMethod.GET, $"tasksAvailable/{id}");

            if (Error.IsResponseError(response))
                throw new NotImplementedException();

            List<TaskAvailableEntity> tasks = JsonConvert.DeserializeObject<List<TaskAvailableEntity>>(response);

            foreach(var task in tasks)
            {
                tasksAvailable.Add(new TaskAvailableModel(task.id, task.task));
            }
        }

        public void GetTasksInProgress(ref List<TaskInProgressModel> tasksInProgress, string id)
        {
            string response = APIService.makeRequest(HTTPMethod.GET, $"tasksInProgress/{id}");

            if (Error.IsResponseError(response))
                throw new NotImplementedException();

            List<TaskInProgressEntity> tasks = JsonConvert.DeserializeObject<List<TaskInProgressEntity>>(response);

            foreach (var task in tasks)
            {
                tasksInProgress.Add(new TaskInProgressModel(task.id, task.task, task.date));
            }
        }

        public void GetTasksDone(ref List<TaskDoneModel> tasksDone, string id, string date)
        {
            string response = APIService.makeRequest(HTTPMethod.GET, $"tasksDone/{id}/{date}");

            if (Error.IsResponseError(response))
                throw new NotImplementedException();

            List<TaskDoneEntity> tasks = JsonConvert.DeserializeObject<List<TaskDoneEntity>>(response);

            foreach (var task in tasks)
            {
                tasksDone.Add(new TaskDoneModel(task.id, task.task, task.startdate, task.enddate, task.time));
            }
        }

        public void StartTask(TaskAvailableModel task, string employeeID)
        {
            string response1 = APIService.makeRequest(HTTPMethod.DELETE, $"startTask/{task.ID}");

            if (Error.IsResponseError(response1))
                throw new NotImplementedException();

            var taskInProgress = new TaskInProgressEntity
            {
                employeeID = employeeID,
                task = task.Task,
                date = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
            };

            string response2 = APIService.makeRequest(HTTPMethod.POST, "tasksInProgress", taskInProgress.ToKeyValueURL());

            if (Error.IsResponseError(response2))
                throw new NotImplementedException();
        }

        public void EndTask(TaskInProgressModel task, string employeeID)
        {
            DateTime DateStart = DateTime.Parse(task.Date);
            string response1 = APIService.makeRequest(HTTPMethod.DELETE, $"endTask/{task.ID}");

            if (Error.IsResponseError(response1))
                throw new NotImplementedException();

            var taskDone = new TaskDoneEntity
            {
                employeeID = employeeID,
                task = task.Task,
                startdate = task.Date,
                enddate = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                time = Shift == "Dzienny" ? CalcDay(DateStart, DateTime.Now) : CalcNight(DateStart, DateTime.Now, 18, 0, 4, 0)
            };

            string response2 = APIService.makeRequest(HTTPMethod.POST, "tasksDone", taskDone.ToKeyValueURL());

            if (Error.IsResponseError(response2))
                throw new NotImplementedException();  
        }

        public bool CheckIfLoggedOnThisDay(string date, string employeeID)
        {
            string response = APIService.makeRequest(HTTPMethod.GET, $"logs/{employeeID}/{date}");

            LogEntity log = JsonConvert.DeserializeObject<LogEntity>(response);

            if (log == null)
                return false;

            return true;
        }

        public string CalcNight(DateTime start, DateTime end, int startHour, int startMin, int endHour, int endMin)
        {
            if (start > end)
                throw new Exception();

            TimeSpan shiftStart = new TimeSpan(startHour, startMin, 0);
            TimeSpan shiftEnd = new TimeSpan(endHour, endMin, 0);


            if (start.Date == end.Date && start.TimeOfDay >= shiftEnd && end.TimeOfDay <= shiftStart)
            {
                return "-";
            }

            if (start.Date == end.Date)
            {
                if (start.TimeOfDay > shiftStart || end.TimeOfDay < shiftEnd)
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
                        return $"{Math.Round(totalMin * 60, 0)}{"sek."}";
                    }
                }

                double total = 0;
                if (start.TimeOfDay < shiftEnd)
                {
                    total += (shiftEnd - start.TimeOfDay).TotalMinutes;
                }
                if (end.TimeOfDay > shiftStart)
                {
                    total += (end.TimeOfDay - shiftStart).TotalMinutes;
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
                    return $"{Math.Round(total * 60, 0)}{"sek."}";
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
                    if (start.TimeOfDay < shiftStart)
                    {
                        total += ((new TimeSpan(24, 0, 0)) - shiftStart).TotalMinutes;
                    }
                    else
                    {
                        total += ((new TimeSpan(24, 0, 0)) - start.TimeOfDay).TotalMinutes;
                    }
                }

                if (CheckIfLoggedOnThisDay(end.ToString("dd/MM/yyyy"), ID))
                {
                    if (end.TimeOfDay > shiftStart)
                    {
                        total += (end.TimeOfDay - shiftStart).TotalMinutes;
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
                        double hoursInFullDay = ((new TimeSpan(24, 0, 0)) - shiftStart).TotalMinutes + shiftEnd.TotalMinutes;

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
                    return $"{Math.Round(total*60, 0)}{"sek."}";
                }
            }
        }

        public string CalcDay(DateTime start, DateTime stop)
        {
            //Jeżeli daty są te same, nie upłynął:
            if (start == stop)
                return "-";

            //Inicjowanie zmiennych:
            double total;
            int hours;

            if (start > stop)
            {
                DateTime temp = start;
                start = stop;
                stop = temp;
            }

            //Ustalenie początku zmiany oraz końca zmiany z daty startu:

            DateTime startFloor = new DateTime(start.Year, start.Month, start.Day, 8, 0, 0);
            DateTime startCeil = new DateTime(start.Year, start.Month, start.Day, 18, 0, 0);

            if (start < startFloor) start = startFloor;
            if (start > startCeil) start = startCeil;

            //Sprawdzenie czy użytkownik logował się w dniu rozpoczęcia zadania:

            TimeSpan firstDayTime = startCeil - start;
            bool loggedIn = true;
            if (!CheckIfLoggedOnThisDay(start.ToString("dd/MM/yyyy"), ID))
            {
                loggedIn = false;
                firstDayTime = TimeSpan.Zero;
            }

            //Ustalenie początku zmiany oraz końca zmiany z daty zakończenia zadania:

            DateTime stopFloor = new DateTime(stop.Year, stop.Month, stop.Day, 8, 0, 0);
            DateTime stopCeil = new DateTime(stop.Year, stop.Month, stop.Day, 18, 0, 0);
            if (stop < stopFloor) stop = stopFloor;
            if (stop > stopCeil) stop = stopCeil;

            //Sprawdzenie czy użytkownik logował się w dniu zakończenia zadania:

            TimeSpan lastDayTime = stop - stopFloor;
            if (!CheckIfLoggedOnThisDay(stop.ToString("dd/MM/yyyy"), ID))
                lastDayTime = TimeSpan.Zero;

            //Jeżęli data dzienna (bez godzin) jest taka sama dla startu i końca:

            if (start.Date == stop.Date)
            {
                //Zwróc 0 min jeżeli uzytkownik nie logował się do systemu:
                if (!loggedIn)
                    return "0min.";

                //Obliczenie minut z różnycy dni
                total = (stop - start).TotalMinutes;

                //Wyliczenie godzin oraz minut z .TotalMinutes
                hours = 0;
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
                    return $"{Math.Round(total * 60, 0)}{"sek."}";
                }
            }

            //Ustawienie zmiennych, czasu między zadaniami oraz godzin w ciągu jednej zmiany
            TimeSpan timeInBetween = TimeSpan.Zero;
            TimeSpan hoursInAWholeDay = (startCeil - startFloor);

            //Dodanie liczby wszystkich godzin w ciągu zmiany jeżeli użytkownik zalogował się w danym dniu
            for (DateTime itr = startFloor.AddDays(1); itr < stopFloor; itr = itr.AddDays(1))
            {
                if (!CheckIfLoggedOnThisDay(itr.ToString("dd.MM.yyyy"), ID))
                    continue;

                timeInBetween += hoursInAWholeDay;
            }

            //Wyliczenie wszystkich minut
            total =  (firstDayTime + lastDayTime + timeInBetween).TotalMinutes;

            ////Wyliczenie godzin oraz minut z .TotalMinutes
            hours = 0;
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
                return $"{Math.Round(total * 60, 0)}{"sek."}";
            }
        }     

        #endregion
    }
}
