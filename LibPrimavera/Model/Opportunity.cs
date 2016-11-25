using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class Opportunity
    {
        [JsonProperty(PropertyName = "id")]
        public string Identificador
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "entity")]
        public string Entity
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "campaign")]
        public string Campaign
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "sellscycle")]
        public string SellCycle
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateOrdered")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime DateOrdered
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateExpiration")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime ExpirationDate
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "realDateOrdered")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime RealDateOrdered
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

        [JsonProperty(PropertyName = "marginOV")]
        public float MarginOV
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "marginPercOV")]
        public float MarginPercOV
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "origin")]
        public string Origin
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "orderValueOV")]
        public float OrderValueOV
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "proposedValueOV")]
        public float ProposedValueOV
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "zone")]
        public string Zone
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "seller")]
        public string Seller
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "createdBy")]
        public string CreatedBy
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "realBillingDate")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime RealBillingDate
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "closureDate")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime ClosureDate
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "lossMotive")]
        public string LossMotive
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "opportunity")]
        public string Opportunityy
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "currency")]
        public string Currency
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "brief")]
        public string Brief
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "entityType")]
        public string EntityType
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateCreated")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime DataCriacao
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "dateModified")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime DataModificacao
        {
            get;
            set;
        }
    }
}