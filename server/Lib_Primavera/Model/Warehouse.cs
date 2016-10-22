using System;
using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    [Serializable]
    public class Warehouse
    {
        [JsonProperty(PropertyName = "id")]
        public string Armazem
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

        [JsonProperty(PropertyName = "location")]
        public Address Localizacao
        {
            get
            {
                return Localizacao;
            }
            set
            {
                if (value != null)
                {
                    Localizacao = value;
                }
            }
        }
    }
}