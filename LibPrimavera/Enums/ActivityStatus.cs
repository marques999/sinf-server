using System.ComponentModel;

namespace FirstREST.LibPrimavera.Model
{
    [DefaultValue(Pendente)]
    public enum EnumActivityStatus
    {
        [Description("Todos")]
        Any = -1,
        [Description("Pendente")]
        Pendente = 0,
        [Description("Terminada")]
        Terminada = 1,
        [Description("Cancelada")]
        Cancelada = 2
    }
}