using System.ComponentModel;

namespace FirstREST.LibPrimavera.Model
{
    public enum EntityType
    {
        [Description("Desconhecido")]
        N = -1,
        [Description("Cliente")]
        C,
        [Description("Lead")]
        X,
        [Description("Contacto")]
        O
    }
}