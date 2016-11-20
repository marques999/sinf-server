using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class CustomerListing : EntityListing
    {
        [JsonProperty(PropertyName = "status")]
        public string Estado
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "debt")]
        public double Debito
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "pending")]
        public double Pendentes
        {
            get;
            set;
        }
    }
}