using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class LeadListing : ContactListing
    {
        [JsonProperty(PropertyName = "active")]
        public bool Active
        {
            get;
            set;
        }
    }
}