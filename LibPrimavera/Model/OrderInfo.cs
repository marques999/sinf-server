using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class OrderInfo : Order
    {
        [JsonProperty(PropertyName = "product")]
        public new Reference Produto
        {
            get;
            set;
        }
    }
}