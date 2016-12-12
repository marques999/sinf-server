using System;
using System.Collections.Generic;

using Interop.CrmBE900;
using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class ProposalLinesIntegration
    {
        private static SqlColumn[] sqlColumnsListing =
        {
            new SqlColumn("LINHASPROPOSTASOPV.IdOportunidade", null),
            new SqlColumn("LINHASPROPOSTASOPV.NumProposta", null),
            new SqlColumn("LINHASPROPOSTASOPV.Linha", null),
            new SqlColumn("LINHASPROPOSTASOPV.Artigo", null),
            new SqlColumn("LINHASPROPOSTASOPV.Descricao", null),
            new SqlColumn("LINHASPROPOSTASOPV.Quantidade", null),
            new SqlColumn("LINHASPROPOSTASOPV.Unidade", null),
            new SqlColumn("LINHASPROPOSTASOPV.FactorConv", null),
            new SqlColumn("LINHASPROPOSTASOPV.PrecoCusto", null),
            new SqlColumn("LINHASPROPOSTASOPV.PrecoVenda", null),
            new SqlColumn("LINHASPROPOSTASOPV.Desconto1", null),
            new SqlColumn("LINHASPROPOSTASOPV.Desconto2", null),
            new SqlColumn("LINHASPROPOSTASOPV.Desconto3", null),
            new SqlColumn("LINHASPROPOSTASOPV.Desconto", null),
            new SqlColumn("LINHASPROPOSTASOPV.ValorDesconto", null),
            new SqlColumn("LINHASPROPOSTASOPV.Rentabilidade", null),
            new SqlColumn("LINHASPROPOSTASOPV.Margem", null),
            new SqlColumn("LINHASPROPOSTASOPV.Observacoes", null)
        };

        private static ProposalsLine GenerateListing(StdBELista proposalLineInfo)
        {
            return new ProposalsLine()
            {
                idOportunidade = TypeParser.String(proposalLineInfo.Valor("IdOportunidade")),
                ProposalNumber = proposalLineInfo.Valor("NumProposta"),
                Line = proposalLineInfo.Valor("Linha"),
                Article = TypeParser.String(proposalLineInfo.Valor("Artigo")),
                Description = TypeParser.String(proposalLineInfo.Valor("Descricao")),
                Quantity = TypeParser.Double(proposalLineInfo.Valor("Quantidade")),
                Unit = TypeParser.String(proposalLineInfo.Valor("Unidade")),
                FactorConv = TypeParser.Double(proposalLineInfo.Valor("FactorConv")),
                CostPrice = TypeParser.Double(proposalLineInfo.Valor("PrecoCusto")),
                SellsPrice = TypeParser.Double(proposalLineInfo.Valor("PrecoVenda")),
                Discount1 = TypeParser.Double(proposalLineInfo.Valor("Desconto1")),
                Discount2 = TypeParser.Double(proposalLineInfo.Valor("Desconto2")),
                Discount3 = TypeParser.Double(proposalLineInfo.Valor("Desconto3")),
                Discount = TypeParser.Double(proposalLineInfo.Valor("Desconto")),
                DiscountValue = TypeParser.Double(proposalLineInfo.Valor("ValorDesconto")),
                Rentability = TypeParser.Double(proposalLineInfo.Valor("Rentabilidade")),
                Margin = TypeParser.Double(proposalLineInfo.Valor("Margem")),
                Observations = TypeParser.String(proposalLineInfo.Valor("Observacoes")),
            };
        }

        private static ProposalsLine GenerateProposal(CrmBELinhaPropostaOPV proposalLineInfo)
        {
            return new ProposalsLine()
            {
                idOportunidade = proposalLineInfo.get_IdOportunidade(),
                ProposalNumber = proposalLineInfo.get_NumProposta(),
                Line = proposalLineInfo.get_Linha(),
                Article = proposalLineInfo.get_Artigo(),
                Description = proposalLineInfo.get_Descricao(),
                Quantity = proposalLineInfo.get_Quantidade(),
                Unit = proposalLineInfo.get_Unidade(),
                FactorConv = proposalLineInfo.get_FactorConv(),
                CostPrice = proposalLineInfo.get_PrecoCusto(),
                SellsPrice = proposalLineInfo.get_PrecoVenda(),
                Discount1 = proposalLineInfo.get_Desconto1(),
                Discount2 = proposalLineInfo.get_Desconto2(),
                Discount3 = proposalLineInfo.get_Desconto3(),
                Discount = proposalLineInfo.get_Desconto(),
                DiscountValue = proposalLineInfo.get_ValorDesconto(),
                Rentability = proposalLineInfo.get_Rentabilidade(),
                Margin = proposalLineInfo.get_Margem(),
                Observations = proposalLineInfo.get_Observacoes(),
            };
        }

        private static bool CheckPermissions(CrmBEOportunidadeVenda opportunityInfo, string sessionId)
        {
            if (opportunityInfo.get_EstadoVenda() == -1)
            {
                return false;
            }

            var representativeId = opportunityInfo.get_Vendedor();

            if (representativeId != null && representativeId != sessionId)
            {
                return false;
            }

            return true;
        }

        public static List<ProposalsLine> List(string id, short proposalNumber)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var formattedId = id.Replace("%7B", "{").Replace("%7D", "}");
            var queryResult = new List<ProposalsLine>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("LINHASPROPOSTASOPV")
                .Columns(sqlColumnsListing)
                .Where("IdOportunidade", Comparison.Equals, formattedId)
                .Where("NumProposta", Comparison.Equals, proposalNumber));

            while (!queryObject.NoFim())
            {
                queryResult.Add(GenerateListing(queryObject));
                queryObject.Seguinte();
            }

            return queryResult;
        }

        public static ProposalsLine View(string opportunityId, short numProposal, short line)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var opportunitiesTable = PrimaveraEngine.Engine.CRM.PropostasOPV.Edita(opportunityId, numProposal);

            //var linhax = opportunityInfo.get_Linhas().Edita(opportunityInfo);

            /*if (opportunityInfo.get_Vendedor() != sessionId)
            {
                return null; 
            }*/

            return GenerateProposal(opportunitiesTable.get_Linhas().get_Edita(line));
        }

        /*private static Proposals GenerateProposal(CrmBELinhasPropostaOPV opportunityInfo)
        {
 	        return new Proposals()
                    {
                        ProposalNumber = TypeParser.String(opportunityInfo.get_NumProposta()),
                        Description = opportunityInfo.get_Descricao(),
                        PaymentMethod = opportunityInfo.get_ModoPagamento(),
                        PaymentCondition = opportunityInfo.get_CondPagamento(),
                        Cost = opportunityInfo.get_Custo(),
                        Value = opportunityInfo.get_Valor(),
                        DiscountValue = opportunityInfo.get_ValorDesconto(),
                        Rentability = opportunityInfo.get_Rentabilidade(),
                        Margin = opportunityInfo.get_Margem()
                    };
        }*/

        private static void SetFields(CrmBELinhaPropostaOPV proposalLineInfo, ProposalsLine jsonObject)
        {
            proposalLineInfo.set_IdOportunidade(jsonObject.idOportunidade);
            proposalLineInfo.set_NumProposta(jsonObject.ProposalNumber);
            proposalLineInfo.set_Linha(jsonObject.Line);
            proposalLineInfo.set_Artigo(jsonObject.Article);
            proposalLineInfo.set_Descricao(jsonObject.Description);
            proposalLineInfo.set_Quantidade(jsonObject.Quantity);
            proposalLineInfo.set_Unidade(jsonObject.Unit);
            proposalLineInfo.set_FactorConv(jsonObject.FactorConv);
            proposalLineInfo.set_PrecoCusto(jsonObject.CostPrice);
            proposalLineInfo.set_PrecoVenda(jsonObject.SellsPrice);
            proposalLineInfo.set_Desconto1(jsonObject.Discount1);
            proposalLineInfo.set_Desconto2(jsonObject.Discount2);
            proposalLineInfo.set_Desconto3(jsonObject.Discount3);
            proposalLineInfo.set_Desconto(jsonObject.Discount);
            proposalLineInfo.set_ValorDesconto(jsonObject.DiscountValue);
            proposalLineInfo.set_Rentabilidade(jsonObject.Rentability);
            proposalLineInfo.set_Margem(jsonObject.Margin);
            proposalLineInfo.set_Observacoes(jsonObject.Observations);
        }

        public static ProposalsLine Update(string sessionId, string opportunityId, ProposalsLine jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            /*if (opportunitiesTable.Existe(opportunityId) == false)
            {
                throw new NotFoundException("oportunidade", true);
            }

            if (CheckPermissions(opportunityInfo, sessionId) == false)
            {
                return null;
            }*/

            var opportunitiesTableAux = PrimaveraEngine.Engine.CRM.PropostasOPV;
            var opportunitiesTable = PrimaveraEngine.Engine.CRM.PropostasOPV.Edita(jsonObject.idOportunidade, jsonObject.ProposalNumber);
            var proposalLines = opportunitiesTable.get_Linhas();

            opportunitiesTable.set_EmModoEdicao(true);

            var line = proposalLines.get_Edita(jsonObject.Line);

            line.set_EmModoEdicao(true);
            line.set_Artigo(jsonObject.Article);
            line.set_Descricao(jsonObject.Description);
            line.set_Quantidade(jsonObject.Quantity);
            line.set_Unidade(jsonObject.Unit);
            line.set_FactorConv(jsonObject.FactorConv);
            line.set_PrecoCusto(jsonObject.CostPrice);
            line.set_PrecoVenda(jsonObject.SellsPrice);
            line.set_Desconto1(jsonObject.Discount1);
            line.set_Desconto2(jsonObject.Discount2);
            line.set_Desconto3(jsonObject.Discount3);
            line.set_Desconto(jsonObject.Discount);
            line.set_ValorDesconto(jsonObject.DiscountValue);
            line.set_Rentabilidade(jsonObject.Rentability);
            line.set_Margem(jsonObject.Margin);
            line.set_Observacoes(jsonObject.Observations);
            proposalLines.Insere(line);
            opportunitiesTable.set_Linhas(proposalLines);
            opportunitiesTableAux.Actualiza(opportunitiesTable);

            return GenerateProposal(line);
        }

        public static ProposalsLine Insert(string sessionId, ProposalsLine jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var proposalLineInfo = new CrmBELinhaPropostaOPV();
            var opportunitiesTable = PrimaveraEngine.Engine.CRM.PropostasOPV;
            var proposalInfo = opportunitiesTable.Edita(jsonObject.idOportunidade, jsonObject.ProposalNumber);
            var proposalLines = proposalInfo.get_Linhas();

            /*if (opportunitiesTable.Existe(opportunityId))
            {
                throw new EntityExistsException("oportunidade", true);
            }*/

            proposalInfo.set_EmModoEdicao(true);
            SetFields(proposalLineInfo, jsonObject);
            proposalLines.Insere(proposalLineInfo);
            proposalInfo.set_Linhas(proposalLines);
            opportunitiesTable.Actualiza(proposalInfo);

            return GenerateProposal(proposalLineInfo);
        }

        public static bool Delete(string sessionId, string proposalsId, ProposalsLine jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            /*if (opportunitiesTable.Existe(opportunityId) == false)
            {
                throw new NotFoundException("oportunidade", true);
            }*/

            var proposalsTableAux = PrimaveraEngine.Engine.CRM.PropostasOPV;
            var proposalsTable = PrimaveraEngine.Engine.CRM.PropostasOPV.Edita(jsonObject.idOportunidade, jsonObject.ProposalNumber);
            var proposalLines = proposalsTable.get_Linhas();

            proposalsTable.set_EmModoEdicao(true);
            proposalLines.Remove(jsonObject.Line);
            proposalsTableAux.Actualiza(proposalsTable);

            /*if (CheckPermissions(opportunityInfo, sessionId) == false)
            {
                return false;
            }*/

            return true;
        }
    }
}