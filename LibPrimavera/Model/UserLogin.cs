using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class UserLogin
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
    }
}