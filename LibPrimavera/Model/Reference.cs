﻿using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class Reference
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get;
            set;
        }
    }
}