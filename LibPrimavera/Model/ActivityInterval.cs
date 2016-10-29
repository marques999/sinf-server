using System.ComponentModel;

namespace FirstREST.LibPrimavera.Model
{
    [DefaultValue(Today)]
    public enum ActivityInterval
    {
        [Description("Hoje")]
        Today,
        [Description("Ontem")]
        Yesterday,
        [Description("Amanhã")]
        Tomorrow,
        [Description("Passado")]
        Past,
        [Description("Futuro")]
        Future,
        [Description("Semana")]
        Week,
        [Description("Mês")]
        Month,
        [Description("Ano")]
        Year
    }
}