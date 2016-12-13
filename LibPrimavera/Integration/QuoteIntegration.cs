using System;
using System.Collections.Generic;

using Interop.GcpBE900;
using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;

using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class QuoteIntegration
    {
        private static SqlColumn[] sqlProductsColumns =
        {
            new SqlColumn("LinhasDoc.Artigo", null),
            new SqlColumn("LinhasDoc.Descricao", null),
            new SqlColumn("LinhasDoc.Quantidade", null),
            new SqlColumn("LinhasDoc.PrecUnit", null),
            new SqlColumn("LinhasDoc.TaxaIva", null),
            new SqlColumn("LinhasDoc.Desconto1", null)
        };

        private static SqlColumn[] sqlQuoteColumns =
        {
            new SqlColumn("CabecDoc.Id", null),
            new SqlColumn("CabecDoc.TipoDoc", null),
            new SqlColumn("CabecDoc.NumDoc", null),
            new SqlColumn("CabecDoc.Responsavel", null),
            new SqlColumn("CabecDoc.Entidade", null),
            new SqlColumn("CabecDoc.Nome", null),
            new SqlColumn("CabecDoc.Data", null),
            new SqlColumn("CabecDoc.IdOportunidade", null),
            new SqlColumn("CabecDoc.TotalDocumento", null),
            new SqlColumn("CabecDoc.TotalIva", null),
            new SqlColumn("CabecDoc.TotalDesc", null),
            new SqlColumn("CabecDoc.TotalMerc", null),
            new SqlColumn("CabecDoc.Morada", null),
            new SqlColumn("CabecDoc.CodPostal", null),
            new SqlColumn("CabecDoc.Localidade", null),
            new SqlColumn("CabecDoc.Distrito", null),
            new SqlColumn("CabecDoc.Pais", null),
            new SqlColumn("CabecDoc.NumContribuinte", null)
        };
 
        private static bool CheckPermissions(GcpBEDocumentoVenda opportunityInfo, string sessionId)
        {
            if (opportunityInfo.get_Anulado())
            {
                return false;
            }

            var representativeId = opportunityInfo.get_Responsavel();

            if (representativeId != null && representativeId != sessionId)
            {
                return false;
            }

            return true;
        }

        public static List<Quote> List()
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("CabecDoc")
                .Columns(sqlQuoteColumns)
                .Where("TipoDoc", Comparison.Equals, "ECL"));

            if (queryObject.Vazia())
            {
                return null;
            }

            var queryResult = new List<Quote>();

            while (!queryObject.NoFim())
            {
                queryResult.Add(new Quote
                {
                    NumEncomenda = TypeParser.Integer(queryObject.Valor("NumDoc")),
                    Cliente = TypeParser.String(queryObject.Valor("Entidade")),
                    NomeCliente = TypeParser.String(queryObject.Valor("Nome")),
                    TotalDocumento = TypeParser.Double(queryObject.Valor("TotalMerc")),
                    Data = TypeParser.Date(queryObject.Valor("Data"))
                });
                queryObject.Seguinte();
            }

            return queryResult;
        }

        public static QuoteInfo View(string sessionId, string quoteId)
        {
            
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var quoteInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("CabecDoc")
                .Columns(sqlQuoteColumns)
                .Where("NumDoc", Comparison.Equals, quoteId));
            
            if (quoteInfo.Vazia() || TypeParser.String(quoteInfo.Valor("TipoDoc")) != "ECL" || TypeParser.String(quoteInfo.Valor("Responsavel")) != sessionId)
            {
                return null;
            }

            List<OrderInfo> quoteProducts = new List<OrderInfo>();

            var productsInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("LinhasDoc")
                .Columns(sqlProductsColumns)
                .Where("IdCabecDoc", Comparison.Equals, quoteInfo.Valor("Id")));

            while (!productsInfo.NoFim())
            {
                quoteProducts.Add(new OrderInfo
                {
                    Quantidade = TypeParser.Integer(productsInfo.Valor("Quantidade")),
                    Preco = TypeParser.Double(productsInfo.Valor("PrecUnit")),
                    Desconto = TypeParser.Double(productsInfo.Valor("Desconto1")),
                    Iva = TypeParser.Double(productsInfo.Valor("TaxaIva")),
                    Produto = new Reference(TypeParser.String(productsInfo.Valor("Artigo")), TypeParser.String(productsInfo.Valor("Descricao")))
                });

                productsInfo.Seguinte();
            }

           return new QuoteInfo
            {                
                NumEncomenda = TypeParser.Integer(quoteInfo.Valor("NumDoc")),
                Cliente = TypeParser.String(quoteInfo.Valor("Entidade")),
                NomeCliente = TypeParser.String(quoteInfo.Valor("Nome")),
                EnderecoEntrega = new Address
                {
                    Morada = TypeParser.String(quoteInfo.Valor("Morada")),
                    CodigoPostal = TypeParser.String(quoteInfo.Valor("CodPostal")),
                    Localidade = TypeParser.String(quoteInfo.Valor("Localidade")),
                    Distrito = TypeParser.String(quoteInfo.Valor("Distrito")),
                    Pais = TypeParser.String(quoteInfo.Valor("Pais"))
                },
                Data = TypeParser.Date(quoteInfo.Valor("Data")),
                IdOportunidade = TypeParser.String(quoteInfo.Valor("IdOportunidade")),
                TotalDesc = TypeParser.Double(quoteInfo.Valor("TotalDesc")),
                TotalIva = TypeParser.Double(quoteInfo.Valor("TotalIva")),
                TotalMerc = TypeParser.Double(quoteInfo.Valor("TotalMerc")),
                TotalDocumento = TypeParser.Double(quoteInfo.Valor("TotalDocumento")),
                NumContribuinte = TypeParser.Double(quoteInfo.Valor("NumContribuinte")),
                Produtos = quoteProducts
            };           
        }

        private static void SetOptionalFields(GcpBEDocumentoVenda quoteInfo, QuoteInfo jsonObject)
        {
            if (jsonObject.Cliente != null)
            {
                quoteInfo.set_Entidade(jsonObject.Cliente);               
                quoteInfo.set_EntidadeFac(jsonObject.Cliente);               
            }

            if (jsonObject.NomeCliente != null)
                quoteInfo.set_Nome(jsonObject.NomeCliente);
            
            if (jsonObject.EnderecoEntrega != null)
            {
                if (jsonObject.EnderecoEntrega != null)
                    quoteInfo.set_CodigoPostal(jsonObject.EnderecoEntrega.CodigoPostal);
                if (jsonObject.EnderecoEntrega != null)
                    quoteInfo.set_Distrito(jsonObject.EnderecoEntrega.Distrito);
                if (jsonObject.EnderecoEntrega != null)
                    quoteInfo.set_Localidade(jsonObject.EnderecoEntrega.Localidade);
                if (jsonObject.EnderecoEntrega != null)
                    quoteInfo.set_Morada(jsonObject.EnderecoEntrega.Morada);
                if (jsonObject.EnderecoEntrega != null)
                    quoteInfo.set_Pais(jsonObject.EnderecoEntrega.Pais);
            }
        }
        

        public static bool Update(string sessionId, string quoteId, QuoteInfo jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var quotesTable = PrimaveraEngine.Engine.Comercial.Vendas;

            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("CabecDoc")
                .Columns(new SqlColumn[]{new SqlColumn("CabecDoc.Id", null)})
                .Where("NumDoc", Comparison.Equals, quoteId));

            if (queryObject.Vazia())
            {
                return false;
            }

            var quoteInfo = quotesTable.EditaID(quoteId);

            if (CheckPermissions(quoteInfo, sessionId) == false)
            {
                return false;
            }
            try
            {
                quoteInfo.set_EmModoEdicao(true);
                quoteInfo.set_DataUltimaActualizacao(DateTime.Now);
                SetOptionalFields(quoteInfo, jsonObject);
                PrimaveraEngine.Engine.IniciaTransaccao();
                quotesTable.Actualiza(quoteInfo);
                PrimaveraEngine.Engine.TerminaTransaccao();
            }
            catch (Exception)
            {
                PrimaveraEngine.Engine.DesfazTransaccao();
                return false;
            }
            return true;
        }

        public static void SetDefaultQuotesInfo(string sessionId, GcpBEDocumentoVenda quoteInfo)
        {
            quoteInfo.set_Seccao(QuotesConstants.seccao);
            quoteInfo.set_Cambio(QuotesConstants.cambio);
            quoteInfo.set_CambioMAlt(QuotesConstants.cambioMAlt);
            quoteInfo.set_CambioMBase(QuotesConstants.cambioMBase);
            quoteInfo.set_CondPag(QuotesConstants.condPag); //Para alterar
            
            quoteInfo.set_Serie(QuotesConstants.serie);
            quoteInfo.set_Tipodoc(QuotesConstants.tipoDoc);
            quoteInfo.set_Responsavel(sessionId);
            quoteInfo.set_TipoEntidade(QuotesConstants.tipoEntidade);
            quoteInfo.set_DataDoc(System.DateTime.Now);
            quoteInfo.set_DataVenc(System.DateTime.Now);

            //PrimaveraEngine.Engine.Comercial.Vendas.PreencheDadosRelacionados(quoteInfo);
        }

        public static QuoteInfo Insert(string sessionId, QuoteInfo jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var quoteInfo = new GcpBEDocumentoVenda();

            try
            {
                SetDefaultQuotesInfo(sessionId, quoteInfo);
                SetOptionalFields(quoteInfo, jsonObject);

                System.Diagnostics.Debug.Print(System.DateTime.Now + "");
                
                if (jsonObject.Produtos != null)
                {
                    foreach (var produto in jsonObject.Produtos)
                    {
                        PrimaveraEngine.Engine.Comercial.Vendas.AdicionaLinha(quoteInfo, produto.Produto.Identificador, produto.Quantidade, "", "", produto.Preco, produto.Desconto);
                    }
                }

                PrimaveraEngine.Engine.IniciaTransaccao();
                PrimaveraEngine.Engine.Comercial.Vendas.Actualiza(quoteInfo);
                PrimaveraEngine.Engine.TerminaTransaccao();
                System.Diagnostics.Debug.Print(System.DateTime.Now + "");
            }
            catch (Exception ex)
            {
                PrimaveraEngine.Engine.DesfazTransaccao();
                throw ex;
            }
            return new QuoteInfo
            {
                NumEncomenda = quoteInfo.get_NumDoc(),
                Cliente = quoteInfo.get_Entidade(),
                NomeCliente = quoteInfo.get_Nome(),
                EnderecoEntrega = new Address
                {
                    Morada = quoteInfo.get_Morada(),
                    CodigoPostal = quoteInfo.get_CodigoPostal(),
                    Localidade = quoteInfo.get_Localidade(),
                    Distrito = quoteInfo.get_Distrito(),
                    Pais = quoteInfo.get_Pais()
                },
                Data = quoteInfo.get_DataDoc(),
                IdOportunidade = quoteInfo.get_IdOportunidade(),
                TotalMerc = quoteInfo.get_TotalMerc(),
                Produtos = null
            };
        }

        public static bool Delete(string sessionId, string quoteId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var quotesTable = PrimaveraEngine.Engine.Comercial.Vendas;

            if (quotesTable.ExisteID(quoteId) == false)
            {
                return false;
            }

            var quoteInfo = quotesTable.EditaID(quoteId);

            if (CheckPermissions(quoteInfo, sessionId) == false)
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