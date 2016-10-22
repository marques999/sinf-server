using System;
using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    [Serializable]
    public class Quote
    {
        [JsonProperty(PropertyName = "billingAddress")]
        public Address BillingAddress
        {
            get
            {
                return BillingAddress;
            }
            set
            {
                if (value != null)
                {
                    BillingAddress = value;
                }
            }
        }

        [JsonProperty(PropertyName = "shippingAddress")]
        public Address ShippingAddress
        {
            get
            {
                return ShippingAddress;
            }
            set
            {
                if (value != null)
                {
                    ShippingAddress = value;
                }
            }
        }
    }
}