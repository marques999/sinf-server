using System;

namespace FirstREST.Lib_Primavera.Model
{
    public class Account
    {
        public string ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public Address Address
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string Website
        {
            get;
            set;
        }

        public string Phone
        {
            get;
            set;
        }

        public string TaxNumber
        {
            get;
            set;
        }

        public string Currency
        {
            get;
            set;
        }

        public DateTime LastUpdated
        {
            get;
            set;
        }

        public DateTime CreatedAt
        {
            get;
            set;
        }

        public UserReference Owner
        {
            get;
            set;
        }
    }
}