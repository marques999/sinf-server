using System;
using System.Collections.Generic;

using Interop.ICrmBS900;
using Interop.CrmBE900;
using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class OpportunityIntegration
    {
        private static SqlColumn[] sqlColumnsListing =
        {
            new SqlColumn("CABECOPORTUNIDADESVENDA.ID", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.Entidade", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.BarraPercentual", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.Campanha", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.CicloVenda", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.DataCriacao", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.DataEncomenda", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.DataExpiracao", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.DataRealEncomenda", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.Descricao", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.MargemOV", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.MargemPercOV", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.Origem", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.ValorEncomendaOV", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.ValorPropostoOV", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.ValorTotalOV", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.Vendedor", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.CriadoPor", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.DataRealFacturacao", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.DataFecho", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.MotivoPerda", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.Oportunidade", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.Moeda", null),
            new SqlColumn("CABECOPORTUNIDADESVENDA.Resumo", null)
        };

        private static Opportunity GenerateListing(StdBELista opportunityInfo)
        {
            return new Opportunity()
            {
                Entity = TypeParser.String(opportunityInfo.Valor("Entidade")),
                Campaign = TypeParser.String(opportunityInfo.Valor("Campanha")),
                SellCycle = TypeParser.String(opportunityInfo.Valor("CicloVenda")),
                ExpirationDate = TypeParser.Date(opportunityInfo.Valor("DataExpiracao")),
                RealDateOrdered = TypeParser.Date(opportunityInfo.Valor("DataRealEncomenda")),
                Description = TypeParser.String(opportunityInfo.Valor("Descricao")),
                MarginOV = (float)TypeParser.Double(opportunityInfo.Valor("MargemOV")),
                ProposedValueOV = (float)TypeParser.Double(opportunityInfo.Valor("MargemPercOV")),
                Origin = TypeParser.String(opportunityInfo.Valor("Origem")),
                Seller = TypeParser.String(opportunityInfo.Valor("Vendedor")),
                CreatedBy = TypeParser.String(opportunityInfo.Valor("CriadoPor")),
                RealBillingDate = TypeParser.Date(opportunityInfo.Valor("DataRealFacturacao")),
                ClosureDate = TypeParser.Date(opportunityInfo.Valor("DataFecho")),
                LossMotive = TypeParser.String(opportunityInfo.Valor("MotivoPerda")),
                OpportunityId = TypeParser.String(opportunityInfo.Valor("Oportunidade")),
                Currency = TypeParser.String(opportunityInfo.Valor("Moeda")),
                Identificador = TypeParser.String(opportunityInfo.Valor("ID")),
                Brief = TypeParser.String(opportunityInfo.Valor("Resumo"))
            };
        }

        private static OpportunityInfo GenerateOpportunity(CrmBEOportunidadeVenda opportunityInfo)
        {
            return new OpportunityInfo()
            {
                DataCriacao = opportunityInfo.get_DataCriacao(),
                Entity = opportunityInfo.get_Entidade(),
                Campaign = opportunityInfo.get_Campanha(),
                SellCycle = opportunityInfo.get_CicloVenda(),
                ExpirationDate = opportunityInfo.get_DataExpiracao(),
                RealDateOrdered = opportunityInfo.get_DataRealEncomenda(),
                Description = opportunityInfo.get_Descricao(),
                MarginOV = (float)opportunityInfo.get_MargemOV(),
                ProposedValueOV = (float)opportunityInfo.get_ValorPropostoOV(),
                Origin = opportunityInfo.get_Origem(),
                Seller = opportunityInfo.get_Vendedor(),
                CreatedBy = opportunityInfo.get_CriadoPor(),
                RealBillingDate = opportunityInfo.get_DataRealFacturacao(),
                ClosureDate = opportunityInfo.get_DataFecho(),
                LossMotive = opportunityInfo.get_MotivoPerda(),
                OpportunityId = opportunityInfo.get_Oportunidade(),
                Currency = opportunityInfo.get_Moeda(),
                Brief = opportunityInfo.get_Resumo()
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

        public static List<Opportunity> List(string sessionId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Opportunity>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("CABECOPORTUNIDADESVENDA")
                .Columns(sqlColumnsListing)
                .Where("Descricao", Comparison.NotEquals, "DELETED"));

            while (!queryObject.NoFim())
            {
                queryResult.Add(GenerateListing(queryObject));
                queryObject.Seguinte();
            }

            return queryResult;
        }

        public static OpportunityInfo View(string sessionId, string opportunityId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var opportunitiesTable = PrimaveraEngine.Engine.CRM.OportunidadesVenda;

            if (opportunitiesTable.Existe(opportunityId) == false)
            {
                return null;
            }

            var opportunityInfo = opportunitiesTable.Edita(opportunityId);

            /*if (opportunityInfo.get_Vendedor() != sessionId)
            {
                return null;
            }*/

            return GenerateOpportunity(opportunityInfo);
        }

        private static void SetFields(CrmBEOportunidadeVenda opportunityInfo, Opportunity jsonObject)
        {
            opportunityInfo.set_Entidade(jsonObject.Entity);
            opportunityInfo.set_Campanha(jsonObject.Campaign);
            opportunityInfo.set_CicloVenda(jsonObject.SellCycle);
            opportunityInfo.set_DataEncomenda(jsonObject.DateOrdered);
            opportunityInfo.set_DataExpiracao(jsonObject.ExpirationDate);
            opportunityInfo.set_DataRealEncomenda(jsonObject.RealBillingDate);
            opportunityInfo.set_Descricao(jsonObject.Description);
            opportunityInfo.set_MargemOV(jsonObject.MarginOV);
            opportunityInfo.set_Origem(jsonObject.Origin);
            opportunityInfo.set_ValorEncomendaOV(jsonObject.OrderValueOV);
            opportunityInfo.set_ValorPropostoOV(jsonObject.ProposedValueOV);
            opportunityInfo.set_Zona(jsonObject.Zone);
            opportunityInfo.set_Vendedor(jsonObject.Seller);
            opportunityInfo.set_CriadoPor(jsonObject.CreatedBy);
            opportunityInfo.set_DataRealFacturacao(jsonObject.RealBillingDate);
            opportunityInfo.set_DataFecho(jsonObject.ClosureDate);
            opportunityInfo.set_MotivoPerda(jsonObject.LossMotive);
            opportunityInfo.set_Oportunidade(jsonObject.OpportunityId);
            opportunityInfo.set_Moeda(jsonObject.Currency);
            opportunityInfo.set_Resumo(jsonObject.Brief);
            opportunityInfo.set_TipoEntidade(jsonObject.EntityType);
        }

        public static OpportunityInfo Update(string sessionId, string opportunityId, Opportunity jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var opportunitiesTable = PrimaveraEngine.Engine.CRM.OportunidadesVenda;

            if (opportunitiesTable.Existe(opportunityId) == false)
            {
                return null;
            }

            var opportunityInfo = opportunitiesTable.Edita(opportunityId);

            /*if (CheckPermissions(opportunityInfo, sessionId) == false)
            {
                return null;
            }*/

            opportunityInfo.set_EmModoEdicao(true);
            SetFields(opportunityInfo, jsonObject);
            opportunityInfo.set_CriadoPor(sessionId);
            opportunityInfo.set_DataCriacao(DateTime.Now);
            opportunitiesTable.Actualiza(opportunityInfo);

            return GenerateOpportunity(opportunityInfo);
        }

        public static OpportunityInfo Insert(string sessionId, Opportunity jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var opportunityInfo = new CrmBEOportunidadeVenda();
            var opportunityId = PrimaveraEngine.GenerateHash();
            var opportunitiesTable = PrimaveraEngine.Engine.CRM.OportunidadesVenda;

            if (opportunitiesTable.Existe(opportunityId))
            {
                return null;
            }

            SetFields(opportunityInfo, jsonObject);
            opportunityInfo.set_CriadoPor(sessionId);
            opportunityInfo.set_DataCriacao(DateTime.Now);
            opportunitiesTable.Actualiza(opportunityInfo);

            return GenerateOpportunity(opportunityInfo);
        }

        public static bool Delete(string sessionId, string opportunityId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var opportunitiesTable = PrimaveraEngine.Engine.CRM.OportunidadesVenda;

            if (opportunitiesTable.Existe(opportunityId) == false)
            {
                return false;
            }

            var opportunityInfo = opportunitiesTable.Edita(opportunityId);

            /*if (CheckPermissions(opportunityInfo, sessionId) == false)
            {
                return false;
            }*/

            opportunityInfo.set_EmModoEdicao(true);
            opportunityInfo.set_Descricao("DELETED");
            opportunitiesTable.Actualiza(opportunityInfo);

            return true;
        }
    }
}