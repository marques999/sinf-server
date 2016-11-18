using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class UserPassword
    {
        [JsonProperty(PropertyName = "old")]
        public string oldPassword
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "new")]
        public string newPassword
        {
            get;
            set;
        }
    }
}