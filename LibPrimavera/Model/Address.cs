using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class Address
    {
        [JsonProperty(PropertyName = "address")]
        public string Morada
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "postal")]
        public string CodigoPostal
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "parish")]
        public string Localidade
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "state")]
        public string Distrito
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "country")]
        public string Pais
        {
            get;
            set;
        }
    }
}