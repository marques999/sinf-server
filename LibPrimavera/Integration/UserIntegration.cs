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
        public static List<Representative> List()
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Representative>();
            var queryObject = PrimaveraEngine.TabelaVendedores.LstVendedores();

            while (!queryObject.NoFim())
            {
                queryResult.Add(new Representative
                {
                    Name = TypeParser.String(queryObject.Valor("Nome")),
                    Email = TypeParser.String(queryObject.Valor("Email")),
                    // Picture = TypeParser.String(queryObject.Valor("LocalizacaoFoto")),
                    //  Identifier = TypeParser.String(queryObject.Valor("Vendedor"))
                });

                queryObject.Seguinte();
            }

            return queryResult;
        }

        private static UserData generateUser(SQLiteDataReader sqliteQuery, Interop.StdBE900.StdBELista queryObject)
        {
            if (sqliteQuery.Read())
            {
                return new UserData
                {
                    Username = sqliteQuery.GetString(sqliteQuery.GetOrdinal("username")),
                    Representative = new Representative
                    {
                        Name = queryObject.Valor("Nome"),
                        Email = queryObject.Valor("Email"),
                        Mobile = queryObject.Valor("Telemovel"),
                        Identifier = queryObject.Valor("Vendedor")

                    }
                };
            }
            else
            {
                return null;
            }
        }

        public static Representative View(string sessionUsername)
        {
            try
            {
                PrimaveraEngine.InitializeSQLite();
            }
            catch
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var sqlQuery = new SqlBuilder()
                .FromTable("users")
                .Column("reperesentative", null)
                .Where("username", Comparison.Equals, sessionUsername);

            using (var queryResult = PrimaveraEngine.ConsultaSQLite(sqlQuery))
            {
                if (queryResult.Read())
                {
                    var representativeId = queryResult.GetString(queryResult.GetOrdinal("representative"));
                    var representativeInfo = PrimaveraEngine.TabelaVendedores.Edita(representativeId);

                    return new Representative
                    {
                        Name = representativeInfo.get_Nome(),
                        Email = representativeInfo.get_Email(),
                        Identifier = representativeInfo.get_Vendedor(),
                        Phone = representativeInfo.get_Telefone(),
                        Mobile = representativeInfo.get_Telemovel(),
                        Picture = representativeInfo.get_LocalizacaoFoto()
                    };
                }
            }

            return null;
        }

        private static string changePassword = "UPDATE users SET password = ? WHERE username = ?";
        private static string registerUser = "INSERT INTO users(username, password, reference) VALUES (?,?,?)";

        public static bool Update(string sessionUsername, UserPassword userPassword)
        {
            try
            {
                PrimaveraEngine.InitializeSQLite();
            }
            catch
            {
                throw new DatabaseConnectionException();
            }

            var sqlQuery = new SqlBuilder()
                .FromTable("users")
                .Column("password")
                .Where("username", Comparison.Equals, sessionUsername);

            using (var queryResult = PrimaveraEngine.ConsultaSQLite(sqlQuery))
            {
                if (queryResult.Read())
                {
                    var oldPassword = queryResult.GetString(0);

                    if (oldPassword.Equals(userPassword.oldPassword) == false)
                    {
                        return false;
                    }

                    using (var sqlCommand = new SQLiteCommand(changePassword, PrimaveraEngine.getAuthenticationService()))
                    {
                        sqlCommand.Parameters.Add(sessionUsername);
                        sqlCommand.Parameters.Add(userPassword.newPassword);
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private static SqlColumn[] userLoginFields = new SqlColumn[]
        {
            new SqlColumn("username", null),
            new SqlColumn("passord", null)
        };

        public static Representative Authenticate(string userName, string userPassword)
        {
            try
            {
                PrimaveraEngine.InitializeSQLite();
            }
            catch
            {
                throw new DatabaseConnectionException();
            }

            var sqlQuery = new SqlBuilder()
                .FromTable("users")
                .Columns(userLoginFields)
                .Where("username", Comparison.Equals, userName);

            using (var queryResult = PrimaveraEngine.ConsultaSQLite(sqlQuery))
            {
                var userPasword = queryResult.GetOrdinal("password");

                if (queryResult.Read() == false)
                {
                    return null;
                }

                if (queryResult.GetString(userPasword).Equals(userPassword) == false)
                {
                    return null;
                }

                if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
                {
                    throw new DatabaseConnectionException();
                }

                var representativeId = queryResult.GetString(queryResult.GetOrdinal("representative"));
                var representativeInfo = PrimaveraEngine.TabelaVendedores.Edita(representativeId);

                return new Representative
                {
                    Name = representativeInfo.get_Nome(),
                    Email = representativeInfo.get_Email(),
                    Identifier = representativeInfo.get_Vendedor(),
                    Phone = representativeInfo.get_Telefone(),
                    Mobile = representativeInfo.get_Telemovel(),
                    Picture = representativeInfo.get_LocalizacaoFoto()
                };
            }
        }

        public static bool Insert(UserForm paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            try
            {
                PrimaveraEngine.InitializeSQLite();
            }
            catch
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.Comercial.Vendedores.Existe(paramObject.Representative) == false)
            {
                return false;
            }

            using (var sqlCommand = new SQLiteCommand(registerUser, PrimaveraEngine.getAuthenticationService()))
            {
                sqlCommand.Parameters.Add(paramObject.Username);
                sqlCommand.Parameters.Add(paramObject.Password);
                sqlCommand.Parameters.Add(paramObject.Representative);
                sqlCommand.ExecuteNonQuery();
            }

            return true;
        }

        public static bool Delete(string p)
        {
            throw new NotImplementedException();
        }
    }
}