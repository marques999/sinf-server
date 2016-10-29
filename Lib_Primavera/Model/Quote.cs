using Newtonsoft.Json;
using System.Collections.Generic;

namespace FirstREST.LibPrimavera.Model
{
    public class Quote
    {
        [JsonProperty(PropertyName = "id")]
        public short Identifier
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "description")]
        public string Description
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "opportunity")]
        public string OpportunityId
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "products")]
        public List<ProductOrder> Products
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "notes")]
        public string Notes
        {
            get;
            set;
        }

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