using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data.SqlClient;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class UserIntegration
    {
        private static string fieldNome = "Nome";
        private static string fieldComissao = "Comissao";
        private static string fieldVendedor = "Vendedor";

        private static SqlColumn[] sqlVendedor =
        {
            new SqlColumn(fieldNome, null),
            new SqlColumn(fieldComissao, null),
            new SqlColumn(fieldVendedor, null)    
        };

        public static List<RepresentativeListing> List()
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var representativeList = new List<RepresentativeListing>();
            var representativeInfo = PrimaveraEngine.Consulta(new SqlBuilder().Columns(sqlVendedor).FromTable("VENDEDORES"));

            while (!representativeInfo.NoFim())
            {
                representativeList.Add(new RepresentativeListing
                {
                    Identificador = TypeParser.String(representativeInfo.Valor(fieldVendedor)),
                    Nome = TypeParser.String(representativeInfo.Valor(fieldNome)),
                    Comissao = TypeParser.Double(representativeInfo.Valor(fieldComissao))
                });

                representativeInfo.Seguinte();
            }

            return representativeList;
        }

        public static Representative View(string representativeId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var representativesTable = PrimaveraEngine.Engine.Comercial.Vendedores;

            if (representativesTable.Existe(representativeId) == false)
            {
                return null;
            }

            var representativeInfo = representativesTable.Edita(representativeId);

            return new Representative
            {
                Nome = representativeInfo.get_Nome(),
                Email = representativeInfo.get_Email(),
                Morada = representativeInfo.get_Morada(),
                Comissao = representativeInfo.get_Comissao(),
                Telefone = representativeInfo.get_Telefone(),
                Telemovel = representativeInfo.get_Telemovel(),
                Identificador = representativeInfo.get_Vendedor(),
                Empresa = PrimaveraEngine.Engine.Licenca.get_Nome(),
                CodigoPostal = representativeInfo.get_CodigoPostal(),
                Fotografia = representativeInfo.get_LocalizacaoFoto(),
                Localidade = representativeInfo.get_LocalidadeCodigoPostal(),
            };
        }

        public static Reference Reference(string representativeId)
        {
            if (representativeId == null)
            {
                return null;
            }
            
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var representativesTable = PrimaveraEngine.Engine.Comercial.Vendedores;

            if (representativesTable.Existe(representativeId) == false)
            {
                return null;
            }

            return new Reference(representativeId, representativesTable.DaValorAtributo(representativeId, "Nome"));
        }

        private static string registerUser = "INSERT INTO users(username, password, representative) VALUES (:username, :password, :representative)";

        public static bool Insert(UserInfo paramObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var representativeId = paramObject.Representante;

            if (PrimaveraEngine.Engine.Comercial.Vendedores.Existe(representativeId) == false)
            {
                return false;
            }

            using (var sqlCommand = new SQLiteCommand(registerUser, PrimaveraEngine.getAuthenticationService()))
            {
                sqlCommand.Parameters.Add(new SQLiteParameter("username", paramObject.Username));
                sqlCommand.Parameters.Add(new SQLiteParameter("password", paramObject.Password));
                sqlCommand.Parameters.Add(new SQLiteParameter("representative", representativeId));
                sqlCommand.ExecuteNonQuery();
            }

            return true;
        }
    }
}