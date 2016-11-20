using System;
using System.Collections.Generic;

using Interop.GcpBE900;
using Interop.StdBE900;

using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class QuoteIntegration
    {
        public static List<Quote> List(string quoteId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var quotesTable = PrimaveraEngine.Engine.CRM.OportunidadesVenda;

            if (quotesTable.ExisteID(quoteId) == false)
            {
                return null;
            }

            var queryResult = new List<Quote>();
            var queryObject = PrimaveraEngine.Engine.CRM.OportunidadesVenda.listaDocumentos(quoteId);

            while (!queryObject.NoFim())
            {
                queryResult.Add(new Quote());
                queryObject.Seguinte();
            }

            return queryResult;
        }

        public static QuoteInfo View(string sessionId, string quoteId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var quotesTable = PrimaveraEngine.Engine.Comercial.Vendas;

            if (quotesTable.ExisteID(quoteId) == false)
            {
                return null;
            }

            var quoteInfo = quotesTable.EditaID(quoteId);

            if (quoteInfo.get_Responsavel() != sessionId)
            {
                return null;
            }

            return new QuoteInfo
            {
                Identificador = quoteInfo.get_ID(),
                Descricao = quoteInfo.get_Nome(),
                Notas = quoteInfo.get_Observacoes(),
                OpportunityId = quoteInfo.get_IdOportunidade()
            };
        }

        private static void SetFields(GcpBEDocumentoVenda quoteInfo, Quote jsonObject)
        {
            if (jsonObject.Descricao != null)
                quoteInfo.set_Nome(jsonObject.Descricao);
            if (jsonObject.Notas != null)
                quoteInfo.set_Observacoes(jsonObject.Notas);
            if (jsonObject.OpportunityId != null)
                quoteInfo.set_IdOportunidade(jsonObject.OpportunityId);
        }

        public static bool Update(string sessionId, string quoteId, Quote jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var quotesTable = PrimaveraEngine.Engine.Comercial.Vendas;

            if (quotesTable.ExisteID(quoteId) == false)
            {
                return false;
            }

            var quoteInfo = quotesTable.EditaID(quoteId);

            if (quoteInfo.get_Responsavel() != sessionId)
            {
                return false;
            }

            quoteInfo.set_EmModoEdicao(true);
            quoteInfo.set_DataUltimaActualizacao(DateTime.Now);
            SetFields(quoteInfo, jsonObject);
            quotesTable.Actualiza(quoteInfo);

            return true;
        }

        public static bool Insert(string sessionId, Quote jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var quoteInfo = new GcpBEDocumentoVenda();
            var quoteId = new HashGenerator().EncodeLong(DateTime.Now.Ticks);
            var quotesTable = PrimaveraEngine.Engine.Comercial.Vendas;

            if (quotesTable.ExisteID(quoteId))
            {
                return false;
            }

            var serverTime = DateTime.Now;

            quoteInfo.set_ID(quoteId);
            quoteInfo.set_Responsavel(sessionId);
            quoteInfo.set_DataDoc(serverTime);
            quoteInfo.set_DataUltimaActualizacao(serverTime);
            SetFields(quoteInfo, jsonObject);
            quoteInfo = quotesTable.PreencheDadosRelacionados(quoteInfo);
            quotesTable.Actualiza(quoteInfo);

            return true;
        }

        public static bool Delete(string sessionId, string quoteId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var quotesTable = PrimaveraEngine.Engine.Comercial.Vendas;

            if (quotesTable.ExisteID(quoteId) == false)
            {
                return false;
            }

            var quoteInfo = quotesTable.EditaID(quoteId);

            if (quoteInfo.get_Responsavel() != sessionId)
            {
                return false;
            }

            quoteInfo.set_EmModoEdicao(true);
            quoteInfo.set_Anulado(true);
            quotesTable.Actualiza(quoteInfo);

            return true;
        }
    }
}