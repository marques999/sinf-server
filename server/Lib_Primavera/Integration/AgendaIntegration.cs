using System;
using System.Collections.Generic;

using Interop.CrmBE900;
using Interop.StdBE900;

using FirstREST.Lib_Primavera.Model;
using FirstREST.Lib_Primavera.Enums;

namespace FirstREST.Lib_Primavera.Integration
{
    public class AgendaIntegration
    {
        public static List<Activity> Get(AgendaType agendaType, AgendaStatus agendaStatus, Agenda agendaWhen)
        {
            var queryResult = new List<Activity>();
            var queryObject = PriEngine.Consulta(new QueryBuilder()
                .FromTable("TAREFAS"));
            /*      .Where(FilterDate(agendaWhen))
                  .Where(FilterStatus(agendaStatus))
                  .Where(FilterType(agendaType)));
                  */
            while (!queryObject.NoFim())
            {
                queryResult.Add(Generate(queryObject));
                queryObject.Seguinte();
            }

            return queryResult;
        }

        private static Activity Generate(StdBELista queryResult)
        {
            return new Activity
            {
                Identifier = TypeParser.String(queryResult.Valor("Id")),
                DateCreated = TypeParser.Date(queryResult.Valor("DataCriacao")),
                DateModified = TypeParser.Date(queryResult.Valor("DataUltAct")),
                Description = TypeParser.String(queryResult.Valor("Descricao")),
                End = TypeParser.Date(queryResult.Valor("DataFim")),
                Start = TypeParser.Date(queryResult.Valor("DataInicio")),
            };
        }

        private static DateTime GetWeek()
        {
            var ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            var fdow = ci.DateTimeFormat.FirstDayOfWeek;
            var today = DateTime.Now.DayOfWeek;
            return DateTime.Now.AddDays(-(today - fdow)).Date;
        }

        private static WhereClause FilterDate(Agenda agendaWhen)
        {
            var startInterval = DateTime.Today;
            var endInterval = startInterval.AddHours(24);

            if (agendaWhen == Agenda.Yesterday)
            {
                startInterval = startInterval.AddDays(-1);
                endInterval = startInterval.AddHours(24);
            }
            else if (agendaWhen == Agenda.Tomorrow)
            {
                startInterval = startInterval.AddDays(1);
                endInterval = startInterval.AddHours(24);
            }
            else if (agendaWhen == Agenda.Week)
            {
                startInterval = GetWeek();
                endInterval = startInterval.AddDays(7);
            }
            else if (agendaWhen == Agenda.Month)
            {
                startInterval = new DateTime(startInterval.Year, startInterval.Month, 1);
                endInterval = startInterval.AddMonths(1);
            }
            else if (agendaWhen == Agenda.Year)
            {
                startInterval = new DateTime(startInterval.Year, 1, 1);
                endInterval = startInterval.AddYears(1);
            }
            else if (agendaWhen == Agenda.Past)
            {
                startInterval = DateTime.MinValue;
            }
            else if (agendaWhen == Agenda.Future)
            {
                endInterval = DateTime.MaxValue;
            }

            return new WhereClause("DataInicio", Comparison.GreaterOrEquals, startInterval.ToString()).AddClause(LogicOperator.And, Comparison.LessThan, endInterval.ToString());
        }

        private static WhereClause FilterStatus(AgendaStatus agendaStatus)
        {
            return new WhereClause("Estado", Comparison.Equals, agendaStatus);
        }

        private static WhereClause FilterStatus(AgendaStatus[] agendaStatus)
        {
            WhereClause wc = null;
            Dictionary<AgendaStatus, bool> duplicateCheck = new Dictionary<AgendaStatus, bool>();

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

        public static WhereClause FilterType(AgendaType agendaType)
        {
            return new WhereClause("IdTipoActividade", Comparison.Equals, agendaType);
        }

        public static WhereClause FilterType(AgendaType[] agendaType)
        {
            WhereClause wc = null;
            Dictionary<AgendaType, bool> duplicateCheck = new Dictionary<AgendaType, bool>();

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
            if (paramObject.Description != null)
            {
                selectedRow.set_Descricao(paramObject.Description);
            }

            if (paramObject.Status != AgendaStatus.Any)
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

        public static bool UpdateActivity(string paramId, Activity paramObject)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var selectedTable = PriEngine.Engine.CRM.Actividades;

            if (selectedTable.Existe(paramId) == false)
            {
                return false;
            }

            var selectedRow = selectedTable.Edita(paramId);

            selectedRow.set_EmModoEdicao(true);
            SetFields(selectedRow, paramObject);
            selectedTable.Actualiza(selectedRow);

            return true;
        }

        public static bool CreateActivity(string paramId, Activity paramObject)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var selectedRow = new CrmBEActividade();
            var selectedTable = PriEngine.Engine.CRM.Actividades;

            if (selectedTable.Existe(paramId))
            {
                return false;
            }

            selectedRow.set_ID(paramId);
            SetFields(selectedRow, paramObject);
            selectedTable.Actualiza(selectedRow);

            return true;
        }
    }
}