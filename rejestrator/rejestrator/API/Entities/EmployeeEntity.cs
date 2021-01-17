namespace rejestrator.API.Entities
{
    using Newtonsoft.Json;

    public class EmployeeEntity
    {
        [JsonProperty("employeeID")]
        public string employeeID { get; set; }

        [JsonProperty("pin")]
        public string pin { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("surname")]
        public string surname { get; set; }

        [JsonProperty("shift")]
        public string shift { get; set; }
    }
}
