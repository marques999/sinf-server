using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class LeadListing : EntityListing
    {
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