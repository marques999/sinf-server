using System;
using System.Diagnostics;
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
        private static SqlColumn[] sqlColumns =
        {
            new SqlColumn("TAREFAS.Id", null),
            new SqlColumn("TAREFAS.Resumo", null),
            new SqlColumn("TAREFAS.Estado", null),
            new SqlColumn("TAREFAS.Descricao", null),
            new SqlColumn("TAREFAS.Prioridade", null),
            new SqlColumn("TAREFAS.CriadoPor", null),
            new SqlColumn("TAREFAS.TipoEntidadePrincipal", null),
            new SqlColumn("TAREFAS.EntidadePrincipal", null),       
            new SqlColumn("TAREFAS.DataInicio", null),
            new SqlColumn("TAREFAS.DataFim", null),
            new SqlColumn("TAREFAS.DataCriacao", null),
            new SqlColumn("TAREFAS.DataUltAct", null),
            new SqlColumn("TIPOSTAREFA.TipoActividade", null),
            new SqlColumn("TIPOSTAREFA.Descricao", "DescricaoActividade"),
        };

        private static bool CheckPermissions(CrmBEActividade activityInfo, string sessionId)
        {
            if (activityInfo.get_Estado().Equals("INACTIVO"))
            {
                return false;
            }

            var representativeId = activityInfo.get_CriadoPor();

            if (representativeId != null && representativeId != sessionId)
            {
                return false;
            }

            return true;
        }

        public static List<Activity> ListActive(string sessionId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Activity>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("TAREFAS")
                .Columns(sqlColumns)
                .LeftJoin("TIPOSTAREFA", "Id", Comparison.Equals, "TAREFAS", "IdTipoActividade")
                .Where(new WhereClause("Estado", Comparison.Equals, 0).AddClause(LogicOperator.Or, Comparison.Equals, null))
                .Where(new WhereClause("DataFim", Comparison.GreaterOrEquals, DateTime.Now)));

            while (!queryObject.NoFim())
            {
                queryResult.Add(Generate(queryObject));
                queryObject.Seguinte();
            }

            return queryResult;
        }

        public static List<Activity> ListInactive(string sessionId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Activity>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("TAREFAS")
                .Columns(sqlColumns)
                .LeftJoin("TIPOSTAREFA", "Id", Comparison.Equals, "TAREFAS", "IdTipoActividade")
                .Where(new WhereClause("Estado", Comparison.Equals, 1), 1)
                .Where(new WhereClause("DataFim", Comparison.LessThan, DateTime.Now), 2));

            while (!queryObject.NoFim())
            {
                queryResult.Add(Generate(queryObject));
                queryObject.Seguinte();
            }

            return queryResult;
        }

        private static Reference TypeReference(StdBELista queryResult)
        {
            return new Reference
            {
                Identificador = TypeParser.String(queryResult.Valor("TipoActividade")),
                Descricao = TypeParser.String(queryResult.Valor("DescricaoActividade"))
            };
        }

        private static ActivityInfo Generate(StdBELista queryResult)
        {
            var entityId = TypeParser.String(queryResult.Valor("EntidadePrincipal"));
            var entityType = TypeParser.String(queryResult.Valor("TipoEntidadePrincipal"));

            return new ActivityInfo
            {
                Tipo = TypeReference(queryResult),
                Identificador = TypeParser.String(queryResult.Valor("Id")),
                Resumo = TypeParser.String(queryResult.Valor("Resumo")),
                Responsavel = TypeParser.String(queryResult.Valor("CriadoPor")),
                DataInicio = TypeParser.Date(queryResult.Valor("DataInicio")),
                DataFim = TypeParser.Date(queryResult.Valor("DataFim")),
                Estado = TypeParser.Integer(queryResult.Valor("Estado")),
                Prioridade = TypeParser.Integer(queryResult.Valor("Prioridade")),
                Descricao = TypeParser.String(queryResult.Valor("Descricao")),
                CriadoEm = TypeParser.Date(queryResult.Valor("DataCriacao")),
                ModificadoEm = TypeParser.Date(queryResult.Valor("DataUltAct")),
                Entidade = EntityReference(entityId, entityType)
            };
        }

        private static EntityReference EntityReference(string entityId, string entityType)
        {
            if (string.IsNullOrEmpty(entityType) == false)
            {
                if (entityType == "X")
                {
                    return LeadIntegration.LeadReference(entityId);
                }

                if (entityType == "C")
                {
                    return CustomerIntegration.Reference(entityId);
                }

                if (entityType == "O")
                {
                    return ContactIntegration.Reference(entityId);
                }
            }

            return null;
        }

        private static void SetFields(CrmBEActividade activityInfo, Activity jsonObject)
        {
            activityInfo.set_Estado(jsonObject.Estado.ToString());
            activityInfo.set_Prioridade(jsonObject.Prioridade.ToString());
            if (jsonObject.Resumo != null)
                activityInfo.set_Resumo(jsonObject.Resumo);
            if (jsonObject.Descricao != null)
                activityInfo.set_Descricao(jsonObject.Descricao);
            if (jsonObject.Tipo != null)
                activityInfo.set_IDTipoActividade(jsonObject.Tipo.ToString());
            if (jsonObject.TipoEntidade != null)
                activityInfo.set_TipoEntidadePrincipal(jsonObject.TipoEntidade);
            if (jsonObject.Entidade != null)
                activityInfo.set_EntidadePrincipal(jsonObject.Entidade);
            if (jsonObject.DataInicio != null)
                activityInfo.set_DataInicio(jsonObject.DataInicio);
            if (jsonObject.DataFim != null)
                activityInfo.set_DataFim(jsonObject.DataFim);
        }

        public static Reference ActivityType(string typeId)
        {
            var activityTypes = PrimaveraEngine.Engine.CRM.TiposActividade.EditaID(typeId);
            return new Reference(activityTypes.get_TipoActividade(), activityTypes.get_Descricao());
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
                CriadoEm = activityInfo.get_DataCriacao(),
                ModificadoEm = activityInfo.get_DataUltAct(),
                Descricao = activityInfo.get_Descricao(),
                DataFim = activityInfo.get_DataFim(),
                Entidade = EntityReference(entityId, entityType),
                Identificador = activityInfo.get_ID(),
                Responsavel = activityInfo.get_Utilizador(),
                Prioridade = Convert.ToInt32(activityInfo.get_Prioridade()),
                DataInicio = activityInfo.get_DataInicio(),
                Duracao = activityInfo.get_Duracao(),
                Estado = Convert.ToInt32(activityInfo.get_Estado()),
                Resumo = activityInfo.get_Resumo(),
                Tipo = ActivityType(activityInfo.get_IDTipoActividade())
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
            activityInfo.set_DataUltAct(DateTime.Now);
            activitiesTable.Actualiza(activityInfo);

            return GenerateActivity(activityInfo);
        }

        public static ActivityInfo Insert(string sessionId, Activity jsonObject)
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

            activityInfo.set_ID(activityId);
            activityInfo.set_Estado("0");
            activityInfo.set_CriadoPor(sessionId);
            activityInfo.set_DataCriacao(DateTime.Now);
            activityInfo.set_DataUltAct(DateTime.Now);
            SetFields(activityInfo, jsonObject);
            activityInfo = activitiesTable.PreencheDadosRelacionados(activityInfo);
            activitiesTable.Actualiza(activityInfo);

            return GenerateActivity(activityInfo);
        }

        public static bool Delete(string sessionId, string activityId)
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
                return false;
            }

            activityInfo.set_EmModoEdicao(true);
            activityInfo.set_Estado("0");
            activityInfo.set_DataUltAct(DateTime.Now);
            activitiesTable.Actualiza(activityInfo);

            return true;
        }
    }
}