namespace FirstREST.Lib_Primavera.Model
{
    public enum Agenda
    {
        Today, Yesterday, Past, Future, Week, Month, Year
    }

    public enum AgendaType
    {
        Everything, Tasks, Visits, Meetings, Calls
    }

    public enum AgendaStatus
    {
        Ongoing = 0,
        Completed = 1,
        Cancelled = 2
    }
}