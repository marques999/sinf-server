using System;
using System.Collections.Generic;

using Interop.CrmBE900;
using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class LeadIntegration
    {
        private static SqlColumn[] sqlColumnsFull =
        {
            new SqlColumn("ENTIDADESEXTERNAS.Entidade", null),
            new SqlColumn("ENTIDADESEXTERNAS.Nome", null),
            new SqlColumn("ENTIDADESEXTERNAS.Email", null),
            new SqlColumn("ENTIDADESEXTERNAS.Activo", null),
            new SqlColumn("ENTIDADESEXTERNAS.DataCriacao", null),
            new SqlColumn("ENTIDADESEXTERNAS.DataUltAct", null),
            new SqlColumn("ENTIDADESEXTERNAS.Telefone", null),
            new SqlColumn("ENTIDADESEXTERNAS.Telemovel", null),
            new SqlColumn("ENTIDADESEXTERNAS.CodPostal", null),
            new SqlColumn("ENTIDADESEXTERNAS.Distrito", null),
            new SqlColumn("ENTIDADESEXTERNAS.Localidade", null),
            new SqlColumn("ENTIDADESEXTERNAS.Morada", null),
            new SqlColumn("ENTIDADESEXTERNAS.Pais", null)            
        };

        private static SqlColumn[] sqlColumnsListing =
        {
            new SqlColumn("ENTIDADESEXTERNAS.Entidade", null),
            new SqlColumn("ENTIDADESEXTERNAS.Nome", null),
            new SqlColumn("ENTIDADESEXTERNAS.Email", null),
            new SqlColumn("ENTIDADESEXTERNAS.Activo", null),
            new SqlColumn("ENTIDADESEXTERNAS.DataUltAct", null),
            new SqlColumn("ENTIDADESEXTERNAS.Telemovel", null),
            new SqlColumn("ENTIDADESEXTERNAS.Distrito", null),
            new SqlColumn("ENTIDADESEXTERNAS.Morada", null),
            new SqlColumn("ENTIDADESEXTERNAS.Pais", null)            
        };

        private static SqlColumn[] sqlColumnsReference =
        {
            new SqlColumn("ENTIDADESEXTERNAS.Entidade", null),
            new SqlColumn("ENTIDADESEXTERNAS.Nome", null),
        };

        private static LeadInfo GenerateFull(StdBELista queryObject)
        {
            return new LeadInfo()
            {
                Identficador = TypeParser.String(queryObject.Valor("Entidade")),
                Activo = TypeParser.Boolean(queryObject.Valor("Activo")),
                Nome = TypeParser.String(queryObject.Valor("Nome")),
                Email = TypeParser.String(queryObject.Valor("Email")),
                Telefone = TypeParser.String(queryObject.Valor("Telefone")),
                DataCriacao = TypeParser.Date(queryObject.Valor("DataCriacao")),
                ModificadoEm = TypeParser.Date(queryObject.Valor("DataUltAct")),
                Telemovel = TypeParser.String(queryObject.Valor("Telemovel")),

                Localizacao = new Address
                {
                    CodigoPostal = TypeParser.String(queryObject.Valor("CodPostal")),
                    Distrito = TypeParser.String(queryObject.Valor("Distrito")),
                    Localidade = TypeParser.String(queryObject.Valor("Localidade")),
                    Morada = TypeParser.String(queryObject.Valor("Morada")),
                    Pais = TypeParser.String(queryObject.Valor("Pais"))
                },
            };
        }

        private static LeadListing GenerateListing(StdBELista queryObject)
        {
            return new LeadListing()
            {
                Identificador = TypeParser.String(queryObject.Valor("Entidade")),
                Activo = TypeParser.Boolean(queryObject.Valor("Activo")),
                Nome = TypeParser.String(queryObject.Valor("Nome")),
                Email = TypeParser.String(queryObject.Valor("Email")),
                ModificadoEm = TypeParser.Date(queryObject.Valor("DataUltAct")),
                Telemovel = TypeParser.String(queryObject.Valor("Telemovel")),
                Distrito = TypeParser.String(queryObject.Valor("Distrito")),
                Localizacao = TypeParser.String(queryObject.Valor("Morada")),
                Pais = TypeParser.String(queryObject.Valor("Pais"))
            };
        }

        private static EntityReference GenerateReference(StdBELista queryObject)
        {
            return new EntityReference(
                TypeParser.String(queryObject.Valor("Entidade")),
                EntityType.Lead,
                TypeParser.String(queryObject.Valor("Nome"))
            );
        }

        public static List<LeadListing> List(string leadId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<LeadListing>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ENTIDADESEXTERNAS")
                .Columns(sqlColumnsListing)
                .Where("PotencialCliente", Comparison.Equals, "TRUE")
                .Where(new WhereClause("Vendedor", Comparison.Equals, leadId).AddClause(LogicOperator.Or, Comparison.Equals, null)));

            while (!queryObject.NoFim())
            {
                queryResult.Add(GenerateListing(queryObject));
                queryObject.Seguinte();
            }

            queryResult.Sort(delegate(LeadListing lhs, LeadListing rhs)
            {
                if (lhs.Identificador == null || rhs.Identificador == null)
                {
                    return -1;
                }

                return lhs.Identificador.CompareTo(rhs.Identificador);
            });

            return queryResult;
        }

        public static LeadInfo View(string sessionId, string leadId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.CRM.EntidadesExternas.Existe(leadId) == false)
            {
                return null;
            }

            return GenerateFull(PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ENTIDADESEXTERNAS")
                .Columns(sqlColumnsFull)
                .Where("Entidade", Comparison.Equals, leadId)
                .Where("PotencialCliente", Comparison.Equals, "TRUE")
                .Where(new WhereClause("Vendedor", Comparison.Equals, sessionId).AddClause(LogicOperator.Or, Comparison.Equals, null))));
        }

        public static EntityReference LeadReference(string leadId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.CRM.EntidadesExternas.Existe(leadId) == false)
            {
                throw new NotFoundException();
            }

            return GenerateReference(PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ENTIDADESEXTERNAS")
                .Columns(sqlColumnsReference)
                .Where("ENTIDADESEXTERNAS.Entidade", Comparison.Equals, leadId)
                .Where("PotencialCliente", Comparison.Equals, "TRUE")));
        }

        private static void SetAddress(CrmBEEntidadeExterna leadInfo, Address jsonObject)
        {
            if (jsonObject.Morada != null)
                leadInfo.set_Morada(jsonObject.Morada);
            if (jsonObject.Distrito != null)
                leadInfo.set_Distrito(jsonObject.Distrito);
            if (jsonObject.Localidade != null)
                leadInfo.set_Localidade(jsonObject.Localidade);
            if (jsonObject.CodigoPostal != null)
                leadInfo.set_CodPostal(jsonObject.CodigoPostal);
            if (jsonObject.Pais != null)
                leadInfo.set_Pais(jsonObject.Pais);
        }

        private static void SetFields(CrmBEEntidadeExterna leadInfo, Lead jsonObject)
        {
            if (jsonObject.Nome != null)
                leadInfo.set_Nome(jsonObject.Nome);
            if (jsonObject.Email != null)
                leadInfo.set_Email(jsonObject.Email);
            if (jsonObject.Telefone != null)
                leadInfo.set_Telefone(jsonObject.Telefone);
            if (jsonObject.Telefone2 != null)
                leadInfo.set_Telefone2(jsonObject.Telefone2);
            if (jsonObject.Telemovel != null)
                leadInfo.set_Telemovel(jsonObject.Telemovel);
            if (jsonObject.Localizacao != null)
                SetAddress(leadInfo, jsonObject.Localizacao);
        }

        public static bool Update(string sessionId, string leadId, Lead jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var leadsTable = PrimaveraEngine.Engine.CRM.EntidadesExternas;

            if (leadsTable.Existe(leadId) == false)
            {
                return false;
            }

            var leadInfo = leadsTable.Edita(leadId);

            leadInfo.set_EmModoEdicao(true);
            leadInfo.set_DataUltAct(DateTime.Now);
            SetFields(leadInfo, jsonObject);
            leadsTable.Actualiza(leadInfo);

            return true;
        }

        public static bool Insert(string sessionId, Lead jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var leadId = PrimaveraEngine.GenerateHash();
            var leadsTable = PrimaveraEngine.Engine.CRM.EntidadesExternas;
            var leadInfo = leadsTable.PreencheCamposDefeito(new CrmBEEntidadeExterna());

            if (leadsTable.Existe(leadId))
            {
                return false;
            }

            var serverTime = DateTime.Now;

            leadInfo.set_Activo(true);
            leadInfo.set_Entidade(leadId);
            leadInfo.set_Vendedor(sessionId);
            leadInfo.set_DataCriacao(serverTime);
            leadInfo.set_DataUltAct(serverTime);
            leadInfo.set_PotencialCliente(true);
            SetFields(leadInfo, jsonObject);
            leadsTable.Actualiza(leadInfo);

            return true;
        }

        public static bool Delete(string sessionId, string leadId)
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