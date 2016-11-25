using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class OrderInfo : Order
    {
        [JsonProperty(PropertyName = "products")]
        public new Reference Produto
        {
            get;
            set;
        }
    }
}