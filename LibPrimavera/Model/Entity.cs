using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class Entity
    {
        [JsonProperty(PropertyName = "name")]
        public string Nome
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "location")]
        public Address Localizacao
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "phone")]
        public string Telefone
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "phone2")]
        public string Telefone2
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "mobile")]
        public string Telemovel
        {
            get;
            set;
        }
    }
}