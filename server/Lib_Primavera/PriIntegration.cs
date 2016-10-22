using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Interop.ErpBS900;
using Interop.StdPlatBS900;
using Interop.StdBE900;
using Interop.GcpBE900;
using ADODB;
using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Lib_Primavera
{
    public class PriIntegration
    {
        #region DocCompra
        /*
        public static List<DocCompra> VGR_List()
        {
            var listdc = new List<DocCompra>();

            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                return listdc;
            }

            var cabecalhoDocumento = PriEngine.Engine.Consulta("SELECT id, NumDocExterno, Entidade, DataDoc, NumDoc, TotalMerc, Serie From CabecCompras where TipoDoc='VGR'");

            while (!cabecalhoDocumento.NoFim())
            {
                var documentoCompra = new DocCompra();

                documentoCompra.id = cabecalhoDocumento.Valor("id");
                documentoCompra.NumDocExterno = cabecalhoDocumento.Valor("NumDocExterno");
                documentoCompra.Entidade = cabecalhoDocumento.Valor("Entidade");
                documentoCompra.NumDoc = cabecalhoDocumento.Valor("NumDoc");
                documentoCompra.Data = cabecalhoDocumento.Valor("DataDoc");
                documentoCompra.TotalMerc = cabecalhoDocumento.Valor("TotalMerc");
                documentoCompra.Serie = cabecalhoDocumento.Valor("Serie");

                var linhaDocumento = new LinhaDocCompra();
                var listaLinhas = new List<LinhaDocCompra>();
                var resultadoQuery = PriEngine.Engine.Consulta("SELECT idCabecCompras, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido, Armazem, Lote from LinhasCompras where IdCabecCompras='" + documentoCompra.id + "' order By NumLinha");

                while (!resultadoQuery.NoFim())
                {
                    linhaDocumento = new Model.LinhaDocCompra();
                    linhaDocumento.IdCabecDoc = resultadoQuery.Valor("idCabecCompras");
                    linhaDocumento.CodArtigo = resultadoQuery.Valor("Artigo");
                    linhaDocumento.DescArtigo = resultadoQuery.Valor("Descricao");
                    linhaDocumento.Quantidade = resultadoQuery.Valor("Quantidade");
                    linhaDocumento.Unidade = resultadoQuery.Valor("Unidade");
                    linhaDocumento.Desconto = resultadoQuery.Valor("Desconto1");
                    linhaDocumento.PrecoUnitario = resultadoQuery.Valor("PrecUnit");
                    linhaDocumento.TotalILiquido = resultadoQuery.Valor("TotalILiquido");
                    linhaDocumento.TotalLiquido = resultadoQuery.Valor("PrecoLiquido");
                    linhaDocumento.Armazem = resultadoQuery.Valor("Armazem");
                    linhaDocumento.Lote = resultadoQuery.Valor("Lote");
                    listaLinhas.Add(linhaDocumento);
                    resultadoQuery.Seguinte();
                }

                documentoCompra.LinhasDoc = listaLinhas;
                listdc.Add(documentoCompra);
                cabecalhoDocumento.Seguinte();
            }

            return listdc;
        }
     
        public static Model.ServerResponse VGR_New(Model.DocCompra dc)
        {
            GcpBEDocumentoCompra myGR = new GcpBEDocumentoCompra();
            GcpBELinhaDocumentoCompra myLin = new GcpBELinhaDocumentoCompra();
            GcpBELinhasDocumentoCompra myLinhas = new GcpBELinhasDocumentoCompra();
            PreencheRelacaoCompras rl = new PreencheRelacaoCompras();
            List<Model.LinhaDocCompra> lstlindv = new List<Model.LinhaDocCompra>();

            try
            {
                if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == true)
                {
                    // Atribui valores ao cabecalho do doc
                    //myEnc.set_DataDoc(dv.Data);
                    myGR.set_Entidade(dc.Entidade);
                    myGR.set_NumDocExterno(dc.NumDocExterno);
                    myGR.set_Serie(dc.Serie);
                    myGR.set_Tipodoc("VGR");
                    myGR.set_TipoEntidade("F");
                    // Linhas do documento para a lista de linhas
                    lstlindv = dc.LinhasDoc;
                    //PriEngine.Engine.Comercial.Compras.PreencheDadosRelacionados(myGR,rl);
                    PriEngine.Engine.Comercial.Compras.PreencheDadosRelacionados(myGR);
                    foreach (Model.LinhaDocCompra lin in lstlindv)
                    {
                        PriEngine.Engine.Comercial.Compras.AdicionaLinha(myGR, lin.CodArtigo, lin.Quantidade, lin.Armazem, "", lin.PrecoUnitario, lin.Desconto);
                    }


                    PriEngine.Engine.IniciaTransaccao();
                    PriEngine.Engine.Comercial.Compras.Actualiza(myGR, "Teste");
                    PriEngine.Engine.TerminaTransaccao();

                    return new ServerResponse(0, "Sucesso");
                }
                else
                {
                    return new ServerResponse(1, "Erro ao abrir empresa");
                }

            }
            catch (Exception ex)
            {
                PriEngine.Engine.DesfazTransaccao();
                return new ServerResponse(1, ex.Message);
            }
        }
        */
        #endregion DocCompra
    }
}