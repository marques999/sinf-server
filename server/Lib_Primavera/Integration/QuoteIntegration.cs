using System;
using System.Collections.Generic;

using Interop.GcpBE900;

using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Lib_Primavera.Integration
{
    public class QuoteIntegration
    {
        public static List<Quote> GetQuotes()
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                return null;
            }

            return new List<Quote>();
        }

        public static Quote GetQuote(string paramId)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                return null;
            }

            if (PriEngine.Clientes.Existe(paramId) == false)
            {
                return null;
            }

            return null;
        }

        public static bool UpdateQuote(string paramId, Quote paramObject)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseException("LeadIntegration");
            }

            if (PriEngine.Clientes.Existe(paramId) == false)
            {
                return false;
            }

            return true;
        }

        public static bool CreateQuote(string paramId, Quote paramObject)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseException("LeadIntegration");
            }

            return true;
        }
    }
}