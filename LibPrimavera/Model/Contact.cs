using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class Contact : Entity
    {
        [JsonProperty(PropertyName = "title")]
        public string Titulo
        {
            get;
            set;
        }
    }
}