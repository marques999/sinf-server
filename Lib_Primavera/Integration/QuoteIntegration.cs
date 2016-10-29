using System;
using System.Collections.Generic;

using Interop.CrmBE900;

using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class QuoteIntegration
    {
        public static List<Quote> GetQuotes(string sessionId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            return new List<Quote>();
        }

        private static List<Quote> GetByOpportunity(string paramId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var quotesTable = PrimaveraEngine.Engine.CRM.OportunidadesVenda;

            if (quotesTable.ExisteID(paramId) == false)
            {
                return null;
            }

            var queryResult = new List<Quote>();
            var queryObject = PrimaveraEngine.Engine.CRM.OportunidadesVenda.listaDocumentos(paramId);

            while (!queryObject.NoFim())
            {
                queryResult.Add(GenerateQuote(queryObject));
                queryObject.Seguinte();
            }

            return queryResult;
        }

        private static Quote GenerateQuote(Interop.StdBE900.StdBELista queryObject)
        {
            return new Quote
            {

            };
        }
        public static Quote GetQuote(string sessionId, string paramId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var quotesTable = PrimaveraEngine.Engine.Comercial.Vendas;

            if (quotesTable.ExisteID(paramId) == false)
            {
                return null;
            }

            return null;
        }

        private static void SetFields(CrmBEPropostaOPV selectedRow, Quote paramObject)
        {
            if (paramObject.Notes != null)
            {
                selectedRow.set_Observacoes(paramObject.Notes.Trim());
            }

            if (paramObject.Description != null)
            {
                selectedRow.set_Descricao(paramObject.Description.Trim());
            }
        }

        public static bool Update(string sessionId, Quote paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var selectedId = paramObject.OpportunityId;

            if (PrimaveraEngine.Engine.Comercial.Vendas.ExisteID(selectedId) == false)
            {
                return false;
            }

            CrmBEPropostaOPV proposta = new CrmBEPropostaOPV();

            var selectedOpportunity = PrimaveraEngine.Engine.CRM.OportunidadesVenda.EditaPropostasOPV(paramObject.OpportunityId);
            var selectedRow = selectedOpportunity.get_Edita(paramObject.Identifier);

            selectedRow.set_EmModoEdicao(true);
            SetFields(selectedRow, paramObject);
            selectedRow.set_EmModoEdicao(false);
            selectedOpportunity.set_Edita(paramObject.Identifier, selectedRow);

            return true;
        }

        public static bool Insert(string sessionId, Quote paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.CRM.OportunidadesVenda.Existe(paramObject.OpportunityId) == false)
            {
                return false;
            }

            var selectedOpportunity = PrimaveraEngine.Engine.CRM.OportunidadesVenda.EditaPropostasOPV(paramObject.OpportunityId);

            if (selectedOpportunity.NumItens < paramObject.Identifier)
            {
                return false;
            }

            var selectedRow = new CrmBEPropostaOPV();

            SetFields(selectedRow, paramObject);
            selectedOpportunity.Insere(selectedRow);
            //PriEngine.Engine.CRM.OportunidadesVenda.Actualiza(selectedOpportunity);

            return true;
        }

        public static bool Delete(string sessionId, short paramId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            System.Diagnostics.Debug.Print("TESTING DELETE METHOD!");

            return true;
        }
    }
}