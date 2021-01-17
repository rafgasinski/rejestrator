namespace rejestrator.API.Entities
{
    using Newtonsoft.Json;

    public class LogEntity
    {
        [JsonProperty("employeeID")]
        public string employeeID { get; set; }

        [JsonProperty("date")]
        public string date { get; set; }

    }
}
