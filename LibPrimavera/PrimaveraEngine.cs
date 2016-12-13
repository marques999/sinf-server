using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;

using ADODB;

using Interop.ErpBS900;
using Interop.StdPlatBS900;
using Interop.StdBE900;
using Interop.IGcpBS900;
using Interop.ICrmBS900;

using FirstREST.QueryBuilder;
using FirstREST.LibPrimavera.Model;
using FirstREST.QueryBuilder.Enums;

namespace FirstREST.LibPrimavera
{
    public class PrimaveraEngine
    {
        public static StdPlatBS Platform
        {
            get;
            set;
        }

        public static ErpBS Engine
        {
            get;
            set;
        }

        public static StdBELista Consulta(SqlBuilder queryString)
        {
            string query = queryString.BuildQuery();
            System.Diagnostics.Debug.Print(query);
            return Engine.Consulta(query);
        }

        public static SQLiteDataReader ConsultaSQLite(SqlBuilder queryString)
        {
            using (var sqlQuery = sqliteConnection.CreateCommand())
            {
                System.Diagnostics.Debug.Print(queryString.BuildQuery());
                sqlQuery.CommandText = queryString.BuildQuery();
                return sqlQuery.ExecuteReader();
            }
        }

        private static SQLiteConnection sqliteConnection;

        private static bool sqliteInitialized = false;

        public static void InitializeSQLite()
        {
            if (sqliteInitialized)
            {
                return;
            }

            sqliteConnection = new SQLiteConnection("Data Source=C:\\SINF\\sinFORCE.db;Version=3;");

            using (var sqlQuery = sqliteConnection.CreateCommand())
            {
                sqliteConnection.Open();
                sqlQuery.CommandText = new SqlBuilder()
                    .FromTable("sqlite_master")
                    .Column("name")
                    .Where("name", Comparison.Equals, "users").BuildQuery();

                var queryResult = sqlQuery.ExecuteScalar();

                if (queryResult != null && queryResult.ToString() == "users")
                {
                    sqliteInitialized = true;
                }
                else
                {
                    sqlQuery.CommandText = "CREATE TABLE users (username VARCHAR(64), password VARCHAR(64), representative VARCHAR(8))";
                    sqlQuery.ExecuteNonQuery();
                    sqliteInitialized = true;
                }
            }
        }

        public static bool InitializeCompany()
        {
            if (Platform != null && Platform.Inicializada)
            {
                return true;
            }

            bool blnModoPrimario = true;
            var objAplConf = new StdBSConfApl();

            objAplConf.Instancia = "Default";
            objAplConf.AbvtApl = "GCP";
            objAplConf.PwdUtilizador = Properties.Settings.Default.Password.Trim();
            objAplConf.Utilizador = Properties.Settings.Default.User.Trim();
            objAplConf.LicVersaoMinima = "9.00";

            var MotorLE = new ErpBS();
            var Plataforma = new StdPlatBS();
            var objStdTransac = new StdBETransaccao();
            var tipoPlataforma = EnumTipoPlataforma.tpProfissional;

            try
            {
                Plataforma.AbrePlataformaEmpresa(Properties.Settings.Default.Company.Trim(), ref objStdTransac, ref objAplConf, ref tipoPlataforma, "");
            }
            catch
            {
                return false;
            }

            PrimaveraEngine.InitializeSQLite();

            if (Plataforma.Inicializada)
            {
                Platform = Plataforma;
                MotorLE.AbreEmpresaTrabalho(EnumTipoPlataforma.tpProfissional, Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim(), ref objStdTransac, "Default", ref blnModoPrimario);
                MotorLE.set_CacheActiva(true);
                Engine = MotorLE;
            }

            return Plataforma.Inicializada;
        }

        public static SQLiteConnection getAuthenticationService()
        {
            return sqliteConnection;
        }

        private static HashGenerator hashGenerator = new HashGenerator();

        public static string generateGUID()
        {
            return Guid.NewGuid().ToString("D").ToUpper();
        }

        public static string GenerateHash()
        {
            return hashGenerator.EncodeLong(DateTime.Now.Ticks);
        }

        public static string GenerateName(string fullName)
        {
            var sb = new StringBuilder();
            var trimmedName = fullName.Trim().ToUpperInvariant();

            foreach (char stringChar in trimmedName)
            {
                if ((stringChar >= '0' && stringChar <= '9') || (stringChar >= 'A' && stringChar <= 'Z') || stringChar == ' ')
                {
                    sb.Append(stringChar);
                }
            }

            var nameArray = sb.ToString().Split(' ');
            var firstName = nameArray[0];

            if (nameArray.Length > 1)
            {
                return nameArray[0].Substring(0, 2) + nameArray[nameArray.Length - 1].Substring(0, 3);
            }
            else
            {
                return firstName.Substring(0, firstName.Length < 6 ? firstName.Length : 6);
            }
        }
    }
}