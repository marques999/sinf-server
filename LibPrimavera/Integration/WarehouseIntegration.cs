using System.Collections.Generic;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class WarehouseIntegration
    {
        private static SqlColumn[] sqlColumnsFull =
        {
            new SqlColumn("ARMAZENS.Armazem", null),
            new SqlColumn("ARMAZENS.Descricao", null),
            new SqlColumn("ARMAZENS.Morada", null),
            new SqlColumn("ARMAZENS.Localidade", null),
            new SqlColumn("ARMAZENS.Cp", null),
            new SqlColumn("ARMAZENS.Distrito", null),
            new SqlColumn("ARMAZENS.Pais", null)
        };

        private static SqlColumn[] sqlColumnsAggregate =
        {
            new SqlColumn("ARMAZENS.Armazem", null),
            new SqlColumn("MAX(ARMAZENS.Descricao)", "Descricao"),
            new SqlColumn("MAX(ARMAZENS.Morada)", "Morada"),
            new SqlColumn("MAX(ARMAZENS.Localidade)", "Localidade"),
            new SqlColumn("MAX(ARMAZENS.Cp)", "Cp"),
            new SqlColumn("MAX(ARMAZENS.Distrito)", "Distrito"),
            new SqlColumn("MAX(ARMAZENS.Pais)", "Pais"),
            new SqlColumn("SUM(ARTIGOARMAZEM.StkActual)", "Stock")
        };

        private static SqlColumn[] sqlColumnsListing =
        {
            new SqlColumn("ARMAZENS.Armazem", null),
            new SqlColumn("ARMAZENS.Descricao", null),
            new SqlColumn("ARMAZENS.Morada", null),
            new SqlColumn("ARMAZENS.Localidade", null),
            new SqlColumn("ARMAZENS.Cp", null),
            new SqlColumn("ARMAZENS.Distrito", null),
            new SqlColumn("ARMAZENS.Pais", null)
        };

        public static List<Warehouse> List()
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Warehouse>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ARMAZENS")
                .Columns(sqlColumnsListing));

            while (!queryObject.NoFim())
            {
                queryResult.Add(new Warehouse
                {
                    Identifier = TypeParser.String(queryObject.Valor("Armazem")),
                    Name = TypeParser.String(queryObject.Valor("Descricao")),

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

        public static Warehouse Get(string warehouseId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Warehouse>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ARMAZENS")
                .Columns(sqlColumnsListing)
                .Where("Armazem", Comparison.Equals, warehouseId));

            return new Warehouse
                {
                    Identifier = TypeParser.String(queryObject.Valor("Armazem")),
                    Name = TypeParser.String(queryObject.Valor("Descricao")),

                    Location = new Address
                    {
                        PostalCode = TypeParser.String(queryObject.Valor("Cp")),
                        Street = TypeParser.String(queryObject.Valor("Morada")),
                        Country = TypeParser.String(queryObject.Valor("Pais")),
                        Parish = TypeParser.String(queryObject.Valor("Localidade")),
                        State = TypeParser.String(queryObject.Valor("Distrito")),
                    }
                };
        }

        public static List<Warehouse> GetWarehouses(string productId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Warehouse>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ARTIGOARMAZEM")
                .Columns(sqlColumnsAggregate)
                .Where("ARTIGOARMAZEM.Artigo", Comparison.Equals, productId)
                .LeftJoin("ARMAZENS", "Armazem", Comparison.Equals, "ARTIGOARMAZEM", "Armazem")
                .GroupBy(new string[] { "ARTIGOARMAZEM.Artigo", "ARMAZENS.Armazem" }));

            while (!queryObject.NoFim())
            {
                queryResult.Add(new Warehouse
                {
                    Identifier = TypeParser.String(queryObject.Valor("Armazem")),
                    Name = TypeParser.String(queryObject.Valor("Descricao")),
                    Stock = TypeParser.Double(queryObject.Valor("Stock")),

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