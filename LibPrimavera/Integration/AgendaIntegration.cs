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

        public static List<Activity> List(string sessionId, ActivityType agendaType, ActivityStatus agendaStatus, ActivityInterval agendaWhen)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
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
                Identifier = TypeParser.String(queryResult.Valor("TipoActividade")),
                Name = TypeParser.String(queryResult.Valor("DescricaoActividade"))
            };
        }

        private static Activity Generate(StdBELista queryResult)
        {
            var entityId = TypeParser.String(queryResult.Valor("EntidadePrincipal"));
            var entityType = TypeParser.String(queryResult.Valor("TipoEntidadePrincipal"));

            var newInstance = new Activity
            {
                Type = TypeReference(queryResult),
                Identifier = TypeParser.String(queryResult.Valor("Id")),
                Name = TypeParser.String(queryResult.Valor("Resumo")),
                Owner = TypeParser.String(queryResult.Valor("CriadoPor")),
                Start = TypeParser.Date(queryResult.Valor("DataInicio")),
                End = TypeParser.Date(queryResult.Valor("DataFim")),
                Status = (ActivityStatus)TypeParser.Integer(queryResult.Valor("Estado")),
                Priority = TypeParser.Integer(queryResult.Valor("Prioridade")),
                Description = TypeParser.String(queryResult.Valor("Descricao")),
                DateCreated = TypeParser.Date(queryResult.Valor("DataCriacao")),
                DateModified = TypeParser.Date(queryResult.Valor("DataUltAct")),
                EntityType = (int)TypeParser.Entity_Type(entityType)
            };

            if (string.IsNullOrEmpty(entityType) == false)
            {
                if (entityType == "X")
                {
                    newInstance.Entity = LeadIntegration.LeadReference(entityId);
                }
                else if (entityType == "C")
                {
                    newInstance.Entity = CustomerIntegration.Reference(entityId);
                }
                else if (entityType == "O")
                {
                    newInstance.Entity = ContactIntegration.Reference(entityId);
                }
            }

            return newInstance;
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

        private static void SetFields(CrmBEActividade selectedRow, Activity paramObject)
        {
            if (paramObject.Name != null)
            {
                selectedRow.set_Resumo(paramObject.Name.Trim());
            }

            if (paramObject.Description != null)
            {
                selectedRow.set_Descricao(paramObject.Description.Trim());
            }

            selectedRow.set_Prioridade(paramObject.Priority.ToString());

            if (paramObject.Status != ActivityStatus.Any)
            {
                selectedRow.set_Estado(paramObject.Status.ToString());
            }

            if (paramObject.DateModified != null)
            {
                selectedRow.set_DataUltAct(paramObject.DateModified);
            }

            if (paramObject.Start != null)
            {
                selectedRow.set_DataInicio(paramObject.Start);
            }

            if (paramObject.End != null)
            {
                selectedRow.set_DataFim(paramObject.End);
            }
        }

        public static bool Update(string paramId, Activity paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var tabelaActividades = PrimaveraEngine.Engine.CRM.Actividades;

            if (tabelaActividades.Existe(paramId) == false)
            {
                return false;
            }

            var mensagemErro = "";
            var linhaTabela = tabelaActividades.Edita(paramId);

            linhaTabela.set_EmModoEdicao(true);
            SetFields(linhaTabela, paramObject);
            linhaTabela = tabelaActividades.PreencheDadosRelacionados(linhaTabela);
            tabelaActividades.Actualiza(linhaTabela, ref mensagemErro);
            System.Diagnostics.Debug.Print(mensagemErro);
            
            return true;
        }

        public static bool Insert(string paramId, Activity paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var tabelaActividades = PrimaveraEngine.Engine.CRM.Actividades;

            if (tabelaActividades.Existe(paramId))
            {
                return false;
            }

            var mensagemErro = "";
            var linhaTabela = new CrmBEActividade();

            linhaTabela.set_ID(paramId);
            SetFields(linhaTabela, paramObject);
            linhaTabela = tabelaActividades.PreencheDadosRelacionados(linhaTabela);
            tabelaActividades.Actualiza(linhaTabela, ref mensagemErro);
            System.Diagnostics.Debug.Print(mensagemErro);
  
            return true;
        }

        public static bool Delete(string sessionId, string paramId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.CRM.Actividades.Existe(paramId))
            {
                PrimaveraEngine.Engine.CRM.Actividades.Remove(new Guid(paramId).ToString("D"));
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}