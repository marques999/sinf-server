using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class Category
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

        [JsonProperty(PropertyName = "length")]
        public int NumberProducts
        {
            get;
            set;
        }
    }
}