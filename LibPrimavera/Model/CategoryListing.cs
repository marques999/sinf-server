using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class CategoryListing
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

        [JsonProperty(PropertyName = "length")]
        public int NumeroProdutos
        {
            get;
            set;
        }
    }
}