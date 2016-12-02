using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class ContactListing : EntityListing
    {
        [JsonProperty(PropertyName = "email")]
        public string Email
        {
            get;
            set;
        }
    }
}