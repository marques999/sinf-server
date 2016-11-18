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

        public static Opportunity View(string sessionId, string opportunityId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var opportunitiesTable = PrimaveraEngine.Engine.CRM.OportunidadesVenda;

            if (opportunitiesTable.Existe(opportunityId) == false)
            {
                return null;
            }

            var opportunityInfo = opportunitiesTable.Edita(opportunityId);

            if (opportunityInfo.get_Vendedor() != sessionId)
            {
                return null;
            }

            return new Opportunity();
        }

        private static void SetFields(CrmBEOportunidadeVenda opportunityInfo, Opportunity jsonObject)
        {
        }

        public static bool Update(string sessionId, string opportunityId, Opportunity jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var opportunitiesTable = PrimaveraEngine.Engine.CRM.OportunidadesVenda;

            if (opportunitiesTable.Existe(opportunityId) == false)
            {
                return false;
            }

            var opportunityInfo = opportunitiesTable.Edita(opportunityId);

            if (opportunityInfo.get_Vendedor() != sessionId)
            {
                return false;
            }

            jsonObject.DateModified = DateTime.Now;
            opportunityInfo.set_EmModoEdicao(true);
            SetFields(opportunityInfo, jsonObject);
            opportunitiesTable.Actualiza(opportunityInfo);

            return true;
        }

        public static bool Insert(string sessionId, Opportunity jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var opportunityId = new HashGenerator().EncodeLong(DateTime.Now.Ticks);
            var opportunitiesTable = PrimaveraEngine.Engine.CRM.OportunidadesVenda;

            if (opportunitiesTable.Existe(opportunityId))
            {
                return false;
            }

            var opportunityInfo = new CrmBEOportunidadeVenda();

            jsonObject.DateCreated = DateTime.Now;
            jsonObject.DateModified = jsonObject.DateCreated;
            opportunityInfo.set_ID(opportunityId);
            opportunitiesTable.Actualiza(opportunityInfo);

            return true;
        }

        public static bool Delete(string sessionId, string opportunityId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var opportunitiesTable = PrimaveraEngine.Engine.CRM.OportunidadesVenda;

            if (opportunitiesTable.Existe(opportunityId) == false)
            {
                return false;
            }

            if (opportunitiesTable.Edita(opportunityId).get_Vendedor() != sessionId)
            {
                return false;
            }

            opportunitiesTable.Remove(opportunityId);

            return true;
        }
    }
}