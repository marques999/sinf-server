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
        private static bool CheckPermissions(GcpBECliente customerInfo, string sessionId)
        {
            /*if (customerInfo.get_Inactivo())
            {
                return false;
            }

            var representativeId = customerInfo.get_Vendedor();

            if (representativeId != null && representativeId != sessionId)
            {
                return false;
            }*/

            return true;
        }

        private static SqlColumn[] sqlColumnsListing =
        {
            new SqlColumn("Nome", null),
            new SqlColumn("Cliente", null),        
            new SqlColumn("Situacao", null),        
            new SqlColumn("TotalDeb", null),
            new SqlColumn("EncomendasPendentes", null),
            new SqlColumn("DataCriacao", null),
            new SqlColumn("DataUltimaActualizacao", null),
            new SqlColumn("Fac_Tel", null)
        };

        public static List<CustomerListing> List(string sessionId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var customerList = new List<CustomerListing>();
            var customerInfo = PrimaveraEngine.Consulta(new SqlBuilder().FromTable("CLIENTES").Columns(sqlColumnsListing));

            if (customerInfo == null || customerInfo.Vazia())
            {
                return customerList;
            }

            while (!customerInfo.NoFim())
            {
                customerList.Add(new CustomerListing()
                {
                    Nome = TypeParser.String(customerInfo.Valor("Nome")),
                    Identificador = TypeParser.String(customerInfo.Valor("Cliente")),
                    Estado = TypeParser.String(customerInfo.Valor("Situacao")),
                    Debito = TypeParser.Double(customerInfo.Valor("TotalDeb")),
                    Pendentes = TypeParser.Double(customerInfo.Valor("EncomendasPendentes")),
                    DataCriacao = TypeParser.Date(customerInfo.Valor("DataCriacao")),
                    DataModificacao = TypeParser.Date(customerInfo.Valor("DataUltimaActualizacao")),
                    Telefone = TypeParser.String(customerInfo.Valor("Fac_Tel"))
                });

                customerInfo.Seguinte();
            }

            customerList.Sort(delegate(CustomerListing lhs, CustomerListing rhs)
            {
                if (lhs.Nome == null || rhs.Nome == null)
                {
                    return -1;
                }

                return lhs.Nome.CompareTo(rhs.Nome);
            });

            return customerList;
        }

        private static SqlColumn[] sqlColumnsFull =		
        {		
            new SqlColumn("Cliente", null),		
            new SqlColumn("Nome", null),		
            new SqlColumn("Situacao", null),	
            new SqlColumn("TotalDeb", null),			      	
            new SqlColumn("NumContrib", null),		
            new SqlColumn("Telefone2", null),	
            new SqlColumn("EncomendasPendentes", null),		
            new SqlColumn("DataCriacao", null),		
            new SqlColumn("DataUltimaActualizacao", null),
            new SqlColumn("EnderecoWeb", null),		      
            new SqlColumn("PessoaSingular", null),	
            new SqlColumn("Fac_Fax", null),			
            new SqlColumn("Fac_Tel", null),	  	
            new SqlColumn("Fac_Cp", null),		
            new SqlColumn("Fac_Mor", null),		
            new SqlColumn("Fac_Local", null),
            new SqlColumn("Pais", null),
            new SqlColumn("Distrito", null),
            new SqlColumn("Vendedor", null)
        };

        public static CustomerInfo View(string sessionId, string customerId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var customerInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("CLIENTES")
                .Columns(sqlColumnsFull)
                .Where("Cliente", Comparison.Equals, customerId));

            if (customerInfo == null || customerInfo.Vazia())
            {
                throw new NotFoundException("cliente", false);
            }

            var responsavelId = TypeParser.String(customerInfo.Valor("Vendedor"));
            var distritoId = TypeParser.String(customerInfo.Valor("Distrito"));

            /*if (responsavelId.Equals(sessionId) == false)
            {
                return null;
            }*/

            return new CustomerInfo()
            {
                Identificador = TypeParser.String(customerInfo.Valor("Cliente")),
                Nome = TypeParser.String(customerInfo.Valor("Nome")),
                Estado = TypeParser.String(customerInfo.Valor("Situacao")),
                Debito = TypeParser.Double(customerInfo.Valor("TotalDeb")),
                NumContribuinte = TypeParser.String(customerInfo.Valor("NumContrib")),
                Pendentes = TypeParser.Double(customerInfo.Valor("EncomendasPendentes")),
                DataCriacao = TypeParser.Date(customerInfo.Valor("DataCriacao")),
                DataModificacao = TypeParser.Date(customerInfo.Valor("DataUltimaActualizacao")),
                EnderecoWeb = TypeParser.String(customerInfo.Valor("EnderecoWeb")),
                Particular = TypeParser.Boolean(customerInfo.Valor("PessoaSingular")),
                Telefone = TypeParser.String(customerInfo.Valor("Fac_Tel")),
                Telefone2 = TypeParser.String(customerInfo.Valor("Fac_Fax")),
                Telemovel = TypeParser.String(customerInfo.Valor("Telefone2")),
                Responsavel = UserIntegration.Reference(responsavelId),
                Localizacao = new Address
                {
                    Pais = TypeParser.String(customerInfo.Valor("Pais")),
                    Morada = TypeParser.String(customerInfo.Valor("Fac_Mor")),
                    Distrito = LocationIntegration.DistritoReference(distritoId),
                    CodigoPostal = TypeParser.String(customerInfo.Valor("Fac_Cp")),
                    Localidade = TypeParser.String(customerInfo.Valor("Fac_Local"))
                }
            };
        }

        public static EntityReference Reference(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                return null;
            }

            var customersTable = PrimaveraEngine.Engine.Comercial.Clientes;

            if (customersTable.Existe(customerId) == false)
            {
                return null;
            }

            return new EntityReference(customerId, EntityType.Customer, customersTable.DaNome(customerId));
        }

        private static void SetFields(GcpBECliente customerInfo, Customer jsonObject)
        {
            customerInfo.set_Nome(jsonObject.Nome);
            customerInfo.set_Fax(jsonObject.Telefone2);
            customerInfo.set_Telefone(jsonObject.Telefone);
            customerInfo.set_Telefone2(jsonObject.Telemovel);
            customerInfo.set_Pais(jsonObject.Localizacao.Pais);
            customerInfo.set_EnderecoWeb(jsonObject.EnderecoWeb);
            customerInfo.set_PessoaSingular(jsonObject.Particular);
            customerInfo.set_Morada(jsonObject.Localizacao.Morada);
            customerInfo.set_NumContribuinte(jsonObject.NumContribuinte);
            customerInfo.set_CodigoPostal(jsonObject.Localizacao.CodigoPostal);

            if (jsonObject.Localizacao.Pais.Equals("PT"))
            {
                if (jsonObject.Localizacao.Distrito == null)
                {
                    customerInfo.set_Distrito(null);
                    customerInfo.set_Localidade(null);
                    customerInfo.set_LocalidadeCodigoPostal(null);
                }
                else
                {
                    customerInfo.set_Distrito(jsonObject.Localizacao.Distrito);
                    customerInfo.set_Localidade(jsonObject.Localizacao.Localidade);
                    customerInfo.set_LocalidadeCodigoPostal(jsonObject.Localizacao.Localidade);
                }
            }
            else
            {
                customerInfo.set_Distrito(null);
                customerInfo.set_Localidade(null);
                customerInfo.set_LocalidadeCodigoPostal(null);
            }
        }

        private static CustomerInfo generateCustomer(GcpBECliente customerInfo)
        {
            return new CustomerInfo()
            {
                Identificador = customerInfo.get_Cliente(),
                Nome = customerInfo.get_Nome(),
                Estado = customerInfo.get_Situacao(),
                Debito = customerInfo.get_DebitoContaCorrente(),
                NumContribuinte = customerInfo.get_NumContribuinte(),
                Pendentes = customerInfo.get_DebitoEncomendasPendentes(),
                DataCriacao = customerInfo.get_DataCriacao(),
                DataModificacao = customerInfo.get_DataUltimaActualizacao(),
                EnderecoWeb = customerInfo.get_EnderecoWeb(),
                Particular = customerInfo.get_PessoaSingular(),
                Telefone = customerInfo.get_Telefone(),
                Telefone2 = customerInfo.get_Fax(),
                Telemovel = customerInfo.get_Telefone2(),
                Responsavel = UserIntegration.Reference(customerInfo.get_Vendedor()),
                Localizacao = new Address
                {
                    Pais = customerInfo.get_Pais(),
                    Morada = customerInfo.get_Morada(),
                    Localidade = customerInfo.get_Localidade(),
                    CodigoPostal = customerInfo.get_CodigoPostal(),
                    Distrito = LocationIntegration.DistritoReference(customerInfo.get_Distrito())
                }
            };
        }

        public static CustomerInfo Update(string sessionId, string customerId, Customer jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var customersTable = PrimaveraEngine.Engine.Comercial.Clientes;

            if (customersTable.Existe(customerId) == false)
            {
                throw new NotFoundException("cliente", false);
            }

            var customerInfo = customersTable.Edita(customerId);

            if (CheckPermissions(customerInfo, sessionId) == false)
            {
                return null;
            }

            customerInfo.set_EmModoEdicao(true);
            SetFields(customerInfo, jsonObject);
            customerInfo.set_DataUltimaActualizacao(DateTime.Now);
            customersTable.Actualiza(customerInfo);

            return generateCustomer(customerInfo);
        }

        public static CustomerListing Insert(string sessionId, Customer jsonObject)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var customerInfo = new GcpBECliente();
            var customerId = PrimaveraEngine.GenerateHash();
            var customersTable = PrimaveraEngine.Engine.Comercial.Clientes;

            if (customersTable.Existe(customerId))
            {
                throw new EntityExistsException("cliente", false);
            }

            customerInfo.set_Cliente(customerId);
            SetFields(customerInfo, jsonObject);
            customerInfo.set_Moeda("EUR");
            customerInfo.set_Situacao("ACTIVO");
            customerInfo.set_Inactivo(false);
            customerInfo.set_Vendedor(sessionId);
            customerInfo.set_DataCriacao(DateTime.Now);
            customerInfo.set_DataUltimaActualizacao(DateTime.Now);
            customersTable.Actualiza(customerInfo);

            return new CustomerListing()
            {
                Nome = customerInfo.get_Nome(),
                DataCriacao = customerInfo.get_DataCriacao(),
                Estado = customerInfo.get_Situacao(),
                Telefone = customerInfo.get_Telefone(),
                Identificador = customerInfo.get_Cliente(),
                Debito = customerInfo.get_DebitoContaCorrente(),
                Pendentes = customerInfo.get_DebitoEncomendasPendentes(),
                DataModificacao = customerInfo.get_DataUltimaActualizacao(),
            };
        }

        public static CustomerInfo Delete(string sessionId, string customerId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var customersTable = PrimaveraEngine.Engine.Comercial.Clientes;

            if (customersTable.Existe(customerId) == false)
            {
                throw new NotFoundException("cliente", false);
            }

            var customerInfo = customersTable.Edita(customerId);

            if (CheckPermissions(customerInfo, sessionId) == false)
            {
                return null;
            }

            customerInfo.set_EmModoEdicao(true);
            customerInfo.set_Situacao("INACTIVO");
            customerInfo.set_DataUltimaActualizacao(DateTime.Now);
            customersTable.Actualiza(customerInfo);

            return generateCustomer(customerInfo);
        }
    }
}