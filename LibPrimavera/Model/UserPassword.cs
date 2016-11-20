using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class UserPassword
    {
        [JsonProperty(PropertyName = "old")]
        public string PasswrdAntiga
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