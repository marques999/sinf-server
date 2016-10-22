using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    [Serializable]
    public class Product
    {
        [JsonProperty(PropertyName = "id")]
        public string Artigo
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "name")]
        public string Nome
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "cost")]
        public double Cost
        {
            get
            {
                return Cost;
            }
            set
            {
                if (value < 0.0)
                {
                    Cost = 0.0;
                }
                else
                {
                    Cost = value;
                }
            }
        }

        [JsonProperty(PropertyName = "price")]
        public double Price
        {
            get
            {
                return Price;
            }
            set
            {
                if (value < 0.0)
                {
                    Price = 0.0;
                }
                else
                {
                    Price = value;
                }
            }
        }

        [JsonProperty(PropertyName = "tax")]
        public double Tax
        {
            get
            {
                return Tax;
            }
            set
            {
                if (value < 0.0)
                {
                    Tax = 0.0;
                }
                else if (value > 1.0)
                {
                    Tax = 1.0;
                }
                else
                {
                    Tax = value;
                }
            }
        }

        [JsonProperty(PropertyName = "discountValue")]
        public double DiscountValue
        {
            get
            {
                return DiscountValue;
            }
            set
            {
                if (value < 0.0)
                {
                    DiscountValue = 0.0;
                }
                else if (value > 1.0)
                {
                    DiscountValue = 1.0;
                }
                else
                {
                    DiscountValue = value;
                }
            }
        }

        [JsonProperty(PropertyName = "discountEnabled")]
        public bool DiscountEnabled
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "warehouses")]
        public IEnumerable<Warehouse> Warehouses
        {
            get;
            set;
        }
    }
}