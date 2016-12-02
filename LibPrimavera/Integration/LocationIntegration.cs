using System;
using System.Collections.Generic;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class LocationIntegration
    {
        private static string fieldPais = "Pais";
        private static string fieldConcelho = "Concelho";
        private static string fieldDistrito = "Distrito";
        private static string fieldDescricao = "Descricao";

        private static SqlColumn[] sqlDistrito =
        {
            new SqlColumn(fieldDistrito, null),
            new SqlColumn(fieldDescricao, null),
        };

        private static SqlColumn[] sqlConcelho =
        {
            new SqlColumn(fieldConcelho, null),
            new SqlColumn(fieldDescricao, null)
        };

        public static List<Reference> List()
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var distritoList = new List<Reference>();
            var distritoInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("DISTRITOS")
                .Columns(sqlDistrito)
                .Where(fieldDistrito, Comparison.GreaterThan, 0));

            while (!distritoInfo.NoFim())
            {
                distritoList.Add(new Reference()
                {
                    Descricao = TypeParser.String(distritoInfo.Valor(fieldDescricao)),
                    Identificador = TypeParser.String(distritoInfo.Valor(fieldDistrito))
                });

                distritoInfo.Seguinte();
            }

            return distritoList;
        }

        public static List<Reference> View(string paramId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var concelhoList = new List<Reference>();
            var concelhoInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("CONCELHOS")
                .Columns(sqlConcelho)
                .Where(fieldDistrito, Comparison.Equals, paramId)
                .Where(fieldConcelho, Comparison.GreaterThan, 0));

            while (!concelhoInfo.NoFim())
            {
                concelhoList.Add(new Reference()
                {
                    Descricao = TypeParser.String(concelhoInfo.Valor(fieldDescricao)),
                    Identificador = TypeParser.String(concelhoInfo.Valor(fieldConcelho))
                });

                concelhoInfo.Seguinte();
            }

            return concelhoList;
        }

        public static string DistritoReference(string distritoId)
        {
            var sqlQuery = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("DISTRITOS")
                .Columns(fieldDescricao)
                .Where(fieldDistrito, Comparison.Equals, distritoId));

            return sqlQuery.Vazia() ? distritoId : TypeParser.String(sqlQuery.Valor(fieldDescricao));
        }

        public static string ContryReference(string countryId)
        {
            var sqlQuery = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("PAISES")
                .Columns(fieldDescricao)
                .Where(fieldPais, Comparison.Equals, countryId));
            return sqlQuery.Vazia() ? countryId : TypeParser.String(sqlQuery.Valor(fieldDescricao));
        }
    }
}