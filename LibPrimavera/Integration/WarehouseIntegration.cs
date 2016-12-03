using System;
using System.Collections.Generic;

using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class WarehouseIntegration
    {
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

        private static SqlColumn[] sqlColumnsListing =
        {
            new SqlColumn("ARMAZENS.Cp", null),
            new SqlColumn("ARMAZENS.Pais", null),
            new SqlColumn("ARMAZENS.Morada", null),
            new SqlColumn("ARMAZENS.Armazem", null),
            new SqlColumn("ARMAZENS.Distrito", null),
            new SqlColumn("ARMAZENS.Descricao", null),
            new SqlColumn("ARMAZENS.Localidade", null), 
        };

        public static List<WarehouseListing> List()
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var warehouseList = new List<WarehouseListing>();
            var warehouseInfo = PrimaveraEngine.Consulta(new SqlBuilder().FromTable("ARMAZENS").Columns(sqlColumnsListing));

            while (!warehouseInfo.NoFim())
            {
                warehouseList.Add(new WarehouseListing
                {
                    Localizacao = GetAddress(warehouseInfo),
                    Descricao = TypeParser.String(warehouseInfo.Valor("Descricao")),
                    Identificador = TypeParser.String(warehouseInfo.Valor("Armazem")),
                });

                warehouseInfo.Seguinte();
            }

            return warehouseList;
        }

        private static SqlColumn[] sqlColumnsFull =		
        {		
            new SqlColumn("ARMAZENS.Cp", null),
            new SqlColumn("ARMAZENS.Pais", null),
            new SqlColumn("ARMAZENS.Morada", null),
            new SqlColumn("ARMAZENS.Armazem", null),		
            new SqlColumn("ARMAZENS.Distrito", null),
            new SqlColumn("ARMAZENS.Telefone", null),	
            new SqlColumn("ARMAZENS.Descricao", null),	
            new SqlColumn("ARMAZENS.Localidade", null),
            new SqlColumn("ARMAZENS.DataUltimaActualizacao", null),	
        };

        public static Warehouse Get(string warehouseId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var warehousesTable = PrimaveraEngine.Engine.Comercial.Armazens;

            if (warehousesTable.Existe(warehouseId) == false)
            {
                throw new NotFoundException("armazém", false);
            }

            var warehouseInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ARMAZENS")
                .Columns(sqlColumnsFull)
                .Where("Armazem", Comparison.Equals, warehouseId));

            return new Warehouse
            {
                Localizacao = GetAddress(warehouseInfo),
                Descricao = TypeParser.String(warehouseInfo.Valor("Descricao")),
                Telefone = TypeParser.String(warehouseInfo.Valor("Telefone")),
                Identificador = TypeParser.String(warehouseInfo.Valor("Armazem")),
                DataModificacao = TypeParser.Date(warehouseInfo.Valor("DataUltimaActualizacao"))
            };
        }

        private static SqlColumn[] sqlColumnsAggregate =
        {
            new SqlColumn("MAX(ARMAZENS.Cp)", "Cp"),     	
            new SqlColumn("MAX(ARMAZENS.Pais)", "Pais"),
            new SqlColumn("ARMAZENS.Armazem", "Armazem"),
            new SqlColumn("MAX(ARMAZENS.Morada)", "Morada"),    	
            new SqlColumn("MAX(ARMAZENS.Distrito)", "Distrito"),
            new SqlColumn("MAX(ARMAZENS.Localidade)", "Localidade"),
            new SqlColumn("MAX(ARMAZENS.Descricao)", "Descricao"),
            new SqlColumn("SUM(ARTIGOARMAZEM.StkActual)", "Stock")
        };

        public static List<WarehouseProduct> GetWarehouses(string productId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var warehouseList = new List<WarehouseProduct>();
            var warehouseInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ARTIGOARMAZEM")
                .Columns(sqlColumnsAggregate)
                .Where("ARTIGOARMAZEM.Artigo", Comparison.Equals, productId)
                .LeftJoin("ARMAZENS", "Armazem", Comparison.Equals, "ARTIGOARMAZEM", "Armazem")
                .GroupBy(new string[] { "ARTIGOARMAZEM.Artigo", "ARMAZENS.Armazem" }));

            while (!warehouseInfo.NoFim())
            {
                warehouseList.Add(new WarehouseProduct
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