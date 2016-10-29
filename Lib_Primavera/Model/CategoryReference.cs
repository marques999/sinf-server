using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class CategoryReference
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get;
            set;
        }
    }
}