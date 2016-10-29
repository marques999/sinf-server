using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class ProductOrder
    {
        public ProductOrder(Product paramArtigo, int paramQuantidade)
        {
            Product = paramArtigo;
            Quantity = paramQuantidade;
        }

        [JsonProperty(PropertyName = "product")]
        public Product Product
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "quantity")]
        public int Quantity
        {
            get
            {
                return Quantity;
            }
            set
            {
                if (value < 0)
                {
                    Quantity = 0;
                }
                else
                {
                    Quantity = value;
                }
            }
        }
    }
}