namespace FirstREST.LibPrimavera.Model
{
    public enum Agenda
    {
        Today,
        Yesterday,
        Tomorrow,
        Past,
        Future,
        Week,
        Month,
        Year
    }

    public enum AgendaType
    {
        All,
        Tasks,
        Visits,
        Meetings,
        Calls
    }

    public enum AgendaStatus
    {
        Any = -1,
        Ongoing = 0,
        Completed = 1,
        Cancelled = 2
    }
}