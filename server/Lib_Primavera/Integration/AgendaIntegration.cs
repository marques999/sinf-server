using System.Collections.Generic;

using FirstREST.Lib_Primavera.Model;
using FirstREST.Lib_Primavera.Enums;

namespace FirstREST.Lib_Primavera.Integration
{
    public class AgendaIntegration
    {
        public static List<Activity> Get(AgendaType agendaType, AgendaStatus agendaStatus, Agenda agendaWhen)
        {
            return null;
        }

        private static WhereClause GetWhen(Agenda agendaWhen)
        {
            if (agendaWhen == Agenda.Yesterday)
            {
            }

            if (agendaWhen == Agenda.Week)
            {
            }

            if (agendaWhen == Agenda.Month)
            {
            }

            if (agendaWhen == Agenda.Year)
            {
            }

            if (agendaWhen == Agenda.Past)
            {
            }

            if (agendaWhen == Agenda.Future)
            {
            }

            return null;
        }

        private static WhereClause GetStatus(AgendaStatus[] agendaStatus)
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
                    wc = new WhereClause("status", Comparison.Equals, activityStatus);
                }
                else
                {
                    wc.AddClause(LogicOperator.Or, Comparison.Equals, activityStatus);
                }

                duplicateCheck.Add(activityStatus, true);
            }

            return wc;
        }

        public static WhereClause GetType(AgendaType[] agendaType)
        {
            WhereClause wc = null;
            Dictionary<AgendaType, bool> duplicateCheck = new Dictionary<AgendaType, bool>();

            foreach (var activityType in agendaType)
            {
                if (duplicateCheck.ContainsKey(activityType))
                {
                    continue;
                }

                if (activityType == AgendaType.Calls)
                {
                    if (wc == null)
                    {
                        wc = new WhereClause("type", Comparison.Equals, "call");
                    }
                    else
                    {
                        wc.AddClause(LogicOperator.Or, Comparison.Equals, "call");
                    }
                }
                else if (activityType == AgendaType.Meetings)
                {
                    if (wc == null)
                    {
                        wc = new WhereClause("type", Comparison.Equals, "meeting");
                    }
                    else
                    {
                        wc.AddClause(LogicOperator.Or, Comparison.Equals, "meeting");
                    }
                }
                else if (activityType == AgendaType.Tasks)
                {
                    if (wc == null)
                    {
                        wc = new WhereClause("type", Comparison.Equals, "task");
                    }
                    else
                    {
                        wc.AddClause(LogicOperator.Or, Comparison.Equals, "task");
                    }
                }
                else if (activityType == AgendaType.Visits)
                {
                    if (wc == null)
                    {
                        wc = new WhereClause("type", Comparison.Equals, "visit");
                    }
                    else
                    {
                        wc.AddClause(LogicOperator.Or, Comparison.Equals, "visit");
                    }
                }

                duplicateCheck.Add(activityType, true);
            }

            return wc;
        }
    }
}