namespace rejestrator.API.Entities
{
    using Newtonsoft.Json;

    public class TaskDoneEntity
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("employeeID")]
        public string employeeID { get; set; }

        [JsonProperty("task")]
        public string task { get; set; }

        [JsonProperty("startdate")]
        public string startdate { get; set; }

        [JsonProperty("enddate")]
        public string enddate { get; set; }

        [JsonProperty("time")]
        public string time { get; set; }

    }
}
