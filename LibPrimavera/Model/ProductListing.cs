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
        private double _iva;

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

        [JsonProperty(PropertyName = "average")]
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

        [JsonProperty(PropertyName = "units")]
        public string Unidade
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "tax")]
        public double IVA
        {
            get
            {
                return _iva;
            }
            set
            {
                if (value < 0.0)
                {
                    _iva = 0.0;
                }
                else
                {
                    _iva = value;
                }
            }
        }
    }
}