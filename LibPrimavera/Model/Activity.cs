using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class Activity
    {
        /// <summary>
        /// Private access fields
        /// </summary>

        private int _priority;

        /// <summary>
        /// Public access properties
        /// </summary>

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

        [JsonProperty(PropertyName = "description")]
        public string Description
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "status")]
        public ActivityStatus Status
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "type")]
        public Reference Type
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "entity")]
        public Reference Entity
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "priority")]
        public int Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                if (value < 0)
                {
                    _priority = 0;
                }
                else if (value > 5)
                {
                    _priority = 5;
                }
                else
                {
                    _priority = value;
                }
            }
        }

        [JsonProperty(PropertyName = "duration")]
        public int Duration
        {
            get
            {
                return (int)((End - Start).TotalMinutes);
            }
        }

        [JsonProperty(PropertyName = "duration")]
        public string Owner
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "start")]
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime Start
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "end")]
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime End
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateCreated")]
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime DateCreated
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateModified")]
        [JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime DateModified
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "entityType")]
        public int EntityType
        {
            get;
            set;
        }
    }
}