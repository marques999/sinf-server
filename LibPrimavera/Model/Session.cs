using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class Session
    {
        public Session()
        {
        }

        public Session(string userName, string representativeId)
        {
            Username = userName;
            Identificador = representativeId;
        }

        [JsonProperty(PropertyName = "id")]
        public string Identificador
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "username")]
        public string Username
        {
            get;
            set;
        }
    }
}