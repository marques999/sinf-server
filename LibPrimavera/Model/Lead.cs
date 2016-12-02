using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class Lead : Entity
    {
        [JsonProperty(PropertyName = "Address2")]
        public string Morada2
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "thirdPartyType")]
        public string TipoTerceiro
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "email")]
        public string Email
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "WebAddress")]
        public string EnderecoWeb
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "Zone")]
        public string Zona
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "NumCont")]
        public string NumContribuinte
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "Idiom")]
        public string Idioma
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "Singular")]
        public bool PessoaSingular
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "MarketType")]
        public string TipoMercado
        {
            get;
            set;
        }
    }
}