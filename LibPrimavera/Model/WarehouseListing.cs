using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class WarehouseListing
    {
        [JsonProperty(PropertyName = "id")]
        public string Identificador
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "name")]
        public string Descricao
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "location")]
        public Address Localizacao
        {
            get;
            set;
        }
    }
}