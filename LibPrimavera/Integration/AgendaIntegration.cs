using System;
using System.Collections.Generic;

using Interop.CrmBE900;
using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class AgendaIntegration
    {
        private static bool CheckPermissions(CrmBEActividade activityInfo, string sessionId)
        {
            /*if (activityInfo.get_Estado().Equals("1"))
            {
                return false;
            }

            var representativeId = activityInfo.get_CriadoPor();

            if (representativeId != null && representativeId != sessionId)
            {
                return false;
            }*/

            return true;
        }

        private static SqlColumn[] sqlColumns =
        {
            new SqlColumn("TAREFAS.Id", null),
            new SqlColumn("TAREFAS.Resumo", null),
            new SqlColumn("TAREFAS.Estado", null),
            new SqlColumn("TAREFAS.Prioridade", null),
            new SqlColumn("TAREFAS.TipoEntidadePrincipal", null),
            new SqlColumn("TAREFAS.EntidadePrincipal", null),  
            new SqlColumn("TAREFAS.DataFim", null),     
            new SqlColumn("TAREFAS.DataInicio", null),
            new SqlColumn("TAREFAS.DataUltAct", null),
            new SqlColumn("TIPOSTAREFA.TipoActividade", null),
            new SqlColumn("TIPOSTAREFA.Descricao", "DescricaoActividade"),
        };

        private static ActivityListing GenerateListing(StdBELista queryResult)
        {
            var entityId = TypeParser.String(queryResult.Valor("EntidadePrincipal"));
            var entityType = TypeParser.String(queryResult.Valor("TipoEntidadePrincipal"));

            return new ActivityListing
            {
                Tipo = TypeReference(queryResult),
                Entidade = EntityReference(entityId, entityType),
                DataFim = TypeParser.Date(queryResult.Valor("DataFim")),
                Resumo = TypeParser.String(queryResult.Valor("Resumo")),
                Estado = TypeParser.Integer(queryResult.Valor("Estado")),
                Identificador = TypeParser.String(queryResult.Valor("Id")),
                DataInicio = TypeParser.Date(queryResult.Valor("DataInicio")),
                Prioridade = TypeParser.Integer(queryResult.Valor("Prioridade")),
                DataModificacao = TypeParser.Date(queryResult.Valor("DataUltAct"))
            };
        }

        public static List<ActivityListing> List(string sessionId, SqlBuilder sqlBuilder)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<ActivityListing>();
            var queryObject = PrimaveraEngine.Consulta(sqlBuilder);

            while (!queryObject.NoFim())
            {
                queryResult.Add(GenerateListing(queryObject));
                queryObject.Seguinte();
            }

            return queryResult;
        }

        public static List<ActivityListing> ListActive(string sessionId)
        {
            return List(sessionId, new SqlBuilder()
                .FromTable("TAREFAS")
                .Columns(sqlColumns)
                .LeftJoin("TIPOSTAREFA", "Id", Comparison.Equals, "TAREFAS", "IdTipoActividade")
                .Where(new WhereClause("Estado", Comparison.Equals, 0).AddClause(LogicOperator.Or, Comparison.Equals, null))
                .Where(new WhereClause("DataFim", Comparison.GreaterOrEquals, DateTime.Now.ToString("yyyy-MM-dd HH:mm"))));
        }

        public static List<ActivityListing> ListInactive(string sessionId)
        {
            return List(sessionId, new SqlBuilder()
                .FromTable("TAREFAS")
                .Columns(sqlColumns)
                .LeftJoin("TIPOSTAREFA", "Id", Comparison.Equals, "TAREFAS", "IdTipoActividade")
                .Where(new WhereClause("Estado", Comparison.Equals, 1), 1)
                .Where(new WhereClause("DataFim", Comparison.LessThan, DateTime.Now.ToString("yyyy-MM-dd HH:mm")), 2));
        }

        private static Reference TypeReference(StdBELista queryResult)
        {
            return new Reference(TypeParser.String(queryResult.Valor("TipoActividade")), TypeParser.String(queryResult.Valor("DescricaoActividade")));
        }

        public static Reference TypeReference(string typeId)
        {
            if (string.IsNullOrEmpty(typeId))
            {
                return null;
            }

            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var typeInfo = PrimaveraEngine.Engine.CRM.TiposActividade.EditaID(typeId);
            return new Reference(typeInfo.get_TipoActividade(), typeInfo.get_Descricao());
        }

        private static EntityReference EntityReference(string entityId, string entityType)
        {
            if (string.IsNullOrEmpty(entityType))
            {
                return null;
            }

            switch (entityType)
            {
                case "L":
                    return LeadIntegration.Reference(entityId);
                case "C":
                    return CustomerIntegration.Reference(entityId);
                case "X":
                    return ContactIntegration.Reference(entityId);
            }

            return null;
        }

        private static void SetFields(CrmBEActividade activityInfo, Activity jsonObject)
        {
            activityInfo.set_Resumo(jsonObject.Resumo);
            activityInfo.set_Descricao(jsonObject.Descricao);
            activityInfo.set_IDTipoActividade(jsonObject.Tipo);
            activityInfo.set_Prioridade(jsonObject.Prioridade.ToString());

            if (jsonObject.TipoEntidade == null)
            {
                activityInfo.set_TipoEntidadePrincipal(null);
                activityInfo.set_EntidadePrincipal(null);
            }
            else
            {
                activityInfo.set_TipoEntidadePrincipal(jsonObject.TipoEntidade);
                activityInfo.set_EntidadePrincipal(jsonObject.Entidade);
            }

            if (jsonObject.DataInicio != null)
            {
                activityInfo.set_DataInicio(jsonObject.DataInicio);
                activityInfo.set_DataFim(jsonObject.DataFim);
            }
        }

        public static ActivityInfo View(string sessionId, string activityId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var activitiesTable = PrimaveraEngine.Engine.CRM.Actividades;

            if (activitiesTable.Existe(activityId) == false)
            {
                throw new NotFoundException("actividade", true);
            }

            var activityInfo = activitiesTable.Edita(activityId);

            /*if (activityInfo.get_CriadoPor() != activityId)
            {
                return null;
            }*/

            return GenerateActivity(activityInfo);
        }

        private static ActivityInfo GenerateActivity(CrmBEActividade activityInfo)
        {
            var entityId = activityInfo.get_EntidadePrincipal();
            var entityType = activityInfo.get_TipoEntidadePrincipal();

            return new ActivityInfo
            {
                Resumo = activityInfo.get_Resumo(),
                Duracao = activityInfo.get_Duracao(),
                Identificador = activityInfo.get_ID(),
                Descricao = activityInfo.get_Descricao(),
                DataCriacao = activityInfo.get_DataCriacao(),
                DataModificacao = activityInfo.get_DataUltAct(),
                DataFim = activityInfo.get_DataFim(),
                DataInicio = activityInfo.get_DataInicio(),
                Entidade = EntityReference(entityId, entityType),
                Prioridade = Convert.ToInt32(activityInfo.get_Prioridade()),
                Estado = Convert.ToInt32(activityInfo.get_Estado()),
                Tipo = TypeReference(activityInfo.get_IDTipoActividade()),
                Responsavel = UserIntegration.Reference(activityInfo.get_CriadoPor()),
            };
        }

        public static ActivityInfo Update(string sessionId, string activityId, Activity jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var activitiesTable = PrimaveraEngine.Engine.CRM.Actividades;

            if (activitiesTable.Existe(activityId) == false)
            {
                throw new NotFoundException("actividade", true);
            }

            var activityInfo = activitiesTable.Edita(activityId);

            if (CheckPermissions(activityInfo, sessionId) == false)
            {
                return null;
            }

            activityInfo.set_EmModoEdicao(true);
            SetFields(activityInfo, jsonObject);
            activityInfo.set_Duracao(jsonObject.DataFim.Subtract(jsonObject.DataInicio).Minutes);
            activityInfo.set_DataUltAct(DateTime.Now);
            activitiesTable.Actualiza(activityInfo);

            return GenerateActivity(activityInfo);
        }

        public static ActivityListing Insert(string sessionId, Activity jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var activityInfo = new CrmBEActividade();
            var activityId = PrimaveraEngine.generateGUID();
            var activitiesTable = PrimaveraEngine.Engine.CRM.Actividades;

            if (activitiesTable.Existe(activityId))
            {
                throw new EntityExistsException("actividade", true);
            }

            System.Diagnostics.Debug.Print(jsonObject.DataInicio.ToString());

            activityInfo.set_ID(activityId);
            activityInfo.set_Estado("0");
            activityInfo.set_CriadoPor(sessionId);
            activityInfo.set_DataCriacao(DateTime.Now);
            activityInfo.set_DataUltAct(DateTime.Now);
            SetFields(activityInfo, jsonObject);
            activityInfo.set_Duracao(jsonObject.DataFim.Subtract(jsonObject.DataInicio).Minutes);
            activityInfo = activitiesTable.PreencheDadosRelacionados(activityInfo);
            activitiesTable.Actualiza(activityInfo);

            var entityId = activityInfo.get_EntidadePrincipal();
            var entityType = activityInfo.get_TipoEntidadePrincipal();

            return new ActivityListing
            {
                Resumo = activityInfo.get_Resumo(),
                Identificador = activityInfo.get_ID(),
                DataFim = activityInfo.get_DataFim(),
                DataInicio = activityInfo.get_DataInicio(),
                DataModificacao = activityInfo.get_DataUltAct(),
                Entidade = EntityReference(entityId, entityType),
                Estado = Convert.ToInt32(activityInfo.get_Estado()),
                Prioridade = Convert.ToInt32(activityInfo.get_Prioridade()),
                Tipo = TypeReference(activityInfo.get_IDTipoActividade())
            };
        }

        public static ActivityInfo Delete(string sessionId, string activityId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var activitiesTable = PrimaveraEngine.Engine.CRM.Actividades;

            if (activitiesTable.Existe(activityId) == false)
            {
                throw new NotFoundException("actividade", true);
            }

            var activityInfo = activitiesTable.Edita(activityId);

            if (CheckPermissions(activityInfo, sessionId) == false)
            {
                return null;
            }

            activityInfo.set_EmModoEdicao(true);
            activityInfo.set_Estado("1");
            activityInfo.set_DataUltAct(DateTime.Now);
            activitiesTable.Actualiza(activityInfo);

            return GenerateActivity(activityInfo);
        }
    }
}