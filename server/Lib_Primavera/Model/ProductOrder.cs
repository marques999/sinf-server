using System;
using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    [Serializable]
    public class ProductOrder
    {
        public ProductOrder(Product paramArtigo, int paramQuantidade)
        {
            Artigo = paramArtigo;
            Quantidade = paramQuantidade;
        }

        [JsonProperty(PropertyName = "product")]
        public Product Artigo
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "quantity")]
        public int Quantidade
        {
            get
            {
                return Quantidade;
            }
            set
            {
                if (value < 0)
                {
                    Quantidade = 0;
                }
                else
                {
                    Quantidade = value;
                }
            }
        }
    }
}