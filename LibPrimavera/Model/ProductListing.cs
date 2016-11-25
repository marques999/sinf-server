using System;
using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class ProductListing
    {
        /// <summary>
        /// Private access fields
        /// </summary>

        private double _price;
        private double _stock;
        private double _tax;

        /// <summary>
        /// Public access properties
        /// </summary>

        [JsonProperty(PropertyName = "id")]
        public string Identificador
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "name")]
        public string Descricao
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "price")]
        public double PrecoMedio
        {
            get
            {
                return _price;
            }
            set
            {
                if (value < 0.0)
                {
                    _price = 0.0;
                }
                else
                {
                    _price = value;
                }
            }
        }

        [JsonProperty(PropertyName = "tax")]
        public double IVA
        {
            get
            {
                return _tax;
            }
            set
            {
                if (value < 0)
                {
                    _tax = 0.0;
                }
                else if (value > 100)
                {
                    _tax = 1.0;
                }
                else
                {
                    _tax = value * 0.01;
                }
            }
        }

        [JsonProperty(PropertyName = "stock")]
        public double Stock
        {
            get
            {
                return _stock;
            }
            set
            {
                if (value < 0.0)
                {
                    _stock = 0.0;
                }
                else
                {
                    _stock = value;
                }
            }
        }

        [JsonProperty(PropertyName = "category")]
        public Reference Categoria
        {
            get;
            set;
        }
    }
}