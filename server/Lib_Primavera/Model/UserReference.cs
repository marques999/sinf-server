using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class UserReference
    {
        [JsonProperty(PropertyName = "userId")]
        public string Identifier
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "userName")]
        public string Name
        {
            get;
            set;
        }
    }
}