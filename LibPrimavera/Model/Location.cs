using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class Location
    {
        [JsonProperty(PropertyName = "id")]
        public int Identifier
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