using System;
using System.Collections.Generic;

using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Lib_Primavera.Integration
{
    public class OpportunityIntegration
    {
        public static List<Opportunity> GetOpportunities()
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            return new List<Opportunity>();
        }

        public static Opportunity GetOpportunity(string paramId)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PriEngine.Clientes.Existe(paramId) == false)
            {
                throw new NotFoundException();
            }

            return null;
        }

        public static bool UpdateOpportunity(string paramId, Opportunity paramObject)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PriEngine.Clientes.Existe(paramId) == false)
            {
                return false;
            }

            return true;
        }

        public static bool CreateOpportunity(string paramId, Opportunity paramObject)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PriEngine.Clientes.Existe(paramId) == true)
            {
                return false;
            }

            return true;
        }
    }
}