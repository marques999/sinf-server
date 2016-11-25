using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class Representative : RepresentativeListing
    {
        [JsonProperty(PropertyName = "email")]
        public string Email
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

        [JsonProperty(PropertyName = "picture")]
        public string Fotografia
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

        [JsonProperty(PropertyName = "address")]
        public string Morada
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

        [JsonProperty(PropertyName = "postal")]
        public string CodigoPostal
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "company")]
        public string Empresa
        {
            get;
            set;
        }
    }
}