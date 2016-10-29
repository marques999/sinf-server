using System.Collections.Generic;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class WarehouseIntegration
    {
        private static SqlColumn[] sqlColumns =
        {
            new SqlColumn("ARMAZENS.Armazem", null),
            new SqlColumn("ARMAZENS.Descricao", null),
            new SqlColumn("ARMAZENS.Morada", null),
            new SqlColumn("ARMAZENS.Localidade", null),
            new SqlColumn("ARMAZENS.Cp", null),
            new SqlColumn("ARMAZENS.Distrito", null),
            new SqlColumn("ARMAZENS.Pais", null),
            new SqlColumn("ARTIGOARMAZEM.StkActual", null)
        };

        public static List<Warehouse> GetWarehouses(string productId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Warehouse>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ARTIGOARMAZEM")
                .Columns(sqlColumns)
                .Where("ARTIGOARMAZEM.Artigo", Comparison.Equals, productId)
                .LeftJoin("ARMAZENS", "Armazem", Comparison.Equals, "ARTIGOARMAZEM", "Armazem"));

            while (!queryObject.NoFim())
            {
                queryResult.Add(new Warehouse
                {
                    Identifier = TypeParser.String(queryObject.Valor("Armazem")),
                    Name = TypeParser.String(queryObject.Valor("Descricao")),
                    Stock = TypeParser.Double(queryObject.Valor("StkActual")),

                    Location = new Address
                    {
                        PostalCode = TypeParser.String(queryObject.Valor("Cp")),
                        Street = TypeParser.String(queryObject.Valor("Morada")),
                        Country = TypeParser.String(queryObject.Valor("Pais")),
                        Parish = TypeParser.String(queryObject.Valor("Localidade")),
                        State = TypeParser.String(queryObject.Valor("Distrito")),
                    }
                });

                queryObject.Seguinte();
            }

            return queryResult;
        }
    }
}