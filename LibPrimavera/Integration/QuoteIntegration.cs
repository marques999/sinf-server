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
        private static bool CheckPermissions(GcpBEDocumentoVenda opportunityInfo, string sessionId)
        {
            /*if (opportunityInfo.get_Anulado())
            {
                return false;
            }

            var representativeId = opportunityInfo.get_Responsavel();

            if (representativeId != null && representativeId != sessionId)
            {
                return false;
            }*/

            return true;
        }

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
            new SqlColumn("CabecDoc.TotalMerc", null),
            new SqlColumn("CabecDoc.Morada", null),
            new SqlColumn("CabecDoc.CodPostal", null),
            new SqlColumn("CabecDoc.Localidade", null),
            new SqlColumn("CabecDoc.Distrito", null),
            new SqlColumn("CabecDoc.Pais", null)
        };

        public static List<QuoteListing> List()
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var quoteList = new List<QuoteListing>();
            var quoteInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("CabecDoc")
                .Columns(sqlQuoteColumns)
                .Where("TipoDoc", Comparison.Equals, "ECL"));

            if (quoteInfo == null || quoteInfo.Vazia())
            {
                return quoteList;
            }

            while (!quoteInfo.NoFim())
            {
                quoteList.Add(new QuoteListing
                {
                    NumEncomenda = TypeParser.Integer(quoteInfo.Valor("NumDoc")),
                    Cliente = TypeParser.String(quoteInfo.Valor("Entidade")),
                    NomeCliente = TypeParser.String(quoteInfo.Valor("Nome")),
                    Total = TypeParser.Double(quoteInfo.Valor("TotalMerc")),
                    DataEncomenda = TypeParser.Date(quoteInfo.Valor("Data"))
                });

                quoteInfo.Seguinte();
            }

            return quoteList;
        }

        private static SqlColumn[] sqlProductsColumns =
        {
            new SqlColumn("LinhasDoc.Artigo", null),
            new SqlColumn("LinhasDoc.Descricao", null),
            new SqlColumn("LinhasDoc.Quantidade", null),
            new SqlColumn("LinhasDoc.Unidade", null),
            new SqlColumn("LinhasDoc.PrecUnit", null),
            new SqlColumn("LinhasDoc.Desconto1", null),
            new SqlColumn("LinhasDoc.TotalILiquido", null),
            new SqlColumn("LinhasDoc.PrecoLiquido", null)
        };

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

            if (quoteInfo == null || quoteInfo.Vazia())
            {
                throw new NotFoundException("encomenda", true);
            }

            var quoteType = TypeParser.String(quoteInfo.Valor("TipoDoc"));
            var representativeId = TypeParser.String(quoteInfo.Valor("Responsavel"));

            if (quoteType.Equals("ECL") == false || representativeId != sessionId)
            {
                return null;
            }

            List<OrderInfo> quoteProducts = new List<OrderInfo>();

            var productsInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("LinhasDoc")
                .Columns(sqlProductsColumns)
                .Where("IdCabecDoc", Comparison.Equals, TypeParser.String(quoteInfo.Valor("Id"))));

            while (!productsInfo.NoFim())
            {
                quoteProducts.Add(new OrderInfo
                {
                    Quantidade = TypeParser.Integer(productsInfo.Valor("Quantidade")),
                    Preco = TypeParser.Double(productsInfo.Valor("PrecUnit")),
                    Desconto = TypeParser.Double(productsInfo.Valor("Desconto1")),
                    ResLiquido = TypeParser.Double(productsInfo.Valor("TotalILiquido")),
                    Produto = new Reference(TypeParser.String(productsInfo.Valor("Artigo")), TypeParser.String(productsInfo.Valor("Descricao")))
                });

                productsInfo.Seguinte();
            }

            return new QuoteInfo
            {
                NumEncomenda = TypeParser.Integer(quoteInfo.Valor("NumDoc")),
                TipoEntidade = TypeParser.String(quoteInfo.Valor("Entidade")),
                Entidade = TypeParser.String(quoteInfo.Valor("Nome")),
                EnderecoExpedicao = new Address
                {
                    Morada = TypeParser.String(quoteInfo.Valor("Morada")),
                    CodigoPostal = TypeParser.String(quoteInfo.Valor("CodPostal")),
                    Localidade = TypeParser.String(quoteInfo.Valor("Localidade")),
                    Distrito = TypeParser.String(quoteInfo.Valor("Distrito")),
                    Pais = TypeParser.String(quoteInfo.Valor("Pais"))
                },
                DataEncomenda = TypeParser.Date(quoteInfo.Valor("Data")),
                Oportunidade = TypeParser.String(quoteInfo.Valor("IdOportunidade")),
                Total = TypeParser.Double(quoteInfo.Valor("TotalMerc")),
                Produtos = quoteProducts
            };
        }

        private static void SetOptionalFields(GcpBEDocumentoVenda quoteInfo, QuoteInfo jsonObject)
        {
            quoteInfo.set_Entidade(jsonObject.Entidade);
            quoteInfo.set_EntidadeFac(jsonObject.Entidade);
            quoteInfo.set_Nome(jsonObject.TipoEntidade);

            if (jsonObject.EnderecoExpedicao != null)
            {
                quoteInfo.set_Pais(jsonObject.EnderecoExpedicao.Pais);
                quoteInfo.set_Morada(jsonObject.EnderecoExpedicao.Morada);
                quoteInfo.set_CodigoPostal(jsonObject.EnderecoExpedicao.CodigoPostal);

                if (jsonObject.EnderecoExpedicao.Pais.Equals("PT"))
                {
                    if (jsonObject.EnderecoExpedicao.Distrito == null)
                    {
                        quoteInfo.set_Distrito(null);
                        quoteInfo.set_Localidade(null);
                        quoteInfo.set_LocalidadeCodigoPostal(null);
                    }
                    else
                    {
                        quoteInfo.set_Distrito(jsonObject.EnderecoExpedicao.Distrito);
                        quoteInfo.set_Localidade(jsonObject.EnderecoExpedicao.Localidade);
                        quoteInfo.set_LocalidadeCodigoPostal(jsonObject.EnderecoExpedicao.Localidade);
                    }
                }
                else
                {
                    quoteInfo.set_Distrito(null);
                    quoteInfo.set_Localidade(null);
                    quoteInfo.set_LocalidadeCodigoPostal(null);
                }
            }

            if (jsonObject.EnderecoFacturacao != null)
            {
                quoteInfo.set_PaisFac(jsonObject.EnderecoFacturacao.Pais);
                quoteInfo.set_MoradaFac(jsonObject.EnderecoFacturacao.Morada);
                quoteInfo.set_CodigoPostalFac(jsonObject.EnderecoFacturacao.CodigoPostal);

                if (jsonObject.EnderecoFacturacao.Pais.Equals("PT"))
                {
                    if (jsonObject.EnderecoFacturacao.Distrito == null)
                    {
                        quoteInfo.set_DistritoFac(null);
                        quoteInfo.set_LocalidadeFac(null);
                        quoteInfo.set_LocalidadeCodigoPostalFac(null);
                    }
                    else
                    {
                        quoteInfo.set_DistritoFac(jsonObject.EnderecoFacturacao.Distrito);
                        quoteInfo.set_LocalidadeFac(jsonObject.EnderecoFacturacao.Localidade);
                        quoteInfo.set_LocalidadeCodigoPostalFac(jsonObject.EnderecoFacturacao.Localidade);
                    }
                }
                else
                {
                    quoteInfo.set_DistritoFac(null);
                    quoteInfo.set_LocalidadeFac(null);
                    quoteInfo.set_LocalidadeCodigoPostalFac(null);
                }
            }
        }

        public static QuoteInfo Update(string sessionId, string quoteId, QuoteInfo jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("CabecDoc")
                .Columns(new SqlColumn[] { new SqlColumn("CabecDoc.Id", null) })
                .Where("NumDoc", Comparison.Equals, quoteId));

            if (queryObject == null || queryObject.Vazia())
            {
                throw new NotFoundException("encomenda", true);
            }

            /*if (CheckPermissions(quoteInfo, sessionId) == false)
            {
                return false;
            }*/

            var quotesTable = PrimaveraEngine.Engine.Comercial.Vendas;
            var quoteInfo = quotesTable.EditaID(quoteId);

            /*try
            {*/
            quoteInfo.set_EmModoEdicao(true);
            quoteInfo.set_DataUltimaActualizacao(DateTime.Now);
            SetOptionalFields(quoteInfo, jsonObject);
            //PrimaveraEngine.Engine.IniciaTransaccao();
            quotesTable.Actualiza(quoteInfo);
            //PrimaveraEngine.Engine.TerminaTransaccao();
            /*}
            catch (Exception ex)
            {
                PrimaveraEngine.Engine.DesfazTransaccao();
                throw ex;
            }*/

            return generateQuote(quoteInfo, null);
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
        }

        private static QuoteInfo generateQuote(GcpBEDocumentoVenda quoteInfo, List<OrderInfo> productsInfo)
        {
            return new QuoteInfo
            {
                NumEncomenda = Convert.ToString(quoteInfo.get_NumDoc()),
                TipoEntidade = quoteInfo.get_Entidade(),
                Entidade = quoteInfo.get_Nome(),
                EnderecoExpedicao = new Address
                {
                    Morada = quoteInfo.get_Morada(),
                    CodigoPostal = quoteInfo.get_CodigoPostal(),
                    Localidade = quoteInfo.get_Localidade(),
                    Distrito = quoteInfo.get_Distrito(),
                    Pais = quoteInfo.get_Pais()
                },
                EnderecoFacturacao = new Address
                {
                    Morada = quoteInfo.get_MoradaFac(),
                    CodigoPostal = quoteInfo.get_CodigoPostalFac(),
                    Localidade = quoteInfo.get_LocalidadeFac(),
                    Distrito = quoteInfo.get_DistritoFac(),
                    Pais = quoteInfo.get_PaisFac()
                },
                DataEncomenda = quoteInfo.get_DataDoc(),
                Oportunidade = OpportunityIntegration.Reference(quoteInfo.get_IdOportunidade()),
                Total = quoteInfo.get_TotalMerc(),
                Produtos = productsInfo
            };
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
                PrimaveraEngine.Engine.IniciaTransaccao();
                PrimaveraEngine.Engine.Comercial.Vendas.Actualiza(quoteInfo);

                if (jsonObject.Produtos != null)
                {
                    foreach (var produto in jsonObject.Produtos)
                    {
                        PrimaveraEngine.Engine.Comercial.Vendas.AdicionaLinha(quoteInfo, produto.Produto.Identificador, produto.Quantidade, "", "", produto.Preco, produto.Desconto);
                    }
                }

                PrimaveraEngine.Engine.TerminaTransaccao();
                System.Diagnostics.Debug.Print(System.DateTime.Now + "");
            }
            catch (Exception ex)
            {
                PrimaveraEngine.Engine.DesfazTransaccao();
                throw ex;
            }

            return generateQuote(quoteInfo, null);
        }

        public static QuoteInfo Delete(string sessionId, string quoteId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var quotesTable = PrimaveraEngine.Engine.Comercial.Vendas;

            if (quotesTable.ExisteID(quoteId) == false)
            {
                throw new NotFoundException("encomenda", true);
            }

            var quoteInfo = quotesTable.EditaID(quoteId);

            if (CheckPermissions(quoteInfo, sessionId) == false)
            {
                return null;
            }

            quoteInfo.set_EmModoEdicao(true);
            quoteInfo.set_Anulado(true);
            quotesTable.Actualiza(quoteInfo);

            return generateQuote(quoteInfo, null);
        }
    }
}