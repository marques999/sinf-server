using System;
using System.Collections.Generic;

using Interop.ICrmBS900;
using Interop.CrmBE900;

using FirstREST.LibPrimavera.Model;

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

            var opportunityInfo = new CrmBEOportunidadeVenda();
            var opportunityId = PrimaveraEngine.GenerateHash();
            var opportunitiesTable = PrimaveraEngine.Engine.CRM.OportunidadesVenda;

            if (opportunitiesTable.Existe(opportunityId))
            {
                return false;
            }

            opportunityInfo.set_Oportunidade(opportunityId);
            opportunityInfo.set_CriadoPor(sessionId);
            opportunityInfo.set_DataCriacao(DateTime.Now);
            opportunityInfo.set_Oportunidade(opportunityId);
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

            var opportunityInfo = opportunitiesTable.Edita(opportunityId);

            if (opportunityInfo.get_Vendedor() != sessionId)
            {
                return false;
            }

            opportunityInfo.set_EmModoEdicao(true);
            opportunityInfo.set_EstadoVenda(-1);
            opportunitiesTable.Actualiza(opportunityInfo);

            return true;
        }
    }
}