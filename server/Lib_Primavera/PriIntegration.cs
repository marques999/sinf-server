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
        #region Cliente

      

        public static ServerResponse createAccount(Account cli)
        {
            try
            {
                if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
                {
                    return new ServerResponse(1, "Error loading company database!");
                }

                var myCli = new GcpBECliente();

                myCli.set_Cliente(cli.ID);
                myCli.set_Nome(cli.NomeCliente);
                myCli.set_NumContribuinte(cli.NumContribuinte);
                myCli.set_Moeda(cli.Moeda);
                myCli.set_Morada(cli.Morada);
                PriEngine.Engine.Comercial.Clientes.Actualiza(myCli);

                return new ServerResponse(0, "Sucesso");
            }
            catch (Exception ex)
            {
                return new ServerResponse(1, ex.Message);
            }
        }

        #endregion Cliente;
        #region Artigo

      

        #endregion Artigo
        #region DocCompra

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

        #endregion DocCompra
        #region DocsVenda

        public static ServerResponse Encomendas_New(DocVenda dv)
        {
            PreencheRelacaoVendas rl = new PreencheRelacaoVendas();
            List<Model.LinhaDocVenda> lstlindv = new List<Model.LinhaDocVenda>();
            
            try
            {
                if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == false)
                {
                    return new ServerResponse(1, "Erro ao abrir empresa");
                }

                var myLin = new GcpBELinhaDocumentoVenda();
                var myLinhas = new GcpBELinhasDocumentoVenda();
                var myEncomenda = new GcpBEDocumentoVenda();
               
                // Atribui valores ao cabecalho do doc
                //myEnc.set_DataDoc(dv.Data);
                myEncomenda.set_Entidade(dv.Entidade);
                myEncomenda.set_Serie(dv.Serie);
                myEncomenda.set_Tipodoc("ECL");
                myEncomenda.set_TipoEntidade("C");
                   
                // Linhas do documento para a lista de linhas
                lstlindv = dv.LinhasDoc;
                   
                //PriEngine.Engine.Comercial.Vendas.PreencheDadosRelacionados(myEnc, rl);
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
     
            StdBELista objListCab;
            StdBELista objListLin;
            DocVenda dv = new DocVenda();
            List<DocVenda> listdv = new List<DocVenda>();
            LinhaDocVenda lindv = new LinhaDocVenda();
            List<LinhaDocVenda> listlindv = new List<LinhaDocVenda>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                objListCab = PriEngine.Engine.Consulta("SELECT id, Entidade, Data, NumDoc, TotalMerc, Serie From CabecDoc where TipoDoc='ECL'");
                
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
        }

        #endregion DocsVenda
    }
}