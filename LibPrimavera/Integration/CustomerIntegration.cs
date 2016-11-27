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
            if (customerInfo.get_Inactivo())
            {
                return false;
            }

            var representativeId = customerInfo.get_Vendedor();

            if (representativeId != null && representativeId != sessionId)
            {
                return false;
            }

            return true;
        }

        private static SqlColumn[] sqlColumnsListing =
        {
            new SqlColumn("Cliente", null),
            new SqlColumn("Situacao", null),
            new SqlColumn("Nome", null),
            new SqlColumn("NumContrib", null),
            new SqlColumn("EnderecoWeb", null),
            new SqlColumn("DataUltimaActualizacao", null),
            new SqlColumn("EncomendasPendentes", null),
            new SqlColumn("TotalDeb", null),
            new SqlColumn("Fac_Tel", null),
            new SqlColumn("Fac_Cp", null),
            new SqlColumn("Fac_Mor", null),
            new SqlColumn("Pais", null),
        };

        public static List<CustomerListing> List(string sessionId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<CustomerListing>();
            var customerInfo = PrimaveraEngine.Consulta(new SqlBuilder().FromTable("CLIENTES").Columns(sqlColumnsListing));

            while (!customerInfo.NoFim())
            {
                queryResult.Add(new CustomerListing()
                {
                    Identificador = TypeParser.String(customerInfo.Valor("Cliente")),
                    Nome = TypeParser.String(customerInfo.Valor("Nome")),
                    Estado = TypeParser.String(customerInfo.Valor("Situacao")),
                    Debito = TypeParser.Double(customerInfo.Valor("TotalDeb")),
                    Pendentes = TypeParser.Double(customerInfo.Valor("EncomendasPendentes")),
                    DataModificacao = TypeParser.Date(customerInfo.Valor("DataUltimaActualizacao")),
                    Pais = TypeParser.String(customerInfo.Valor("Pais")),
                });

                customerInfo.Seguinte();
            }

            queryResult.Sort(delegate(CustomerListing lhs, CustomerListing rhs)
            {
                if (lhs.Nome == null || rhs.Nome == null)
                {
                    return -1;
                }

                return lhs.Nome.CompareTo(rhs.Nome);
            });

            return queryResult;
        }

        private static SqlColumn[] sqlColumnsFull =		
        {		
            new SqlColumn("Cliente", null),		
            new SqlColumn("Nome", null),		
            new SqlColumn("Situacao", null),	
            new SqlColumn("TotalDeb", null),			      	
            new SqlColumn("NumContrib", null),		
            new SqlColumn("DataCriacao", null),		
            new SqlColumn("DataUltimaActualizacao", null),
            new SqlColumn("EncomendasPendentes", null),		
            new SqlColumn("EnderecoWeb", null),		      
            new SqlColumn("PessoaSingular", null),		
            new SqlColumn("Fac_Tel", null),	
            new SqlColumn("Fac_Fax", null),		     	
            new SqlColumn("Fac_Cp", null),		
            new SqlColumn("Fac_Mor", null),		
            new SqlColumn("Pais", null),		
            new SqlColumn("Fac_Local", null),		
            new SqlColumn("Distrito", null)
        };

        public static CustomerInfo View(string sessionId, string customerId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var customersTable = PrimaveraEngine.Engine.Comercial.Clientes;

            if (customersTable.Existe(customerId) == false)
            {
                return null;
            }

            var customerInfo = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("CLIENTES")
                .Columns(sqlColumnsFull)
                .Where("Cliente", Comparison.Equals, customerId));

            var distritoId = TypeParser.String(customerInfo.Valor("Distrito"));

            return new CustomerInfo()
            {
                Identificador = TypeParser.String(customerInfo.Valor("Cliente")),
                Nome = TypeParser.String(customerInfo.Valor("Nome")),
                Situacao = TypeParser.String(customerInfo.Valor("Situacao")),
                Debito = TypeParser.Double(customerInfo.Valor("TotalDeb")),
                NumContribuinte = TypeParser.String(customerInfo.Valor("NumContrib")),
                Pendentes = TypeParser.Double(customerInfo.Valor("EncomendasPendentes")),
                DataCriacao = TypeParser.Date(customerInfo.Valor("DataCriacao")),
                DataModificacao = TypeParser.Date(customerInfo.Valor("DataUltimaActualizacao")),
                EnderecoWeb = TypeParser.String(customerInfo.Valor("EnderecoWeb")),
                Particular = TypeParser.Boolean(customerInfo.Valor("PessoaSingular")),
                Telefone = TypeParser.String(customerInfo.Valor("Fac_Tel")),
                Telefone2 = TypeParser.String(customerInfo.Valor("Fac_Fax")),
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
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var customersTable = PrimaveraEngine.Engine.Comercial.Clientes;

            if (customersTable.Existe(customerId) == false)
            {
                return null;
            }

            return new EntityReference(customerId, EntityType.Customer, customersTable.DaNome(customerId));
        }

        private static void SetMorada(GcpBECliente customerInfo, Address jsonObject)
        {
            if (jsonObject.Pais != null)
                customerInfo.set_Pais(jsonObject.Pais);
            if (jsonObject.Morada != null)
                customerInfo.set_Morada(jsonObject.Morada);
            if (jsonObject.Distrito != null)
                customerInfo.set_Distrito(jsonObject.Distrito);
            if (jsonObject.Localidade != null)
                customerInfo.set_Localidade(jsonObject.Localidade);
            if (jsonObject.CodigoPostal != null)
                customerInfo.set_CodigoPostal(jsonObject.CodigoPostal);
            if (jsonObject.Localidade != null)
                customerInfo.set_LocalidadeCodigoPostal(jsonObject.Localidade);
        }

        private static void SetFields(GcpBECliente customerInfo, Customer jsonObject)
        {
            customerInfo.set_PessoaSingular(jsonObject.Particular);
            if (jsonObject.Nome != null)
                customerInfo.set_Nome(jsonObject.Nome);
            if (jsonObject.Situacao != null)
                customerInfo.set_Situacao(jsonObject.Situacao);
            if (jsonObject.NumContribuinte != null)
                customerInfo.set_NumContribuinte(jsonObject.NumContribuinte);
            if (jsonObject.Telefone != null)
                customerInfo.set_Telefone(jsonObject.Telefone);
            if (jsonObject.Telefone2 != null)
                customerInfo.set_Telefone(jsonObject.Telefone2);
            if (jsonObject.EnderecoWeb != null)
                customerInfo.set_EnderecoWeb(jsonObject.EnderecoWeb);
            if (jsonObject.Localizacao != null)
                SetMorada(customerInfo, jsonObject.Localizacao);
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
                return null;
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

            return new CustomerInfo()
            {
                Identificador = customerInfo.get_Cliente(),
                Nome = customerInfo.get_Nome(),
                Situacao = customerInfo.get_Situacao(),
                Debito = customerInfo.get_DebitoContaCorrente(),
                NumContribuinte = customerInfo.get_NumContribuinte(),
                Pendentes = customerInfo.get_DebitoEncomendasPendentes(),
                DataCriacao = customerInfo.get_DataCriacao(),
                DataModificacao = customerInfo.get_DataUltimaActualizacao(),
                EnderecoWeb = customerInfo.get_EnderecoWeb(),
                Particular = customerInfo.get_PessoaSingular(),
                Telefone = customerInfo.get_Telefone(),
                Telefone2 = customerInfo.get_Telefone2(),
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
                return null;
            }

            var dateCreated = DateTime.Now;

            customerInfo.set_Cliente(customerId);
            SetFields(customerInfo, jsonObject);
            customerInfo.set_Moeda("EUR");
            customerInfo.set_Vendedor(sessionId);
            customerInfo.set_DataCriacao(dateCreated);
            customerInfo.set_DataUltimaActualizacao(dateCreated);
            customersTable.Actualiza(customerInfo);

            return new CustomerListing()
            {
                Identificador = customerInfo.get_Cliente(),
                Nome = customerInfo.get_Nome(),
                Estado = customerInfo.get_Situacao(),
                Debito = customerInfo.get_DebitoContaCorrente(),
                Pendentes = customerInfo.get_DebitoEncomendasPendentes(),
                DataModificacao = customerInfo.get_DataUltimaActualizacao(),
                Pais = customerInfo.get_Pais(),
                Distrito = customerInfo.get_Distrito()
            };
        }

        public static bool Delete(string sessionId, string customerId)
        {
            if (PrimaveraEngine.InitializeCompany() == false)
            {
                throw new DatabaseConnectionException();
            }

            var customersTable = PrimaveraEngine.Engine.Comercial.Clientes;

            if (customersTable.Existe(customerId) == false)
            {
                return false;
            }

            //var customerInfo = customersTable.Edita(customerId);

            /*if (CheckPermissions(customerInfo, sessionId) == false)
            {
                return false;
            }*/

            customersTable.Remove(customerId);

            //customerInfo.set_EmModoEdicao(true);
            //customerInfo.set_Inactivo(true);
            //customerInfo.set_DataUltimaActualizacao(DateTime.Now);
            //customersTable.Actualiza(customerInfo);

            return true;
        }
    }
}