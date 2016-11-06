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

        public static List<Location> List()
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Location>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("DISTRITOS")
                .Columns(sqlDistrito)
                .Where("Distrito", Comparison.GreaterThan, 0));

            while (!queryObject.NoFim())
            {
                queryResult.Add(new Location()
                {
                    Identifier = TypeParser.String(queryObject.Valor("Distrito")),
                    Name = TypeParser.String(queryObject.Valor("Descricao"))
                });

                queryObject.Seguinte();
            }

            return queryResult;
        }

        public static List<Location> View(string paramId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Location>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("CONCELHOS")
                .Columns(sqlConcelho)
                .Where("Distrito", Comparison.Equals, paramId)
                .Where("Concelho", Comparison.GreaterThan, 0));

            while (!queryObject.NoFim())
            {
                queryResult.Add(new Location()
                {
                    Identifier = TypeParser.String(queryObject.Valor("Concelho")),
                    Name = TypeParser.String(queryObject.Valor("Descricao"))
                });

                queryObject.Seguinte();
            }

            return queryResult;
        }
    }
}