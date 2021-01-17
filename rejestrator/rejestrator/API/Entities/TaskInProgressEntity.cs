namespace rejestrator.API.Entities
{
    using Newtonsoft.Json;

    public class TaskInProgressEntity
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("employeeID")]
        public string employeeID { get; set; }

        [JsonProperty("task")]
        public string task { get; set; }

        [JsonProperty("date")]
        public string date { get; set; }
    }
}
