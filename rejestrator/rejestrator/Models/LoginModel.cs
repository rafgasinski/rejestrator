namespace rejestrator.Models
{
    using MySql.Data.MySqlClient;
    using rejestrator.API;
    using rejestrator.Database;
    using System.Collections.Generic;
    using System;
    using rejestrator.API.Entities;
    using Newtonsoft.Json;
    using Utils;
    using Converters;
    using System.Net.Http;

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
            var loginEmployee = new LoginEmployeeEntity
            {
                employeeID = id,
                pin = pin
            };

            string response = APIService.makeRequest(HTTPMethod.POST, "loginEmployee", loginEmployee.ToKeyValueURL());

            if (Error.IsResponseError(response))
                return false;

            EmployeeEntity employee = JsonConvert.DeserializeObject<EmployeeEntity>(response);

            EmployeeModel.Instance.ID = employee.employeeID;
            EmployeeModel.Instance.pin = employee.pin;
            EmployeeModel.Instance.Name = $"{employee.name} {employee.surname}";
            EmployeeModel.Instance.Shift = employee.shift;
            return true;
        }

        public bool LoginAdmin(string username, string password)
        {
            var loginAdmin = new LoginAdminEntity
            {
                username = username,
                password = password
            };

            string response = APIService.makeRequest(HTTPMethod.POST, "loginAdmin", loginAdmin.ToKeyValueURL());

            if (Error.IsResponseError(response))
                return false;
            return true;
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

        public void InsertLoginDate(string id, string date)
        {
            var log = new LogEntity
            {
                employeeID = id,
                date = date
            };

            string response = APIService.makeRequest(HTTPMethod.POST, "logs", log.ToKeyValueURL());

            if (Error.IsResponseError(response))
                throw new NotImplementedException();

/*            string query = @"INSERT INTO `logs`(`employeeID`, `date`) VALUES(@id,@date)";
            using (MySqlCommand myCommand = new MySqlCommand(query, Database.DBConnection()))
            {
                Database.OpenConnection();
                myCommand.Parameters.AddWithValue("@id", id);
                myCommand.Parameters.AddWithValue("@date", date);
                myCommand.CommandText = query;
                myCommand.ExecuteNonQuery();
                Database.CloseConnection();
            }*/
        }
    }
}
