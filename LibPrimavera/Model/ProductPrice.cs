using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class ProductPrice
    {
        public ProductPrice()
        {
        }

        public ProductPrice(bool productIVA, double productPrice)
        {
            IVA = productIVA;
            Preco = productPrice;
        }

        [JsonProperty(PropertyName = "price")]
        public double Preco
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "tax")]
        public bool IVA
        {
            get;
            set;
        }
    }
}