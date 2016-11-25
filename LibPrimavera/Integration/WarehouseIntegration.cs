using System.Collections.Generic;

using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class WarehouseIntegration
    {
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

        private static Address GetAddress(StdBELista warehouseInfo)
        {
            return new Address
            {
                Pais = TypeParser.String(warehouseInfo.Valor("Pais")),
                Morada = TypeParser.String(warehouseInfo.Valor("Morada")),
                CodigoPostal = TypeParser.String(warehouseInfo.Valor("Cp")),
                Distrito = TypeParser.String(warehouseInfo.Valor("Distrito")),
                Localidade = TypeParser.String(warehouseInfo.Valor("Localidade"))
            };
        }

        public static List<Warehouse> List()
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var warehouseList = new List<Warehouse>();
            var warehouseInfo = PrimaveraEngine.Consulta(new SqlBuilder().FromTable("ARMAZENS").Columns(sqlColumnsListing));

            while (!warehouseInfo.NoFim())
            {
                warehouseList.Add(new Warehouse
                {
                    Localizacao = GetAddress(warehouseInfo),
                    Descricao = TypeParser.String(warehouseInfo.Valor("Descricao")),
                    Identificador = TypeParser.String(warehouseInfo.Valor("Armazem"))
                });

                warehouseInfo.Seguinte();
            }

            return warehouseList;
        }

        public static Warehouse Get(string warehouseId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var warehousesTable = PrimaveraEngine.Engine.Comercial.Armazens;

            if (warehousesTable.Existe(warehouseId) == false)
            {
                return null;
            }

            var warehouseInfo = warehousesTable.Edita(warehouseId);

            return new Warehouse
            {
                Identificador = warehouseInfo.get_Armazem(),
                Descricao = warehouseInfo.get_Descricao(),
                Telefone = warehouseInfo.get_Telefone(),
                DataModificacao = warehouseInfo.get_DataUltimaActualizacao(),
                Localizacao = new Address
                {
                    Pais = warehouseInfo.get_Pais(),
                    Morada = warehouseInfo.get_Morada(),
                    Distrito = warehouseInfo.get_Distrito(),
                    Localidade = warehouseInfo.get_Localidade(),
                    CodigoPostal = warehouseInfo.get_CodigoPostal()
                }
            };
        }

        public static List<Warehouse> GetWarehouses(string productId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var warehouseList = new List<Warehouse>();
            var warehouseInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ARTIGOARMAZEM")
                .Columns(sqlColumnsAggregate)
                .Where("ARTIGOARMAZEM.Artigo", Comparison.Equals, productId)
                .LeftJoin("ARMAZENS", "Armazem", Comparison.Equals, "ARTIGOARMAZEM", "Armazem")
                .GroupBy(new string[] { "ARTIGOARMAZEM.Artigo", "ARMAZENS.Armazem" }));

            while (!warehouseInfo.NoFim())
            {
                warehouseList.Add(new Warehouse
                {
                    Localizacao = GetAddress(warehouseInfo),
                    Stock = TypeParser.Double(warehouseInfo.Valor("Stock")),
                    Descricao = TypeParser.String(warehouseInfo.Valor("Descricao")),
                    Identificador = TypeParser.String(warehouseInfo.Valor("Armazem"))
                });

                warehouseInfo.Seguinte();
            }

            return warehouseList;
        }
    }
}