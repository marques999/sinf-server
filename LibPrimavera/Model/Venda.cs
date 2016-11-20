using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class Venda
    {
        [JsonProperty(PropertyName = "id")]
        public string CodigoArtigo
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "description")]
        public string DescricaoArtigo
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "cabecalho")]
        public string IdCabecDoc
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "quantity")]
        public double Quantidade
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "discount")]
        public double Desconto
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "unit")]
        public double PrecoUnitario
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "total")]
        public double TotalLiquido
        {
            get;
            set;
        }
    }
}