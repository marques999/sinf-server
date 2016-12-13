using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class Order
    {
        /// <summary>
        /// Private access fields
        /// </summary>

        private int _quantidade;
        private double _iliquido;
        private double _desconto;
        private double _preco;
        private double _iva;

        /// <summary>
        /// Public access properties
        /// </summary>

        [JsonProperty(PropertyName = "product")]
        public string Produto
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "price")]
        public double Preco
        {
            get
            {
                return _preco;
            }
            set
            {
                if (value < 0.0)
                {
                    _preco = 0.0;
                }
                else
                {
                    _preco = value;
                }
            }
        }

        [JsonProperty(PropertyName = "discount")]
        public double Desconto
        {
            get
            {
                return _desconto;
            }
            set
            {
                if (value < 0.0)
                {
                    _desconto = 0.0;
                }
                else if (value > 1.0)
                {
                    _desconto = 1.0;
                }
                else
                {
                    _desconto = value;
                }
            }
        }

        [JsonProperty(PropertyName = "quantity")]
        public int Quantidade
        {
            get
            {
                return _quantidade;
            }
            set
            {
                if (value < 0)
                {
                    _quantidade = 0;
                }
                else
                {
                    _quantidade = value;
                }
            }
        }

        [JsonProperty(PropertyName = "tax")]
        public double Iva
        {
            get
            {
                return _iva;
            }
            set
            {
                if (value < 0)
                {
                    _iva = 0;
                }
                else
                {
                    _iva = value;
                }
            }
        }
    }
}