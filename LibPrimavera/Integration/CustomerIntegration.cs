using System;
using System.Collections.Generic;

using Interop.GcpBE900;
using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class CustomerIntegration
    {
        private static SqlColumn[] sqlColumnsListing =
        {
            new SqlColumn("CLIENTES.Cliente", null),
            new SqlColumn("CLIENTES.Situacao", null),
            new SqlColumn("CLIENTES.Nome", null),
            new SqlColumn("CLIENTES.Moeda", null),
            new SqlColumn("CLIENTES.NumContrib", null),
            new SqlColumn("CLIENTES.EnderecoWeb", null),
            new SqlColumn("CLIENTES.DataCriacao", null),
            new SqlColumn("CLIENTES.DataUltimaActualizacao", null),
            new SqlColumn("CLIENTES.EncomendasPendentes", null),
            new SqlColumn("CLIENTES.TotalDeb", null),
            new SqlColumn("CLIENTES.Fac_Tel", null),
            new SqlColumn("CLIENTES.Fac_Cp", null),
            new SqlColumn("CLIENTES.Fac_Mor", null),
            new SqlColumn("CLIENTES.Pais", null),
            new SqlColumn("CLIENTES.Fac_Local", null),
            new SqlColumn("CLIENTES.Distrito", null)
        };

        private static Customer GenerateFull(GcpBECliente customerInfo)
        {
            return new Customer()
            {
                Identficador = customerInfo.get_Cliente(),
                NomeFiscal = customerInfo.get_Nome(),
                Moeda = customerInfo.get_Moeda(),
                Estado = customerInfo.get_Situacao(),
                Debito = customerInfo.get_DebitoContaCorrente(),
                NumContribuinte = customerInfo.get_NumContribuinte(),
                Pendentes = customerInfo.get_DebitoEncomendasPendentes(),
                CriadoEm = customerInfo.get_DataCriacao(),
                ModificadoEm = customerInfo.get_DataUltimaActualizacao(),
                EnderecoWeb = customerInfo.get_EnderecoWeb(),
                Particular = customerInfo.get_PessoaSingular(),
                Telefone = customerInfo.get_Telefone(),
                Telefone2 = customerInfo.get_Telefone2(),
                Localizacao = new Address
                {
                    CodigoPostal = customerInfo.get_CodigoPostal(),
                    Morada = customerInfo.get_Morada(),
                    Pais = customerInfo.get_Pais(),
                    Localidade = customerInfo.get_Localidade(),
                    Distrito = customerInfo.get_Distrito()
                }
            };
        }

        private static CustomerListing GenerateListing(StdBELista customerInfo)
        {
            return new CustomerListing()
            {
                Identifier = TypeParser.String(customerInfo.Valor("Cliente")),
                Name = TypeParser.String(customerInfo.Valor("Nome")),
                Estado = TypeParser.String(customerInfo.Valor("Situacao")),
                Debito = TypeParser.Double(customerInfo.Valor("TotalDeb")),
                Pendentes = TypeParser.Double(customerInfo.Valor("EncomendasPendentes")),
                DateModified = TypeParser.Date(customerInfo.Valor("DataUltimaActualizacao")),
                Address = TypeParser.String(customerInfo.Valor("Fac_Mor")),
                Country = TypeParser.String(customerInfo.Valor("Pais")),
                State = TypeParser.String(customerInfo.Valor("Distrito"))
            };
        }

        public static List<CustomerListing> List(string sessionId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<CustomerListing>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder().FromTable("CLIENTES").Columns(sqlColumnsListing));

            while (!queryObject.NoFim())
            {
                queryResult.Add(GenerateListing(queryObject));
                queryObject.Seguinte();
            }

            queryResult.Sort(delegate(CustomerListing lhs, CustomerListing rhs)
            {
                if (lhs.Name == null || rhs.Name == null)
                {
                    return -1;
                }

                return lhs.Name.CompareTo(rhs.Name);
            });

            return queryResult;
        }

        public static Customer View(string sessionId, string customerId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var customersTable = PrimaveraEngine.Engine.Comercial.Clientes;

            if (customersTable.Existe(customerId) == false)
            {
                return null;
            }

            return GenerateFull(customersTable.Edita(customerId));
        }

        public static Reference Reference(string customerId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var customersTable = PrimaveraEngine.Engine.Comercial.Clientes;

            if (customersTable.Existe(customerId) == false)
            {
                return null;
            }

            return new Reference(customerId, customersTable.DaNome(customerId));
        }

        private static void SetFields(GcpBECliente customerInfo, Customer jsonObject)
        {
            if (jsonObject.NomeFiscal != null)
            {
                customerInfo.set_Nome(jsonObject.NomeFiscal.Trim());
            }

            if (jsonObject.Estado != null)
            {
                customerInfo.set_Situacao(jsonObject.Estado.Trim());
            }

            if (jsonObject.Moeda != null)
            {
                customerInfo.set_Moeda(jsonObject.Moeda.Trim());
            }

            if (jsonObject.NumContribuinte != null)
            {
                customerInfo.set_NumContribuinte(jsonObject.NumContribuinte.Trim());
            }

            if (jsonObject.ModificadoEm != null)
            {
                customerInfo.set_DataUltimaActualizacao(jsonObject.ModificadoEm);
            }

            if (jsonObject.Telefone != null)
            {
                customerInfo.set_Telefone(jsonObject.Telefone.Trim());
            }

            if (jsonObject.EnderecoWeb != null)
            {
                customerInfo.set_EnderecoWeb(jsonObject.EnderecoWeb.Trim());
            }

            if (jsonObject.Localizacao != null)
            {
                var objectLocation = jsonObject.Localizacao;

                if (objectLocation.Morada != null)
                {
                    customerInfo.set_Morada(jsonObject.Localizacao.Morada.Trim());
                }

                if (objectLocation.Distrito != null)
                {
                    customerInfo.set_Distrito(jsonObject.Localizacao.Distrito.Trim());
                }

                if (objectLocation.Localidade != null)
                {
                    customerInfo.set_Localidade(jsonObject.Localizacao.Localidade.Trim());
                }

                if (objectLocation.CodigoPostal != null)
                {
                    customerInfo.set_CodigoPostal(jsonObject.Localizacao.CodigoPostal.Trim());
                }

                if (objectLocation.Pais != null)
                {
                    customerInfo.set_Pais(jsonObject.Localizacao.Pais.Trim());
                }
            }
        }

        public static bool Update(string sessionId, string customerId, Customer jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var customersTable = PrimaveraEngine.Engine.Comercial.Clientes;

            if (customersTable.Existe(customerId) == false)
            {
                return false;
            }

            var customerInfo = customersTable.Edita(customerId);

            if (customerInfo.get_Vendedor() != sessionId)
            {
                return false;
            }

            jsonObject.ModificadoEm = DateTime.Now;
            customerInfo.set_EmModoEdicao(true);
            SetFields(customerInfo, jsonObject);
            customersTable.Actualiza(customerInfo);

            return true;
        }

        public static bool Insert(string sessionId, Customer jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var customerInfo = new GcpBECliente();
            var customerId = new HashGenerator().EncodeLong(DateTime.Now.Ticks);
            var customersTable = PrimaveraEngine.Engine.Comercial.Clientes;

            if (customersTable.Existe(customerId))
            {
                return false;
            }
            
            jsonObject.Estado = "ACTIVO";
            jsonObject.CriadoEm = DateTime.Now;
            jsonObject.ModificadoEm = jsonObject.CriadoEm;
            customerInfo.set_Cliente(customerId);
            SetFields(customerInfo, jsonObject);
            customersTable.Actualiza(customerInfo);

            return true;
        }

        public static bool Delete(string sessionId, string customerId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var customersTable = PrimaveraEngine.Engine.Comercial.Clientes;

            if (customersTable.Existe(customerId) == false)
            {
                return false;
            }

            if (customersTable.Consulta(customerId).get_Vendedor() != sessionId)
            {
                return false;
            }

            customersTable.Remove(customerId);

            return true;
        }
    }
}