using System;
using System.Collections.Generic;

using Interop.ICrmBS900;
using Interop.CrmBE900;

using FirstREST.LibPrimavera.Model;
using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using Interop.StdBE900;

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

        private static Opportunity GenerateListing(StdBELista queryObject)
        {
            return new Opportunity()
            {
                Entity = TypeParser.String(queryObject.Valor("Entidade")),
                Campaign = TypeParser.String(queryObject.Valor("Campanha")),
                SellCycle = TypeParser.String(queryObject.Valor("CicloVenda")),
                ExpirationDate = TypeParser.Date(queryObject.Valor("DataExpiracao")),
                RealDateOrdered = TypeParser.Date(queryObject.Valor("DataRealEncomenda")),
                Description = TypeParser.String(queryObject.Valor("Descricao")),
                MarginOV = (float)TypeParser.Double(queryObject.Valor("MargemOV")),
                ProposedValueOV = (float)TypeParser.Double(queryObject.Valor("MargemPercOV")),
                Origin = TypeParser.String(queryObject.Valor("Origem")),
                Seller = TypeParser.String(queryObject.Valor("Vendedor")),
                CreatedBy = TypeParser.String(queryObject.Valor("CriadoPor")),
                RealBillingDate = TypeParser.Date(queryObject.Valor("DataRealFacturacao")),
                ClosureDate = TypeParser.Date(queryObject.Valor("DataFecho")),
                LossMotive = TypeParser.String(queryObject.Valor("MotivoPerda")),
                Opportunityy = TypeParser.String(queryObject.Valor("Oportunidade")),
                Currency = TypeParser.String(queryObject.Valor("Moeda")),
                Identificador = TypeParser.String(queryObject.Valor("ID")),
                Brief = TypeParser.String(queryObject.Valor("Resumo"))

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

        public static Opportunity View(string sessionId, string opportunityId)
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

            return new Opportunity()
            {
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
                Opportunityy = opportunityInfo.get_Oportunidade(),
                Currency = opportunityInfo.get_Moeda(),
                Brief = opportunityInfo.get_Resumo()
            };
        }

        private static void SetFields(CrmBEOportunidadeVenda opportunityInfo, Opportunity jsonObject)
        {
        }

        public static bool Update(string sessionId, string opportunityId, Opportunity jsonObject)
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

            /*if (opportunityInfo.get_Vendedor() != sessionId)

            if (CheckPermissions(opportunityInfo, sessionId) == false)
            {
                return false;
            }*/

            opportunityInfo.set_EmModoEdicao(true);

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
            opportunityInfo.set_Oportunidade(jsonObject.Opportunityy);
            opportunityInfo.set_Moeda(jsonObject.Currency);
            opportunityInfo.set_Resumo(jsonObject.Brief);
            opportunityInfo.set_TipoEntidade(jsonObject.EntityType);
            //opportunityInfo.set_ID(jsonObject.Opportunityy);
            //opportunityInfo.set_Oportunidade(jsonObject.Opportunityy);
            //opportunityInfo.set_ID
            opportunityInfo.set_CriadoPor(sessionId);
            opportunityInfo.set_DataCriacao(DateTime.Now);
            //opportunityInfo.set_Oportunidade(opportunityId);
            opportunitiesTable.Actualiza(opportunityInfo);
            //SetFields(opportunityInfo, jsonObject);

            return true;
        }

        public static bool Insert(string sessionId, Opportunity jsonObject)
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
                return false;
            }

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
            opportunityInfo.set_Oportunidade(jsonObject.Opportunityy);
            opportunityInfo.set_Moeda(jsonObject.Currency);
            opportunityInfo.set_Resumo(jsonObject.Brief);
            opportunityInfo.set_TipoEntidade(jsonObject.EntityType);
            //opportunityInfo.set_ID(jsonObject.Opportunityy);
            //opportunityInfo.set_Oportunidade(jsonObject.Opportunityy);
            //opportunityInfo.set_ID
            opportunityInfo.set_CriadoPor(sessionId);
            opportunityInfo.set_DataCriacao(DateTime.Now);
            //opportunityInfo.set_Oportunidade(opportunityId);
            opportunitiesTable.Actualiza(opportunityInfo);

            return true;
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


            /*if (opportunityInfo.get_Vendedor() != sessionId)

            if (CheckPermissions(opportunityInfo, sessionId) == false)
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