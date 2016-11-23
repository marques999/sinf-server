using System.Collections.Generic;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class LocationIntegration
    {
        private static SqlColumn[] sqlDistrito =
        {
            new SqlColumn("DISTRITOS.Distrito", null),
            new SqlColumn("DISTRITOS.Descricao", null),
        };

        private static SqlColumn[] sqlConcelho =
        {
            new SqlColumn("CONCELHOS.Concelho", null),
            new SqlColumn("CONCELHOS.Descricao", null)
        };

        public static List<Reference> List()
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Reference>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("DISTRITOS")
                .Columns(sqlDistrito)
                .Where("Distrito", Comparison.GreaterThan, 0));

            while (!queryObject.NoFim())
            {
                queryResult.Add(new Reference()
                {
                    Identificador = TypeParser.String(queryObject.Valor("Distrito")),
                    Descricao = TypeParser.String(queryObject.Valor("Descricao"))
                });

                queryObject.Seguinte();
            }

            return queryResult;
        }

        public static List<Reference> View(string paramId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Reference>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("CONCELHOS")
                .Columns(sqlConcelho)
                .Where("Distrito", Comparison.Equals, paramId)
                .Where("Concelho", Comparison.GreaterThan, 0));

            while (!queryObject.NoFim())
            {
                queryResult.Add(new Reference()
                {
                    Identificador = TypeParser.String(queryObject.Valor("Concelho")),
                    Descricao = TypeParser.String(queryObject.Valor("Descricao"))
                });

                queryObject.Seguinte();
            }

            return queryResult;
        }
    }
}