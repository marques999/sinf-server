using System;
using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    [Serializable]
    public class Venda
    {
        [JsonProperty(PropertyName = "id")]
        public string CodArtigo
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "description")]
        public string DescArtigo
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