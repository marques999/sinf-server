using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class SessionInfo : Representative
    {
        [JsonProperty(PropertyName = "token")]
        public string Token
        {
            get;
            set;
        }
    }
}