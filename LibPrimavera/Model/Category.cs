using Newtonsoft.Json;
using System.Collections.Generic;

namespace FirstREST.LibPrimavera.Model
{
    public class Category
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

        [JsonProperty(PropertyName = "length")]
        public int NumberProducts
        {
            get;
            set;
        }
    }

    public class CategoryProducts
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

        [JsonProperty(PropertyName = "products")]
        public List<ProductListing> Products
        {
            get;
            set;
        }
    }

    public class CategoryReference
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