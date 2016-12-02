using Newtonsoft.Json;

namespace FirstREST.LibPrimavera.Model
{
    public class EntityReference
    {
        public EntityReference()
        {
        }

        public EntityReference(string entityId, EntityType entityType, string entityName)
        {
            Tipo = entityType.ToDescriptionString();
            Identificador = entityId;
            Descricao = entityName;
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