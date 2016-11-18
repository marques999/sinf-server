using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class UserLogin
    {
        [JsonProperty(PropertyName = "username")]
        public string Username
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "password")]
        public string Password
        {
            get;
            set;
        }
    }

    public class UserPassword
    {
        [JsonProperty(PropertyName = "old")]
        public string oldPassword
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "new")]
        public string newPassword
        {
            get;
            set;
        }
    }

    public class UserData
    {
        [JsonProperty(PropertyName = "username")]
        public string Username
        {
            get;
            set;
        }

        public Representative Representative
        {
            get;
            set;
        }
    }
    
    public class UserForm
    {
        [JsonProperty(PropertyName = "username")]
        public string Username
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "password")]
        public string Password
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "representative")]
        public string Representative
        {
            get;
            set;
        }
    }
}