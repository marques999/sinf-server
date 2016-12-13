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
            new SqlColumn("Email", null),
            new SqlColumn("Activo", null),
            new SqlColumn("TipoTerceiro", null),    
            new SqlColumn("DataCriacao", null), 
            new SqlColumn("DataUltAct", null),
            new SqlColumn("Telemovel", null)
        };

        private static LeadListing GenerateListing(StdBELista queryObject)
        {
            return new LeadListing()
            {
                Identificador = TypeParser.String(queryObject.Valor("Entidade")),
                Nome = TypeParser.String(queryObject.Valor("Nome")),
                Email = TypeParser.String(queryObject.Valor("Email")),
                Activo = TypeParser.Boolean(queryObject.Valor("Activo")),
                TipoTerceiro = TypeParser.String(queryObject.Valor("TipoTerceiro")),
                DataCriacao = TypeParser.Date(queryObject.Valor("DataCriacao")),
                DataModificacao = TypeParser.Date(queryObject.Valor("DataUltAct")),
                Telefone = TypeParser.String(queryObject.Valor("Telemovel"))
            };
        }

        private static SqlColumn[] sqlColumnsReference =
        {
            new SqlColumn("Entidade", null),
            new SqlColumn("Nome", null),
        };

        private static EntityReference GenerateReference(StdBELista queryObject)
        {
            return new EntityReference
            {
                Tipo = EntityType.Lead.ToDescriptionString(),
                Identificador = TypeParser.String(queryObject.Valor("Entidade")),
                Descricao = TypeParser.String(queryObject.Valor("Nome"))
            };
        }

        private static bool CheckPermissions(CrmBEEntidadeExterna leadInfo, string sessionId)
        {
            /*if (leadInfo.get_Activo() == false)
            {
                return false;
            }

            var representativeId = leadInfo.get_Vendedor();

            if (representativeId != null && representativeId != sessionId)
            {
                return false;
            }*/

            return true;
        }

        public static List<LeadListing> List(string sellsForceId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var leadList = new List<LeadListing>();
            var leadInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ENTIDADESEXTERNAS")
                .Columns(sqlColumnsListing)
                .Where("PotencialCliente", Comparison.Equals, "TRUE")
                .Where(new WhereClause("Vendedor", Comparison.Equals, sellsForceId).AddClause(LogicOperator.Or, Comparison.Equals, null)));

            if (leadInfo == null || leadInfo.Vazia())
            {
                return leadList;
            }

            while (!leadInfo.NoFim())
            {
                leadList.Add(GenerateListing(leadInfo));
                leadInfo.Seguinte();
            }

            leadList.Sort(delegate(LeadListing lhs, LeadListing rhs)
            {
                if (lhs.Identificador == null || rhs.Identificador == null)
                {
                    return -1;
                }

                return lhs.Identificador.CompareTo(rhs.Identificador);
            });

            return leadList;
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
                throw new NotFoundException("lead", true);
            }

            var leadInfo = leadsTable.Edita(leadId);
            var representativeId = leadInfo.get_Vendedor();

            /*if (representativeId != null && representativeId != sessionId)
              {
                  return null;
              }*/

            return GenerateInfo(leadInfo);
        }

        private static LeadInfo GenerateInfo(CrmBEEntidadeExterna leadInfo)
        {
            return new LeadInfo
            {
                Identificador = leadInfo.get_Entidade(),
                Activo = leadInfo.get_Activo(),
                Nome = leadInfo.get_Nome(),
                Email = leadInfo.get_Email(),
                Telefone = leadInfo.get_Telefone(),
                Telefone2 = leadInfo.get_Telefone2(),
                DataCriacao = leadInfo.get_DataCriacao(),
                DataModificacao = leadInfo.get_DataUltAct(),
                Telemovel = leadInfo.get_Telemovel(),
                EnderecoWeb = leadInfo.get_EnderecoWeb(),
                TipoMercado = leadInfo.get_TipoMercado(),
                PessoaSingular = leadInfo.get_PessoaSingular(),
                Idioma = leadInfo.get_Idioma(),
                Morada2 = leadInfo.get_Morada2(),
                NumContribuinte = leadInfo.get_NumContrib(),
                Zona = leadInfo.get_Zona(),
                TipoTerceiro = leadInfo.get_TipoTerceiro(),
                Responsavel = UserIntegration.Reference(leadInfo.get_Vendedor()),
                Localizacao = new Address
                {
                    Pais = leadInfo.get_Pais(),
                    Morada = leadInfo.get_Morada(),
                    Distrito = leadInfo.get_Distrito(),
                    Localidade = leadInfo.get_Localidade(),
                    CodigoPostal = leadInfo.get_CodPostal()
                },
            };
        }

        public static EntityReference Reference(string leadId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.CRM.EntidadesExternas.Existe(leadId) == false)
            {
                return null;
            }

            return GenerateReference(PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ENTIDADESEXTERNAS")
                .Columns(sqlColumnsReference)
                .Where("ENTIDADESEXTERNAS.Entidade", Comparison.Equals, leadId)
                .Where("PotencialCliente", Comparison.Equals, "TRUE")));
        }

        private static void SetFields(CrmBEEntidadeExterna leadInfo, Lead jsonObject)
        {
            leadInfo.set_Nome(jsonObject.Nome);
            leadInfo.set_Zona(jsonObject.Zona);
            leadInfo.set_Email(jsonObject.Email);
            leadInfo.set_Idioma(jsonObject.Idioma);
            leadInfo.set_Morada2(jsonObject.Morada2);
            leadInfo.set_Telefone(jsonObject.Telefone);
            leadInfo.set_Telefone2(jsonObject.Telefone2);
            leadInfo.set_Telemovel(jsonObject.Telemovel);
            leadInfo.set_Pais(jsonObject.Localizacao.Pais);
            leadInfo.set_EnderecoWeb(jsonObject.EnderecoWeb);
            leadInfo.set_Morada(jsonObject.Localizacao.Morada);
            leadInfo.set_NumContrib(jsonObject.NumContribuinte);
            leadInfo.set_PessoaSingular(jsonObject.PessoaSingular);
            leadInfo.set_CodPostal(jsonObject.Localizacao.CodigoPostal);

            if (jsonObject.TipoTerceiro != null)
            {
                leadInfo.set_TipoTerceiro(jsonObject.TipoTerceiro);
            }

            if (jsonObject.TipoMercado != null)
            {
                leadInfo.set_TipoMercado(jsonObject.TipoMercado);
            }

            if (jsonObject.Localizacao.Pais.Equals("PT"))
            {
                if (jsonObject.Localizacao.Distrito == null)
                {
                    leadInfo.set_Distrito(null);
                    leadInfo.set_Localidade(null);
                    leadInfo.set_CodPostalLocal(null);
                }
                else
                {
                    leadInfo.set_Distrito(jsonObject.Localizacao.Distrito);
                    leadInfo.set_Localidade(jsonObject.Localizacao.Localidade);
                    leadInfo.set_CodPostalLocal(jsonObject.Localizacao.Localidade);
                }
            }
            else
            {
                leadInfo.set_Distrito(null);
                leadInfo.set_Localidade(null);
                leadInfo.set_CodPostalLocal(null);
            }
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
                throw new NotFoundException("lead", true);
            }

            var leadInfo = leadsTable.Edita(leadId);

            if (CheckPermissions(leadInfo, sessionId) == false)
            {
                return false;
            }

            string clientId = null;
            bool convert2client = jsonObject.TipoTerceiro != null && jsonObject.TipoTerceiro.Equals(LeadInfo.CONVERT_TO_CLIENT_ID);

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

        public static LeadListing Insert(string sessionId, Lead jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var leadId = PrimaveraEngine.GenerateName(jsonObject.Nome);
            var leadsTable = PrimaveraEngine.Engine.CRM.EntidadesExternas;
            var leadInfo = leadsTable.PreencheCamposDefeito(new CrmBEEntidadeExterna());

            if (leadsTable.Existe(leadId))
            {
                throw new EntityExistsException("lead", true);
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

            return new LeadListing()
            {
                Identificador = leadInfo.get_Entidade(),
                Activo = leadInfo.get_Activo(),
                TipoTerceiro = leadInfo.get_TipoTerceiro(),
                Nome = leadInfo.get_Nome(),
                Email = leadInfo.get_Email(),
                DataModificacao = leadInfo.get_DataUltAct(),
                Telefone = leadInfo.get_Telemovel(),
                DataCriacao = leadInfo.get_DataCriacao()
            };
        }

        public static LeadInfo Delete(string sessionId, string leadId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var leadsTable = PrimaveraEngine.Engine.CRM.EntidadesExternas;

            if (leadsTable.Existe(leadId) == false)
            {
                throw new NotFoundException("lead", true);
            }

            var leadInfo = leadsTable.Edita(leadId);

            if (CheckPermissions(leadInfo, sessionId) == false)
            {
                return null;
            }

            leadInfo.set_EmModoEdicao(true);
            leadInfo.set_Activo(false);
            leadInfo.set_DataUltAct(DateTime.Now);
            leadsTable.Actualiza(leadInfo);

            return GenerateInfo(leadInfo);
        }
    }
}