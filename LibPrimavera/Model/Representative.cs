using System;
using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class Representative
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

        [JsonProperty(PropertyName = "email")]
        public string Email
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "phone")]
        public string Mobile
        {
            get;
            set;
        }


        [JsonProperty(PropertyName = "picture")]
        public string Picture
        {
            get;
            set;
        }

        public void setPicture(byte[] pictureData)
        {
            Picture = Convert.ToBase64String(pictureData);
        }

        public string Phone
        {
            get;
            set;
        }
    }
}