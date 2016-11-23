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
        private static SqlColumn[] sqlColumnsListing =
        {
            new SqlColumn("Entidade", null),
            new SqlColumn("Nome", null),
            new SqlColumn("TipoTerceiro", null),
            new SqlColumn("Email", null),
            new SqlColumn("Activo", null),
            new SqlColumn("DataUltAct", null),
            new SqlColumn("Telemovel", null),
            new SqlColumn("Distrito", null),
            new SqlColumn("Morada", null),
            new SqlColumn("Pais", null)            
        };

        private static SqlColumn[] sqlColumnsReference =
        {
            new SqlColumn("Entidade", null),
            new SqlColumn("Nome", null),
        };

        private static LeadListing GenerateListing(StdBELista queryObject)
        {
            return new LeadListing()
            {
                Identificador = TypeParser.String(queryObject.Valor("Entidade")),
                Activo = TypeParser.Boolean(queryObject.Valor("Activo")),
                TipoTerceiro = TypeParser.String(queryObject.Valor("TipoTerceiro")),
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

        private static bool CheckPermissions(CrmBEEntidadeExterna leadInfo, string sessionId)
        {
            if (leadInfo.get_Activo() == false)
            {
                return false;
            }

            var representativeId = leadInfo.get_Vendedor();

            if (representativeId != null && representativeId != sessionId)
            {
                return false;
            }

            return true;
        }

        public static List<LeadListing> List(string sellsForceId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<LeadListing>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ENTIDADESEXTERNAS")
                .Columns(sqlColumnsListing)
                .Where("PotencialCliente", Comparison.Equals, "TRUE")
                .Where(new WhereClause("Vendedor", Comparison.Equals, sellsForceId).AddClause(LogicOperator.Or, Comparison.Equals, null)));

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
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var leadsTable = PrimaveraEngine.Engine.CRM.EntidadesExternas;

            if (leadsTable.Existe(leadId) == false)
            {
                return null;
            }

            var queryResult = leadsTable.Edita(leadId);
            var representativeId = queryResult.get_Vendedor();

            if (representativeId != null && representativeId != sessionId)
            {
                return null;
            }

            return new LeadInfo
            {
                Identificador = queryResult.get_Entidade(),
                Activo = queryResult.get_Activo(),
                Nome = queryResult.get_Nome(),
                Email = queryResult.get_Email(),
                Telefone = queryResult.get_Telefone(),
                Telefone2 = queryResult.get_Telefone2(),
                DataCriacao = queryResult.get_DataCriacao(),
                DataModificacao = queryResult.get_DataUltAct(),
                Telemovel = queryResult.get_Telemovel(),
                EnderecoWeb = queryResult.get_EnderecoWeb(),
                TipoMercado = queryResult.get_TipoMercado(),
                PessoaSingular = queryResult.get_PessoaSingular(),
                Idioma = queryResult.get_Idioma(),
                Morada2 = queryResult.get_Morada2(),
                NumContribuinte = queryResult.get_NumContrib(),
                Zona = queryResult.get_Zona(),
                TipoTerceiro = queryResult.get_TipoTerceiro(),
                Responsavel = UserIntegration.Reference(queryResult.get_Vendedor()),
                Localizacao = new Address
                {
                    Pais = queryResult.get_Pais(),
                    Morada = queryResult.get_Morada(),
                    Distrito = queryResult.get_Distrito(),
                    Localidade = queryResult.get_Localidade(),
                    CodigoPostal = queryResult.get_CodPostal()
                },
            };
        }

        public static EntityReference LeadReference(string leadId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
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
            if (jsonObject.Pais != null)
                leadInfo.set_Pais(jsonObject.Pais);
            if (jsonObject.Morada != null)
                leadInfo.set_Morada(jsonObject.Morada);
            if (jsonObject.Distrito != null)
                leadInfo.set_Distrito(jsonObject.Distrito);
            if (jsonObject.Localidade != null)
                leadInfo.set_Localidade(jsonObject.Localidade);
            if (jsonObject.CodigoPostal != null)
                leadInfo.set_CodPostal(jsonObject.CodigoPostal);
            if (jsonObject.Localidade != null)
                leadInfo.set_CodPostalLocal(jsonObject.Localidade);
        }

        private static void SetFields(CrmBEEntidadeExterna leadInfo, Lead jsonObject)
        {
            if (jsonObject.Nome != null)
                leadInfo.set_Nome(jsonObject.Nome);
            if (jsonObject.Email != null)
                leadInfo.set_Email(jsonObject.Email);
            if (jsonObject.Telefone != null)
                leadInfo.set_Telefone(jsonObject.Telefone);
            if (jsonObject.Telemovel != null)
                leadInfo.set_Telemovel(jsonObject.Telemovel);
            if (jsonObject.Localizacao != null)
                SetAddress(leadInfo, jsonObject.Localizacao);
            if (jsonObject.TipoTerceiro != null)
                leadInfo.set_TipoTerceiro(jsonObject.TipoTerceiro);
            if (jsonObject.TipoMercado != null)
                leadInfo.set_TipoMercado(jsonObject.TipoMercado);
            if (jsonObject.Telefone2 != null)
                leadInfo.set_Telefone2(jsonObject.Telefone2);
            if (jsonObject.EnderecoWeb != null)
                leadInfo.set_EnderecoWeb(jsonObject.EnderecoWeb);
            if (jsonObject.Morada2 != null)
                leadInfo.set_Morada2(jsonObject.Morada2);
            if (jsonObject.Zona != null)
                leadInfo.set_Zona(jsonObject.Zona);
            if (jsonObject.Idioma != null)
                leadInfo.set_Idioma(jsonObject.Idioma);
            if (jsonObject.NumContribuinte != null)
                leadInfo.set_NumContrib(jsonObject.NumContribuinte);
        }

        public static bool Update(string sessionId, string leadId, Lead jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var leadsTable = PrimaveraEngine.Engine.CRM.EntidadesExternas;

            if (leadsTable.Existe(leadId) == false)
            {
                return false;
            }

            var leadInfo = leadsTable.Edita(leadId);

            if (CheckPermissions(leadInfo, sessionId) == false)
            {
                return false;
            }

            string clientId = null;
            bool convert2client = jsonObject.TipoTerceiro.Equals(LeadInfo.CONVERT_TO_CLIENT_ID);

            if (convert2client)
            {
                //tem que ser feito nesta ordem para evitar alterar estado sem criar cliente
                var clientsTable = PrimaveraEngine.Engine.Comercial.Clientes;
                clientId = leadId;
                if (clientsTable.Existe(clientId))
                    clientId = PrimaveraEngine.GenerateHash();
                if (clientsTable.Existe(clientId))
                    return false;
            }

            leadInfo.set_EmModoEdicao(true);
            leadInfo.set_DataUltAct(DateTime.Now);
            SetFields(leadInfo, jsonObject);
            //LINE NOT NEEDED, will not copy that value! if (convert2client) leadInfo.set_TipoTerceiro(null);//remover tipo, pode nao aceite para o clientes
            leadsTable.Actualiza(leadInfo);

            if (convert2client)
            {
                PrimaveraEngine.Engine.CRM.EntidadesExternas
                    .TransformarNoutraEntidade(leadId,
                    clientId, "C", null, null, null);
            }

            return true;
        }

        public static bool Insert(string sessionId, Lead jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
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
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var leadsTable = PrimaveraEngine.Engine.CRM.EntidadesExternas;

            if (leadsTable.Existe(leadId) == false)
            {
                return false;
            }

            var leadInfo = leadsTable.Edita(leadId);

            if (CheckPermissions(leadInfo, sessionId) == false)
            {
                return false;
            }

            leadInfo.set_EmModoEdicao(true);
            leadInfo.set_Activo(false);
            leadInfo.set_DataUltAct(DateTime.Now);
            leadsTable.Actualiza(leadInfo);

            return true;
        }
    }
}
