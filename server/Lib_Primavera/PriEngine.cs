using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ADODB;
using Interop.ErpBS900;
using Interop.StdPlatBS900;
using Interop.StdBE900;
using Interop.IGcpBS900;
using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Lib_Primavera
{
    public class PriEngine
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

        public static IGcpBSArtigos Produtos
        {
            get
            {
                return Engine.Comercial.Artigos;
            }
        }

        public static IGcpBSClientes Clientes
        {
            get
            {
                return Engine.Comercial.Clientes;
            }
        }

        public static StdBELista Consulta(QueryBuilder queryString)
        {
            return Engine.Consulta(queryString.BuildQuery());
        }

        public static bool InitializeCompany(string Company, string User, string Password)
        {
            if (Platform != null && Platform.Inicializada)
            {
                return true;
            }

            bool blnModoPrimario = true;
            var objAplConf = new StdBSConfApl();

            objAplConf.Instancia = "Default";
            objAplConf.AbvtApl = "GCP";
            objAplConf.PwdUtilizador = Password;
            objAplConf.Utilizador = User;
            objAplConf.LicVersaoMinima = "9.00";

            var MotorLE = new ErpBS();
            var Plataforma = new StdPlatBS();
            var objStdTransac = new StdBETransaccao();
            var tipoPlataforma = EnumTipoPlataforma.tpProfissional;

            try
            {
                Plataforma.AbrePlataformaEmpresa(ref Company, ref objStdTransac, ref objAplConf, ref tipoPlataforma, "");
            }
            catch
            {
                return false;
            }

            if (Plataforma.Inicializada)
            {
                Platform = Plataforma;
                MotorLE.AbreEmpresaTrabalho(EnumTipoPlataforma.tpProfissional, ref Company, ref User, ref Password, ref objStdTransac, "Default", ref blnModoPrimario);
                MotorLE.set_CacheActiva(true);
                Engine = MotorLE;
            }

            return Plataforma.Inicializada;
        }
    }
}