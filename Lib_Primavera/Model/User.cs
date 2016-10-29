using System;
using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class User
    {
        [JsonProperty(PropertyName = "username")]
        private string Username
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "name")]
        private string Name
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "email")]
        private string Email
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "location")]
        private Address Location
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
    }

    public class UserReference
    {
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