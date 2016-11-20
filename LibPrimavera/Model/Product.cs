using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class Product
    {
        /// <summary>
        /// Private access fields
        /// </summary>

        private double _discount;
        private double _price;
        private double _stock;

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

        [JsonProperty(PropertyName = "barcode")]
        public string CodigoBarras
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "units")]
        public string Unidade
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateCreated")]
        public DateTime DateCreated
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateModified")]
        public DateTime DateModified
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
        public string IVA
        {
            get;
            set;
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

        [JsonProperty(PropertyName = "discount")]
        public double Desconto
        {
            get
            {
                return _discount;
            }
            set
            {
                if (value < 0.0)
                {
                    _discount = 0.0;
                }
                else if (value > 100)
                {
                    _discount = 1.0;
                }
                else
                {
                    _discount = value * 0.01;
                }
            }
        }

        [JsonProperty(PropertyName = "warehouses")]
        public IEnumerable<Warehouse> Armazens
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "category")]
        public Reference Categoria
        {
            get;
            set;
        }
    }

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
        public double Preco
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