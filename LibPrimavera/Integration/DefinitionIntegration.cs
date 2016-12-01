using System;
using System.Collections.Generic;

using Interop.CrmBE900;
using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class DefinitionsIntegration
    {
        private static Dictionary<DefinitionType, SqlColumn[]> definitionColumns = new Dictionary<DefinitionType, SqlColumn[]>
        {
            { 
                DefinitionType.ThirdParty, new SqlColumn[]
                {
                    new SqlColumn("TipoTerceiro", null),
                    new SqlColumn("Descricao", null),          
                }
            },
            {
                DefinitionType.Country, new SqlColumn[]
                {
                    new SqlColumn("Pais", null),
                    new SqlColumn("Descricao", null),          
                } 
            },
            {
                DefinitionType.ActivityType, new SqlColumn[]
                {
                    new SqlColumn("Id", null),
                    new SqlColumn("TipoActividade", null),
                    new SqlColumn("Descricao", null),          
                } 
            },
            { 
                DefinitionType.Language, new SqlColumn[]
                {
                    new SqlColumn("Idioma", null),
                    new SqlColumn("Descricao", null),          
                } 
            },
            { 
                DefinitionType.Zone, new SqlColumn[]
                {
                    new SqlColumn("Zona", null),
                    new SqlColumn("Descricao", null),         
                }
            },
            {
            DefinitionType.Title, new SqlColumn[]
                {
                    new SqlColumn("Titulo", null),
                    new SqlColumn("Descricao", null),         
                }
            }
        };

        private static Dictionary<DefinitionType, String> definitionTables = new Dictionary<DefinitionType, String>
        {   
            { DefinitionType.ActivityType, "TiposTarefa" },
            { DefinitionType.ThirdParty, "TipoTerceiros" },
            { DefinitionType.Title, "TitulosAcademicos" },
            { DefinitionType.Country, "Paises" },
            { DefinitionType.Language, "Idiomas" },
            { DefinitionType.Zone, "Zonas" }
        };

        public static String GetTypeIdName(DefinitionType definitionType)
        {
            return definitionType.ToDescriptionString();
        }

        public static SqlColumn[] getColumns(DefinitionType definitionType)
        {
            return definitionColumns.ContainsKey(definitionType) ? definitionColumns[definitionType] : new SqlColumn[] { };
        }

        public static String getTable(DefinitionType definitionType)
        {
            return definitionTables.ContainsKey(definitionType) ? definitionTables[definitionType] : "";
        }

        private static Reference GenerateType(StdBELista queryInfo, DefinitionType definitionType)
        {
            return new Reference()
            {
                Identificador = TypeParser.String(queryInfo.Valor(GetTypeIdName(definitionType))),
                Descricao = TypeParser.String(queryInfo.Valor("Descricao"))
            };
        }

        public static List<Reference> ListThirdPartyTypes()
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Reference>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("TipoTerceiros")
                .Columns(definitionColumns[DefinitionType.ThirdParty])
                .Where("EntidadesExternas", Comparison.Equals, 1));

            while (!queryObject.NoFim())
            {
                queryResult.Add(GenerateType(queryObject, DefinitionType.ThirdParty));
                queryObject.Seguinte();
            }

            return queryResult;
        }

        public static List<ActivityType> ListActivityTypes()
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<ActivityType>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable(getTable(DefinitionType.ActivityType))
                .Columns(getColumns(DefinitionType.ActivityType)));

            while (!queryObject.NoFim())
            {
                queryResult.Add(new ActivityType(
                    queryObject.Valor("Id"),
                    queryObject.Valor("TipoActividade"),
                    queryObject.Valor("Descricao")
                ));
                queryObject.Seguinte();
            }

            return queryResult;
        }

        public static List<Reference> ListTypes(DefinitionType definitionType)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            if (definitionColumns.ContainsKey(definitionType) == false)
            {
                throw new NotFoundException("tipo", false);
            }

            var queryResult = new List<Reference>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable(getTable(definitionType))
                .Columns(definitionColumns[definitionType]));

            while (!queryObject.NoFim())
            {
                queryResult.Add(GenerateType(queryObject, definitionType));
                queryObject.Seguinte();
            }

            return queryResult;
        }
    }
}