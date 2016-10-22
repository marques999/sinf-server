using System;
using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    [Serializable]
    public class UserReference
    {
        public UserReference(string paramId, string paramNome)
        {
            Identifier = paramId;
            Name = paramNome;
        }

        [JsonProperty(PropertyName = "userId")]
        public string Identifier
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "userName")]
        public string Name
        {
            get;
            set;
        }
    }
}