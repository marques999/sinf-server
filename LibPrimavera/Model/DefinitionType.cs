using System.ComponentModel;

namespace FirstREST.LibPrimavera.Model
{
    public enum DefinitionType
    {
        [Description("Pais")]
        Country,
        [Description("Idioma")]
        Language,
        [Description("Titulo")]
        Title,
        [Description("TipoTerceiro")]
        ThirdParty,
        [Description("Zona")]
        Zone,
    };
}