using FirstREST.Lib_Primavera.Model;
using Interop.GcpBE900;
using Interop.StdBE900;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FirstREST.Lib_Primavera.Integration
{
    public class DocVendaIntegration
    {
       /* public static ServerResponse Encomendas_New(DocVenda dv)
        {
            var rl = new PreencheRelacaoVendas();
            var lstlindv = new List<LinhaDocVenda>();

            try
            {
                if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == false)
                {
                    return new ServerResponse(1, "Erro ao abrir empresa");
                }

                var myLin = new GcpBELinhaDocumentoVenda();
                var myLinhas = new GcpBELinhasDocumentoVenda();
                var myEncomenda = new GcpBEDocumentoVenda();

                myEncomenda.set_Entidade(dv.Entidade);
                myEncomenda.set_Serie(dv.Serie);
                myEncomenda.set_Tipodoc("ECL");
                myEncomenda.set_TipoEntidade("C");
                lstlindv = dv.LinhasDoc;

                PriEngine.Engine.Comercial.Vendas.PreencheDadosRelacionados(myEncomenda);

                foreach (Model.LinhaDocVenda lin in lstlindv)
                {
                    PriEngine.Engine.Comercial.Vendas.AdicionaLinha(myEncomenda, lin.CodArtigo, lin.Quantidade, "", "", lin.PrecoUnitario, lin.Desconto);
                }

                // PriEngine.Engine.Comercial.Compras.TransformaDocumento(
                PriEngine.Engine.IniciaTransaccao();
                //PriEngine.Engine.Comercial.Vendas.Edita Actualiza(myEnc, "Teste");
                PriEngine.Engine.Comercial.Vendas.Actualiza(myEncomenda, "Teste");
                PriEngine.Engine.TerminaTransaccao();

                return new ServerResponse(0, "Sucesso");
            }
            catch (Exception ex)
            {
                PriEngine.Engine.DesfazTransaccao();
                return new ServerResponse(1, ex.Message);
            }
        }

        public static List<Model.DocVenda> Encomendas_List()
        {
            StdBELista objListLin;
            DocVenda dv = new DocVenda();
            List<DocVenda> listdv = new List<DocVenda>();
            LinhaDocVenda lindv = new LinhaDocVenda();
            List<LinhaDocVenda> listlindv = new List<LinhaDocVenda>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                StdBELista objListCab = PriEngine.Engine.Consulta("SELECT id, Entidade, Data, NumDoc, TotalMerc, Serie From CabecDoc where TipoDoc='ECL'");

                while (!objListCab.NoFim())
                {
                    dv = new Model.DocVenda();
                    dv.id = objListCab.Valor("id");
                    dv.Entidade = objListCab.Valor("Entidade");
                    dv.NumDoc = objListCab.Valor("NumDoc");
                    dv.Data = objListCab.Valor("Data");
                    dv.TotalMerc = objListCab.Valor("TotalMerc");
                    dv.Serie = objListCab.Valor("Serie");
                    objListLin = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido from LinhasDoc where IdCabecDoc='" + dv.id + "' order By NumLinha");
                    listlindv = new List<Model.LinhaDocVenda>();

                    while (!objListLin.NoFim())
                    {
                        lindv = new Model.LinhaDocVenda();
                        lindv.IdCabecDoc = objListLin.Valor("idCabecDoc");
                        lindv.CodArtigo = objListLin.Valor("Artigo");
                        lindv.DescArtigo = objListLin.Valor("Descricao");
                        lindv.Quantidade = objListLin.Valor("Quantidade");
                        lindv.Unidade = objListLin.Valor("Unidade");
                        lindv.Desconto = objListLin.Valor("Desconto1");
                        lindv.PrecoUnitario = objListLin.Valor("PrecUnit");
                        lindv.TotalILiquido = objListLin.Valor("TotalILiquido");
                        lindv.TotalLiquido = objListLin.Valor("PrecoLiquido");
                        listlindv.Add(lindv);
                        objListLin.Seguinte();
                    }

                    dv.LinhasDoc = listlindv;
                    listdv.Add(dv);
                    objListCab.Seguinte();
                }
            }
            return listdv;
        }

        public static DocVenda Encomenda_Get(string numdoc)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                return null;
            }

            var documentoVenda = new Model.DocVenda();
            var cabecalhoDocumento = PriEngine.Engine.Consulta("SELECT id, Entidade, Data, NumDoc, TotalMerc, Serie From CabecDoc where TipoDoc='ECL' and NumDoc='" + numdoc + "'");

            documentoVenda.id = cabecalhoDocumento.Valor("id");
            documentoVenda.Entidade = cabecalhoDocumento.Valor("Entidade");
            documentoVenda.NumDoc = cabecalhoDocumento.Valor("NumDoc");
            documentoVenda.Data = cabecalhoDocumento.Valor("Data");
            documentoVenda.TotalMerc = cabecalhoDocumento.Valor("TotalMerc");
            documentoVenda.Serie = cabecalhoDocumento.Valor("Serie");

            var linhaVenda = new LinhaDocVenda();
            var listaLinhas = new List<LinhaDocVenda>();
            var resultadoQuery = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido from LinhasDoc where IdCabecDoc='" + documentoVenda.id + "' order By NumLinha");

            while (!resultadoQuery.NoFim())
            {
                linhaVenda = new Model.LinhaDocVenda();
                linhaVenda.IdCabecDoc = resultadoQuery.Valor("idCabecDoc");
                linhaVenda.CodArtigo = resultadoQuery.Valor("Artigo");
                linhaVenda.DescArtigo = resultadoQuery.Valor("Descricao");
                linhaVenda.Quantidade = resultadoQuery.Valor("Quantidade");
                linhaVenda.Unidade = resultadoQuery.Valor("Unidade");
                linhaVenda.Desconto = resultadoQuery.Valor("Desconto1");
                linhaVenda.PrecoUnitario = resultadoQuery.Valor("PrecUnit");
                linhaVenda.TotalILiquido = resultadoQuery.Valor("TotalILiquido");
                linhaVenda.TotalLiquido = resultadoQuery.Valor("PrecoLiquido");
                listaLinhas.Add(linhaVenda);
                resultadoQuery.Seguinte();
            }

            documentoVenda.LinhasDoc = listaLinhas;

            return documentoVenda;
        }*/
    }
}