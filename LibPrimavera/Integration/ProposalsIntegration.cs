using System;
using System.Collections.Generic;

using Interop.CrmBE900;
using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class ProposalsIntegration
    {
        private static SqlColumn[] sqlColumnsListing =
        {
            new SqlColumn("PROPOSTASOPV.IdOportunidade", null),
            new SqlColumn("PROPOSTASOPV.NumProposta", null),
            new SqlColumn("PROPOSTASOPV.Descricao", null),
            new SqlColumn("PROPOSTASOPV.ModoPagamento", null),
            new SqlColumn("PROPOSTASOPV.CondPagamento", null),
            new SqlColumn("PROPOSTASOPV.Custo", null),
            new SqlColumn("PROPOSTASOPV.Valor", null),
            new SqlColumn("PROPOSTASOPV.ValorDesconto", null),
            new SqlColumn("PROPOSTASOPV.DescontoEntidade", null),
            new SqlColumn("PROPOSTASOPV.Rentabilidade", null),
            new SqlColumn("PROPOSTASOPV.Margem", null),
            new SqlColumn("PROPOSTASOPV.Observacoes", null),
            new SqlColumn("PROPOSTASOPV.NaoTotalizadora", null)
        };

        private static Proposals GenerateListing(StdBELista proposalInfo, string id)
        {
            var proposalsLines = ProposalLinesIntegration.List(id, proposalInfo.Valor("NumProposta"));

            return new Proposals()
            {
                idOportunidade = TypeParser.String(proposalInfo.Valor("IdOportunidade")),
                ProposalNumber = proposalInfo.Valor("NumProposta"),
                Description = TypeParser.String(proposalInfo.Valor("Descricao")),
                PaymentMethod = TypeParser.String(proposalInfo.Valor("ModoPagamento")),
                PaymentCondition = TypeParser.String(proposalInfo.Valor("CondPagamento")),
                Cost = TypeParser.Double(proposalInfo.Valor("Custo")),
                Value = TypeParser.Double(proposalInfo.Valor("Valor")),
                DiscountValue = TypeParser.Double(proposalInfo.Valor("ValorDesconto")),
                Rentability = TypeParser.Double(proposalInfo.Valor("Rentabilidade")),
                Margin = TypeParser.Double(proposalInfo.Valor("Margem")),
                Observations = TypeParser.String(proposalInfo.Valor("Observacoes")),
                EntityDiscount = TypeParser.Double(proposalInfo.Valor("DescontoEntidade")),
                Totalize = proposalInfo.Valor("NaoTotalizadora"),
                ProposalsLines = proposalsLines
            };
        }

        private static Proposals GenerateProposal(CrmBEPropostaOPV proposalInfo, List<ProposalsLine> lines)
        {
            return new Proposals()
            {
                ProposalNumber = proposalInfo.get_NumProposta(),
                Description = proposalInfo.get_Descricao(),
                PaymentMethod = proposalInfo.get_ModoPagamento(),
                PaymentCondition = proposalInfo.get_CondPagamento(),
                Cost = proposalInfo.get_Custo(),
                Value = proposalInfo.get_Valor(),
                DiscountValue = proposalInfo.get_ValorDesconto(),
                Observations = proposalInfo.get_Observacoes(),
                Margin = proposalInfo.get_Margem(),
                EntityDiscount = proposalInfo.get_DescontoEntidade(),
                Rentability = proposalInfo.get_Rentabilidade(),
                ProposalsLines = lines,
                Totalize = proposalInfo.get_NaoTotalizadora()
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

        public static List<Proposals> List(string sessionId, string opportunityId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var proposalInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("PROPOSTASOPV")
                .Columns(sqlColumnsListing)
                .Where("IdOportunidade", Comparison.Equals, opportunityId));

            if (proposalInfo == null || proposalInfo.Vazia())
            {
                throw new NotFoundException("oportunidade", true);
            }

            var queryResult = new List<Proposals>();

            while (!proposalInfo.NoFim())
            {
                queryResult.Add(GenerateListing(proposalInfo, opportunityId));
                proposalInfo.Seguinte();
            }

            return queryResult;
        }

        public static Proposals View(string sessionId, string opportunityId, short proposalNumber)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var proposalsTable = PrimaveraEngine.Engine.CRM.PropostasOPV.Edita(opportunityId, proposalNumber);
            var proposalInfo = ProposalLinesIntegration.List(opportunityId, proposalNumber);

            //var linhax = opportunityInfo.get_Linhas().Edita(opportunityInfo);

            /*if (opportunityInfo.get_Vendedor() != sessionId)
            {
                return null; 
            }*/

            return GenerateProposal(proposalsTable, proposalInfo);
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

        private static void SetFields(CrmBEPropostaOPV proposalInfo, Proposals jsonObject)
        {
            proposalInfo.set_IdOportunidade(jsonObject.idOportunidade);
            proposalInfo.set_DescontoEntidade(jsonObject.EntityDiscount);
            proposalInfo.set_NumProposta(jsonObject.ProposalNumber);
            proposalInfo.set_Descricao(jsonObject.Description);
            proposalInfo.set_ModoPagamento(jsonObject.PaymentMethod);
            proposalInfo.set_CondPagamento(jsonObject.PaymentCondition);
            proposalInfo.set_Custo(jsonObject.Cost);
            proposalInfo.set_Valor(jsonObject.Value);
            proposalInfo.set_ValorDesconto(jsonObject.DiscountValue);
            proposalInfo.set_Rentabilidade(jsonObject.Rentability);
            proposalInfo.set_Margem(jsonObject.Margin);
            proposalInfo.set_NaoTotalizadora(jsonObject.Totalize);
            proposalInfo.set_Observacoes(jsonObject.Observations);
        }

        public static Proposals Update(string sessionId, string opportunityId, Proposals jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var proposalsTable = PrimaveraEngine.Engine.CRM.PropostasOPV;

            /*if (opportunitiesTable.Existe(opportunityId) == false)
            {
                throw new NotFoundException("oportunidade", true);
            }*/

            var proposalInfo = proposalsTable.Edita(opportunityId, jsonObject.ProposalNumber);

            /*if (CheckPermissions(opportunityInfo, sessionId) == false)
            {
                return null;
            }*/

            //var proposalsInfo = opportunitiesTable.EditaPropostasOPV(opportunityId);
            //var currentProposal = proposalsInfo.get_Edita(0);
            //currentProposal.set_EmModoEdicao(true);
            //var linhasProposta = currentProposal.get_Linhas();

            proposalInfo.set_EmModoEdicao(true);
            //SetFields(proposalInfo, jsonObject);
            proposalInfo.set_NumProposta(jsonObject.ProposalNumber);
            proposalInfo.set_Descricao(jsonObject.Description);
            proposalInfo.set_ModoPagamento(jsonObject.PaymentMethod);
            proposalInfo.set_CondPagamento(jsonObject.PaymentCondition);
            proposalInfo.set_Custo(jsonObject.Cost);
            proposalInfo.set_Valor(jsonObject.Value);
            proposalInfo.set_ValorDesconto(jsonObject.DiscountValue);
            proposalInfo.set_Rentabilidade(jsonObject.Rentability);
            proposalInfo.set_Margem(jsonObject.Margin);
            proposalInfo.set_Observacoes(jsonObject.Observations);
            proposalsTable.Actualiza(proposalInfo);

            return GenerateProposal(proposalInfo, jsonObject.ProposalsLines);
        }

        public static Proposals Insert(string sessionId, Proposals jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var proposalInfo = new CrmBEPropostaOPV();
            var proposalsTable = PrimaveraEngine.Engine.CRM.PropostasOPV;

            /*if (opportunitiesTable.Existe(opportunityId))
            {
                throw new EntityExistsException("oportunidade", true);
            }*/

            SetFields(proposalInfo, jsonObject);
            proposalsTable.Actualiza(proposalInfo);

            /* for (int i = 0; i < jsonObject.ProposalsLines.Count; i++)
             {
                 ProposalLinesIntegration.Insert(jsonObject.ProposalsLines[i]);
             }*/

            return GenerateProposal(proposalInfo, jsonObject.ProposalsLines);
        }

        public static bool DeleteAll(string sessionId, string opportunityId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var proposalsTable = PrimaveraEngine.Engine.CRM.PropostasOPV;

            /*if (opportunitiesTable.Existe(opportunityId) == false)
            {
                throw new NotFoundException("oportunidade", true);
            }*/

            //var proposalInfo = proposalsTable.Edita(proposalsId, );

            /*if (opportunitiesTable.Existe(opportunityId) == false)
            {
                throw new NotFoundException("oportunidade", true);
            }*/

            //                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  var opportunityInfo = propostasTable.Edita(proposalsId, proposalNumber);

            /*if (CheckPermissions(opportunityInfo, sessionId) == false)
            {
                return false;
            }*/
            /*proposalInfo.set_EmModoEdicao(true);
            proposalsTable.Actualiza(proposalInfo);
            propostasTable.Remove(proposalsId, proposalNumber);*/

            /*opportunityInfo.set_EmModoEdicao(true);
            opportunityInfo.set_Descricao("DELETED");
            opportunityInfo.set_NumProposta((short)(-1 * opportunityInfo.get_NumProposta()));
            propostasTable.Actualiza(opportunityInfo);*/

            return true;
        }

        public static bool Delete(string sessionId, string proposalsId, short proposalNumber)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var proposalsTable = PrimaveraEngine.Engine.CRM.PropostasOPV;

            /*if (opportunitiesTable.Existe(opportunityId) == false)
            {
                throw new NotFoundException("oportunidade", true);
            }*/

            var proposalInfo = proposalsTable.Edita(proposalsId, proposalNumber);

            /*if (opportunitiesTable.Existe(opportunityId) == false)
            {
                throw new NotFoundException("oportunidade", true);
            }*/

            //var opportunityInfo = propostasTable.Edita(proposalsId, proposalNumber);

            /*if (CheckPermissions(opportunityInfo, sessionId) == false)
            {
                return false;
            }*/

            proposalInfo.set_EmModoEdicao(true);
            proposalsTable.EditaLinhas(proposalsId, proposalNumber).RemoveTodos();
            proposalsTable.Remove(proposalsId, proposalNumber);
            proposalsTable.Actualiza(proposalInfo);

            /*opportunityInfo.set_EmModoEdicao(true);
            opportunityInfo.set_Descricao("DELETED");
            opportunityInfo.set_NumProposta((short)(-1 * opportunityInfo.get_NumProposta()));
            propostasTable.Actualiza(opportunityInfo);*/

            return true;
        }
    }
}