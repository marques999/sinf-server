using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class UserPassword
    {
        [JsonProperty(PropertyName = "old")]
        public string PasswordAntiga
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "new")]
        public string PasswordNova
        {
            get;
            set;
        }
    }
}