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

        public static String GetTableName(DefinitionType definitionType)
        {
            return definitionTables.ContainsKey(definitionType) ? definitionTables[definitionType] : null;
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

        public static List<Reference> ListTypes(DefinitionType definitionType)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            if (definitionColumns.ContainsKey(definitionType) == false)
            {
                return null;
            }

            var queryResult = new List<Reference>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable(GetTableName(definitionType))
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