using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class ProductOrder
    {
        public ProductOrder()
        {
        }

        public ProductOrder(Product paramArtigo, int paramQuantidade)
        {
            Produto = paramArtigo;
            Quantidade = paramQuantidade;
        }

        [JsonProperty(PropertyName = "product")]
        public Product Produto
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