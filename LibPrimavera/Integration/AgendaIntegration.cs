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
            if (activityInfo.get_Estado() == null)
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

        public static List<Activity> List(string sessionId, ActivityType agendaType, ActivityStatus agendaStatus, ActivityInterval agendaWhen)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Activity>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("TAREFAS")
                .Columns(sqlColumns)
                .Where(FilterDate(agendaWhen))
                .LeftJoin("TIPOSTAREFA", "Id", Comparison.Equals, "TAREFAS", "IdTipoActividade"));
            /*  .Where(FilterStatus(agendaStatus))
              .Where(FilterType(agendaType)));
              */
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
                Tipo = TypeParser.String(queryResult.Valor("TipoActividade")),
                Identificador = TypeParser.String(queryResult.Valor("Id")),
                Resumo = TypeParser.String(queryResult.Valor("Resumo")),
                Responsavel = TypeParser.String(queryResult.Valor("CriadoPor")),
                DataInicio = TypeParser.Date(queryResult.Valor("DataInicio")),
                DataFim = TypeParser.Date(queryResult.Valor("DataFim")),
                Estado = (ActivityStatus)TypeParser.Integer(queryResult.Valor("Estado")),
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

            throw new NotFoundException();
        }

        private static DateTime GetWeek()
        {
            var ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            var fdow = ci.DateTimeFormat.FirstDayOfWeek;
            var today = DateTime.Now.DayOfWeek;
            return DateTime.Now.AddDays(-(today - fdow)).Date;
        }

        private static WhereClause FilterDate(ActivityInterval agendaWhen)
        {
            var startInterval = DateTime.Today;
            var endInterval = startInterval.AddHours(24);

            if (agendaWhen == ActivityInterval.Yesterday)
            {
                startInterval = startInterval.AddDays(-1);
                endInterval = startInterval.AddHours(24);
            }
            else if (agendaWhen == ActivityInterval.Tomorrow)
            {
                startInterval = startInterval.AddDays(1);
                endInterval = startInterval.AddHours(24);
            }
            else if (agendaWhen == ActivityInterval.Week)
            {
                startInterval = GetWeek();
                endInterval = startInterval.AddDays(7);
            }
            else if (agendaWhen == ActivityInterval.Month)
            {
                startInterval = new DateTime(startInterval.Year, startInterval.Month, 1);
                endInterval = startInterval.AddMonths(1);
            }
            else if (agendaWhen == ActivityInterval.Year)
            {
                startInterval = new DateTime(startInterval.Year, 1, 1);
                endInterval = startInterval.AddYears(1);
            }
            else if (agendaWhen == ActivityInterval.Past)
            {
                startInterval = MinimumValue;
                endInterval = DateTime.Now;
            }
            else if (agendaWhen == ActivityInterval.Future)
            {
                startInterval = DateTime.Now;
                endInterval = MaximumValue;
            }

            return FilterDate(startInterval, endInterval);
        }

        private static WhereClause FilterDate(DateTime startInterval, DateTime endInterval)
        {
            if (startInterval == MinimumValue)
            {
                return new WhereClause("DataInicio", Comparison.LessThan, TypeParser.ToString(endInterval));
            }

            if (endInterval == MaximumValue)
            {
                return new WhereClause("DataInicio", Comparison.GreaterThan, TypeParser.ToString(startInterval));
            }

            return new WhereClause("DataInicio", Comparison.GreaterOrEquals, TypeParser.ToString(startInterval)).AddClause(LogicOperator.And, Comparison.LessThan, TypeParser.ToString(endInterval));
        }

        private static DateTime MinimumValue = DateTime.MinValue;
        private static DateTime MaximumValue = DateTime.MaxValue;

        private static WhereClause FilterStatus(ActivityStatus agendaStatus)
        {
            return new WhereClause("Estado", Comparison.Equals, agendaStatus);
        }

        private static WhereClause FilterStatus(ActivityStatus[] agendaStatus)
        {
            WhereClause wc = null;
            Dictionary<ActivityStatus, bool> duplicateCheck = new Dictionary<ActivityStatus, bool>();

            foreach (var activityStatus in agendaStatus)
            {
                if (duplicateCheck.ContainsKey(activityStatus))
                {
                    continue;
                }

                if (wc == null)
                {
                    wc = new WhereClause("Estado", Comparison.Equals, activityStatus);
                }
                else
                {
                    wc.AddClause(LogicOperator.Or, Comparison.Equals, activityStatus);
                }

                duplicateCheck.Add(activityStatus, true);
            }

            return wc;
        }

        public static WhereClause FilterType(ActivityType agendaType)
        {
            return new WhereClause("IdTipoActividade", Comparison.Equals, agendaType);
        }

        public static WhereClause FilterType(ActivityType[] agendaType)
        {
            WhereClause wc = null;
            Dictionary<ActivityType, bool> duplicateCheck = new Dictionary<ActivityType, bool>();

            foreach (var activityType in agendaType)
            {
                if (duplicateCheck.ContainsKey(activityType))
                {
                    continue;
                }

                if (wc == null)
                {
                    wc = new WhereClause("IdTipoActividade", Comparison.Equals, activityType);
                }
                else
                {
                    wc.AddClause(LogicOperator.Or, Comparison.Equals, activityType);
                }

                duplicateCheck.Add(activityType, true);
            }

            return wc;
        }

        private static void SetFields(CrmBEActividade activityInfo, Activity jsonObject)
        {
            if (jsonObject.Resumo != null)
                activityInfo.set_Resumo(jsonObject.Resumo);
            if (jsonObject.Descricao != null)
                activityInfo.set_Descricao(jsonObject.Descricao);
            if (jsonObject.Estado != ActivityStatus.Any)
                activityInfo.set_Estado(jsonObject.Estado.ToString());
            if (jsonObject.Tipo != ActivityType.ANY)
                activityInfo.set_IDTipoActividade(jsonObject.Tipo.ToString());
            if (jsonObject.Prioridade != null)
                activityInfo.set_Prioridade(jsonObject.Prioridade.ToString());
            if (jsonObject.TipoEntidade != null)
                activityInfo.set_TipoEntidadePrincipal(jsonObject.TipoEntidade);
            if (jsonObject.Entidade != null)
                activityInfo.set_EntidadePrincipal(jsonObject.Entidade);
            if (jsonObject.DataInicio != null)
                activityInfo.set_DataInicio(jsonObject.DataInicio);
            if (jsonObject.DataFim != null)
                activityInfo.set_DataFim(jsonObject.DataFim);
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
                return null;
            }

            var activityInfo = activitiesTable.Edita(activityId);

            if (activityInfo.get_CriadoPor() != activityId)
            {
                return null;
            }

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
                Prioridade = activityInfo.get_Prioridade(),
                DataInicio = activityInfo.get_DataInicio(),
                Duracao = activityInfo.get_Duracao(),
                Estado = TypeParser.Activity_Status(activityInfo.get_Estado()),
                Tipo = TypeParser.Activity_Type(activityInfo.get_IDTipoActividade())
            };
        }

        public static bool Update(string sessionId, string activityId, Activity jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var activitiesTable = PrimaveraEngine.Engine.CRM.Actividades;

            if (activitiesTable.Existe(activityId) == false)
            {
                return false;
            }

            var activityInfo = activitiesTable.Edita(activityId);

            if (CheckPermissions(activityInfo, sessionId) == false)
            {
                return false;
            }

            activityInfo.set_EmModoEdicao(true);
            SetFields(activityInfo, jsonObject);
            activityInfo.set_DataUltAct(DateTime.Now);
            activitiesTable.Actualiza(activityInfo);

            return true;
        }

        public static bool Insert(string sessionId, Activity jsonObject)
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
                return false;
            }

            activityInfo.set_ID(activityId);
            activityInfo.set_CriadoPor(sessionId);
            activityInfo.set_DataCriacao(DateTime.Now);
            activityInfo.set_DataUltAct(DateTime.Now);
            SetFields(activityInfo, jsonObject);
            activityInfo = activitiesTable.PreencheDadosRelacionados(activityInfo);
            activitiesTable.Actualiza(activityInfo);

            return true;
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
                return false;
            }

            var activityInfo = activitiesTable.Edita(activityId);

            if (CheckPermissions(activityInfo, sessionId) == false)
            {
                return false;
            }

            activityInfo.set_EmModoEdicao(true);
            activityInfo.set_Estado(null);
            activityInfo.set_DataUltAct(DateTime.Now);
            activitiesTable.Actualiza(activityInfo);

            return true;
        }
    }
}