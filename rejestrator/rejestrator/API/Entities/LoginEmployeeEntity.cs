namespace rejestrator.API.Entities
{
    using Newtonsoft.Json;

    public class LoginEmployeeEntity
    {
        [JsonProperty("employeeID")]
        public string employeeID { get; set; }

        [JsonProperty("pin")]
        public string pin { get; set; }
    }
}
