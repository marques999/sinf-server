using System;
using System.Collections.Generic;

using FirstREST.LibPrimavera.Model;
using Interop.ICrmBS900;
using Interop.CrmBE900;

namespace FirstREST.LibPrimavera.Integration
{
    public class OpportunityIntegration
    {
        public static List<Opportunity> List(string sessionId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            return new List<Opportunity>();
        }

        public static Opportunity View(string sessionId, string paramId)
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

        public static bool Update(string sessionId, string opportunityId, Opportunity paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var opportunityTable = PrimaveraEngine.Engine.CRM.OportunidadesVenda;

            if (opportunityTable.Existe(opportunityId) == false)
            {
                return false;
            }

            var errorMessages = "";
            var opportunityRow = opportunityTable.Edita(opportunityId);

            if (opportunityRow.get_Vendedor() != sessionId)
            {
                return false;
            }

            opportunityRow.set_EmModoEdicao(true);
            opportunityTable.Actualiza(opportunityRow, errorMessages);
            System.Diagnostics.Debug.Print(errorMessages);

            return true;
        }

        public static bool Insert(string sessionId, Opportunity paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var errorMessages = "";
            var opportunityId = paramObject.Identifier;
            var opportunityTable = PrimaveraEngine.Engine.CRM.OportunidadesVenda;

            if (opportunityTable.Existe(opportunityId))
            {
                return false;
            }

            var opportunityRow = new CrmBEOportunidadeVenda();

            opportunityRow.set_ID(opportunityId);
            opportunityTable.Actualiza(opportunityRow, ref errorMessages);
            System.Diagnostics.Debug.Print(errorMessages);

            return true;
        }

        public static bool Delete(string sessionId, string opportunityId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var opportunityTable = PrimaveraEngine.Engine.CRM.OportunidadesVenda;

            if (opportunityTable.Existe(opportunityId) == false)
            {
                return false;
            }

            if (opportunityTable.Edita(opportunityId).get_Vendedor() != sessionId)
            {
                return false;
            }

            opportunityTable.Remove(opportunityId);

            return true;
        }
    }
}