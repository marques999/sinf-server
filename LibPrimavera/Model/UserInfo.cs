using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class UserInfo
    {
        [JsonProperty(PropertyName = "username")]
        public string Username
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "password")]
        public string Password
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "representative")]
        public string Representante
        {
            get;
            set;
        }
    }
}