using System;
using System.Collections.Generic;

using Interop.GcpBE900;
using Interop.StdBE900;

using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class QuoteIntegration
    {
        public static List<Quote> List(string paramId)
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

        private static Quote GenerateQuote(StdBELista queryObject)
        {
            return new Quote
            {

            };
        }

        public static Quote View(string sessionId, string paramId)
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

        private static void SetFields(GcpBEDocumentoVenda selectedRow, Quote paramObject)
        {
            if (paramObject.Notes != null)
            {
                selectedRow.set_Observacoes(paramObject.Notes.Trim());
            }

            if (paramObject.Description != null)
            {
                selectedRow.set_Nome(paramObject.Description.Trim());
            }
        }

        public static bool Update(string sessionId, string quoteId, Quote paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var quoteTable = PrimaveraEngine.Engine.Comercial.Vendas;

            if (quoteTable.ExisteID(quoteId) == false)
            {
                return false;
            }

            var errorMessages = "";
            var quoteRow = quoteTable.EditaID(quoteId);

            if (quoteRow.get_Responsavel() != sessionId)
            {
                return false;
            }

            quoteRow.set_EmModoEdicao(true);
            SetFields(quoteRow, paramObject);
            quoteTable.Actualiza(quoteRow, ref errorMessages);
            System.Diagnostics.Debug.Print(errorMessages);

            return true;
        }

        public static bool Insert(string sessionId, Quote paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var quoteTable = PrimaveraEngine.Engine.Comercial.Vendas;

            if (quoteTable.ExisteID(paramObject.Identifier))
            {
                return false;
            }

            var errorMessages = "";
            var quoteRow = new GcpBEDocumentoVenda();

            SetFields(quoteRow, paramObject);
            quoteTable.Actualiza(quoteRow, errorMessages);
            System.Diagnostics.Debug.Print(errorMessages);

            return true;
        }

        public static bool Delete(string sessionId, string quoteId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var quoteTable = PrimaveraEngine.Engine.Comercial.Vendas;

            if (quoteTable.ExisteID(quoteId) == false)
            {
                return false;
            }

            //quoteTable.Remove(quoteId);

            return true;
        }
    }
}