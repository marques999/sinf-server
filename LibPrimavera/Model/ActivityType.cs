using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class ActivityType
    {
        public ActivityType(string identificadorActividade, string tipoActividade, string descricaoActividade)
        {
            Tipo = tipoActividade;
            Descricao = descricaoActividade;
            Identificador = identificadorActividade;
        }

        [JsonProperty(PropertyName = "id")]
        public string Identificador
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "type")]
        public string Tipo
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "name")]
        public string Descricao
        {
            get;
            set;
        }
    }
}