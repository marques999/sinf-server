using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class Contact : Entity
    {
        [JsonProperty(PropertyName = "email")]
        public string Email
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "title")]
        public string Titulo
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "entity")]
        public string Cliente
        {
            get;
            set;
        }
    }
}