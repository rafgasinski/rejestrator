namespace rejestrator.API.Entities
{
    using Newtonsoft.Json;

    public class LoginAdminEntity
    {
        [JsonProperty("username")]
        public string username { get; set; }

        [JsonProperty("password")]
        public string password { get; set; }
    }
}
