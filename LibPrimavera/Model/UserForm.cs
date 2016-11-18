using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class UserForm
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
        public string Representative
        {
            get;
            set;
        }
    }
}