using System;
using System.Collections.Generic;

using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Lib_Primavera.Integration
{
    public class QuoteIntegration
    {
        public static List<Quote> GetQuotes()
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            return new List<Quote>();
        }

        public static Quote GetQuote(string paramId)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var quotesTable = PriEngine.Engine.Comercial.Vendas;

            if (quotesTable.ExisteID(paramId) == false)
            {
                return null;
            }

            return null;
        }

        public static bool UpdateQuote(string paramId, Quote paramObject)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PriEngine.Engine.Comercial.Vendas.ExisteID(paramId) == false)
            {
                return false;
            }

            return true;
        }

        public static bool CreateQuote(string paramId, Quote paramObject)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PriEngine.Engine.Comercial.Vendas.ExisteID(paramId))
            {
                return false;
            }

            return true;
        }
    }
}