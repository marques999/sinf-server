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
                DefinitionType.Campaign, new SqlColumn[]
                {
                    new SqlColumn("Campanha", null),
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
            { DefinitionType.Zone, "Zonas" },
            { DefinitionType.Country, "Paises" },
            { DefinitionType.Language, "Idiomas" },
            { DefinitionType.Campaign, "Campanhas" },
            { DefinitionType.Title, "TitulosAcademicos" },
            { DefinitionType.ActivityType, "TiposTarefa" },
            { DefinitionType.ThirdParty, "TipoTerceiros" },
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

        private static ActivityType GenerateActivity(StdBELista typeInfo)
        {
            return new ActivityType(typeInfo.Valor("Id"), typeInfo.Valor("TipoActividade"), typeInfo.Valor("Descricao"));
        }

        private static Reference GenerateType(StdBELista queryInfo, DefinitionType definitionType)
        {
            return new Reference(TypeParser.String(queryInfo.Valor(GetTypeIdName(definitionType))), TypeParser.String(queryInfo.Valor("Descricao")));
        }

        public static List<Reference> ListThirdPartyTypes()
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var typeList = new List<Reference>();
            var typeInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("TipoTerceiros")
                .Columns(definitionColumns[DefinitionType.ThirdParty])
                .Where("EntidadesExternas", Comparison.Equals, 1));

            if (typeInfo == null || typeInfo.Vazia())
            {
                return typeList;
            }

            while (!typeInfo.NoFim())
            {
                typeList.Add(GenerateType(typeInfo, DefinitionType.ThirdParty));
                typeInfo.Seguinte();
            }

            return typeList;
        }

        public static List<Reference> ListCampaigns()
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var campaignList = new List<Reference>();
            var campaignInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable(definitionTables[DefinitionType.Campaign])
                .Columns(definitionColumns[DefinitionType.Campaign])
                .Where("Activa", Comparison.Equals, 1));

            if (campaignInfo == null || campaignInfo.Vazia())
            {
                return campaignList;
            }

            while (!campaignInfo.NoFim())
            {
                campaignList.Add(GenerateType(campaignInfo, DefinitionType.Campaign));
                campaignInfo.Seguinte();
            }

            return campaignList;
        }

        public static List<ActivityType> ListActivityTypes()
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var typeList = new List<ActivityType>();
            var typeInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable(getTable(DefinitionType.ActivityType))
                .Columns(getColumns(DefinitionType.ActivityType)));

            if (typeInfo == null || typeInfo.Vazia())
            {
                return typeList;
            }

            while (!typeInfo.NoFim())
            {
                typeList.Add(GenerateActivity(typeInfo));
                typeInfo.Seguinte();
            }

            return typeList;
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

            var typeList = new List<Reference>();
            var typeInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable(getTable(definitionType))
                .Columns(definitionColumns[definitionType]));

            if (typeInfo == null || typeInfo.Vazia())
            {
                return typeList;
            }

            while (!typeInfo.NoFim())
            {
                typeList.Add(GenerateType(typeInfo, definitionType));
                typeInfo.Seguinte();
            }

            return typeList;
        }
    }
}