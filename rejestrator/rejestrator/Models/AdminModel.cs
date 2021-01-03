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
    }
}
