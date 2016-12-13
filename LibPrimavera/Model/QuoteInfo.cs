using System.Collections.Generic;

using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class QuoteInfo : Quote
    {
        private Address _shippingAddress;
        
        [JsonProperty(PropertyName = "products")]
        public List<OrderInfo> Produtos
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "shippingAddress")]
        public Address EnderecoEntrega
        {
            get
            {
                return _shippingAddress;
            }
            set
            {
                if (value != null)
                {
                    _shippingAddress = value;
                }
            }
        }

        [JsonProperty(PropertyName = "totalMerc")]
        public double TotalMerc
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "totalIva")]
        public double TotalIva
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "totalDesc")]
        public double TotalDesc
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "opportunityId")]
        public string IdOportunidade
        {
            get;
            set;
        }
    }
}