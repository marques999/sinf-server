using System.ComponentModel;

namespace FirstREST.LibPrimavera.Model
{
    [DefaultValue(ANY)]
    public enum ActivityType
    {
        [Description("Todos")]
        ANY = -1,
        [Description("Desconhecido")]
        UNKW,
        [Description("Compromisso")]
        COMP,
        [Description("Carta")]
        CARTA,
        [Description("Reunião")]
        REUN,
        [Description("E-mail")]
        EMAIL,
        [Description("Telefonema")]
        TEL,
        [Description("Cobrança")]
        COB,
        [Description("Tarefa")]
        TAR,
        [Description("Proposta")]
        PROP,
        [Description("Apresentação")]
        APRES
    }
}