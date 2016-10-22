using System;
using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    [Serializable]
    public class UserReference
    {
        public UserReference(string paramId, string paramNome)
        {
            Utilizador = paramId;
            Nome = paramNome;
        }

        [JsonProperty(PropertyName = "id")]
        public string Utilizador
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
    }
}