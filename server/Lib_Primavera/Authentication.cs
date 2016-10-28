using System;
using System.Collections.Generic;

using Interop.StdBE900;

using FirstREST.Lib_Primavera.Enums;
using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Lib_Primavera.Integration
{
    public class Authentication
    {
        public static string LoginUtilizador(string email, string password)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var objList = PriEngine.Engine.Consulta("SELECT Nome, CDU_Password FROM Clientes WHERE CDU_Email = '" + email + "'");

            if (objList.Vazia())
            {
                return null;
            }

            string userPasswod = objList.Valor("CDU_Password");
            string userName = objList.Valor("Nome");

            if (userPasswod.Equals(PriEngine.Platform.Criptografia.Encripta(password, 30)))
            {
                return userName;
            }

            return null;
        }

        public static string MostraPassword(string email)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryObject = PriEngine.Consulta(new QueryBuilder()
                .FromTable("Clientes")
                .Column("CDU_Password")
                .Where("CDU_Email", Comparison.Equals, email));

            if (queryObject.Vazia())
            {
                return null;
            }

            return PriEngine.Platform.Criptografia.Encripta(queryObject.Valor("CDU_Password"), 30);
        }

        public static string ObterPasswordOriginal(string email)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryObject = PriEngine.Consulta(new QueryBuilder()
                .FromTable("Clientes")
                .Column("CDU_Password")
                .Where("CDU_Email", Comparison.Equals, email));

            if (queryObject.Vazia())
            {
                return null;
            }

            return PriEngine.Platform.Criptografia.Descripta(queryObject.Valor("CDU_Password"), 30);
        }
    }
}