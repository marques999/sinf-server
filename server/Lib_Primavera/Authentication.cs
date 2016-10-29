using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class Authentication
    {
        public static string LoginUtilizador(string email, string password)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var objList = PrimaveraEngine.Engine.Consulta("SELECT Nome, CDU_Password FROM Clientes WHERE CDU_Email = '" + email + "'");

            if (objList.Vazia())
            {
                return null;
            }

            string userPasswod = objList.Valor("CDU_Password");
            string userName = objList.Valor("Nome");

            if (userPasswod.Equals(PrimaveraEngine.Platform.Criptografia.Encripta(password, 30)))
            {
                return userName;
            }

            return null;
        }

        public static string MostraPassword(string email)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("Clientes")
                .Column("CDU_Password")
                .Where("CDU_Email", Comparison.Equals, email));

            if (queryObject.Vazia())
            {
                return null;
            }

            return PrimaveraEngine.Platform.Criptografia.Encripta(queryObject.Valor("CDU_Password"), 30);
        }

        public static string ObterPasswordOriginal(string email)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("Clientes")
                .Column("CDU_Password")
                .Where("CDU_Email", Comparison.Equals, email));

            if (queryObject.Vazia())
            {
                return null;
            }

            return PrimaveraEngine.Platform.Criptografia.Descripta(queryObject.Valor("CDU_Password"), 30);
        }
    }
}