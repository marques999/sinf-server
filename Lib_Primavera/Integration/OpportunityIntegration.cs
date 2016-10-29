using System;
using System.Collections.Generic;

using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class OpportunityIntegration
    {
        public static List<Opportunity> GetOpportunities(string sessionId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            return new List<Opportunity>();
        }

        public static Opportunity GetOpportunity(string sessionId, string paramId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.CRM.OportunidadesVenda.Existe(paramId) == false)
            {
                return null;
            }

            return null;
        }

        public static bool Update(string sessionId, Opportunity paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var selectedId = paramObject.Identifier;

            if (PrimaveraEngine.Engine.CRM.OportunidadesVenda.Existe(selectedId) == false)
            {
                return false;
            }

            return true;
        }

        public static bool Insert(string sessionId, Opportunity paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var selectedId = paramObject.Identifier;

            if (PrimaveraEngine.Engine.CRM.OportunidadesVenda.Existe(selectedId))
            {
                return false;
            }

            return true;
        }

        public static bool Delete(string sessionUsername, string paramId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.CRM.OportunidadesVenda.Existe(paramId) == false)
            {
                return false;
            }

            return true;
        }
    }
}