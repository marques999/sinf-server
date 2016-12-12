using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace FirstREST.LibPrimavera.Model
{
    public class Proposals
    {
        [JsonProperty(PropertyName = "idOportunidade")]
        public string idOportunidade
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "proposalnumber")]
        public short ProposalNumber
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

        [JsonProperty(PropertyName = "totalize")]
        public bool Totalize
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "paymentmethod")]
        public string PaymentMethod
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "paymentcondition")]
        public string PaymentCondition
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "entityDiscount")]
        public double EntityDiscount
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "cost")]
        public double Cost
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "value")]
        public double Value
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

        [JsonProperty(PropertyName = "proposallines")]
        public List<ProposalsLine> ProposalsLines
        {
            get;
            set;
        }
    }
}