using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class LeadListing : EntityListing
    {
        [JsonProperty(PropertyName = "email")]
        public string Email
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "active")]
        public bool Activo
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
    }
}