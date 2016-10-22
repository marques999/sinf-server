using System;
using Newtonsoft.Json;

namespace FirstREST.Lib_Primavera.Model
{
    [Serializable]
    public class DatabaseException : Exception
    {
        public DatabaseException(string paramModule)
        {
            Module = paramModule;
        }

        [JsonProperty(PropertyName = "message")]
        public override string Message
        {
            get
            {
                return "databaseError";
            }
        }

        [JsonProperty(PropertyName = "module")]
        public string Module
        {
            get;
            private set;
        }
    }
}