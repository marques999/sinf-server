using System.Collections.Generic;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class LocationIntegration
    {
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

            var queryResult = new List<Reference>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("DISTRITOS")
                .Columns(sqlDistrito)
                .Where(fieldDistrito, Comparison.GreaterThan, 0));

            while (!queryObject.NoFim())
            {
                queryResult.Add(new Reference()
                {
                    Identificador = TypeParser.String(queryObject.Valor(fieldDistrito)),
                    Descricao = TypeParser.String(queryObject.Valor(fieldDescricao))
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
                .Where(fieldDistrito, Comparison.Equals, paramId)
                .Where(fieldConcelho, Comparison.GreaterThan, 0));

            while (!queryObject.NoFim())
            {
                queryResult.Add(new Reference()
                {
                    Identificador = TypeParser.String(queryObject.Valor(fieldConcelho)),
                    Descricao = TypeParser.String(queryObject.Valor(fieldDescricao))
                });

                queryObject.Seguinte();
            }

            return queryResult;
        }
    }
}