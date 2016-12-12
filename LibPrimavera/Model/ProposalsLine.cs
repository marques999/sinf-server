using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FirstREST.LibPrimavera.Model
{
    public class ProposalsLine
    {
        [JsonProperty(PropertyName = "idOportunidade")]
        public string idOportunidade
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "proposalNumber")]
        public short ProposalNumber
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "line")]
        public short Line
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "article")]
        public string Article
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

        [JsonProperty(PropertyName = "quantity")]
        public double Quantity
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "unit")]
        public string Unit
        {
            get;
            set;
        }


        [JsonProperty(PropertyName = "factorConv")]
        public double FactorConv
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "costPrice")]
        public double CostPrice
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "sellsPrice")]
        public double SellsPrice
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "discount1")]
        public double Discount1
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "discount2")]
        public double Discount2
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "discount3")]
        public double Discount3
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "discount")]
        public double Discount
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "discountValue")]
        public double DiscountValue
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "rentability")]
        public double Rentability
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "margin")]
        public double Margin
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "observations")]
        public string Observations
        {
            get;
            set;
        }
    }
}