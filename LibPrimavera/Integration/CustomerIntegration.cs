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
        private static SqlColumn[] sqlColumnsFull =
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

        private static SqlColumn[] sqlColumnsReference =
        {
            new SqlColumn("CLIENTES.Cliente", null),
            new SqlColumn("CLIENTES.Nome", null)
        };

        private static Customer GenerateFull(StdBELista queryResult)
        {
            return new Customer()
            {
                Identifier = TypeParser.String(queryResult.Valor("Cliente")),
                Name = TypeParser.String(queryResult.Valor("Nome")),
                Currency = TypeParser.String(queryResult.Valor("Moeda")),
                Status = TypeParser.String(queryResult.Valor("Situacao")),
                Debito = TypeParser.Double(queryResult.Valor("TotalDeb")),
                TaxNumber = TypeParser.String(queryResult.Valor("NumContrib")),
                Pending = TypeParser.Double(queryResult.Valor("EncomendasPendentes")),
                DateCreated = TypeParser.Date(queryResult.Valor("DataCriacao")),
                DateModified = TypeParser.Date(queryResult.Valor("DataUltimaActualizacao")),
                Website = TypeParser.String(queryResult.Valor("EnderecoWeb")),
                Phone = TypeParser.String(queryResult.Valor("Fac_Tel")),

                Location = new Address
                {
                    PostalCode = TypeParser.String(queryResult.Valor("Fac_Cp")),
                    Street = TypeParser.String(queryResult.Valor("Fac_Mor")),
                    Country = TypeParser.String(queryResult.Valor("Pais")),
                    Parish = TypeParser.String(queryResult.Valor("Fac_Local")),
                    State = TypeParser.String(queryResult.Valor("Distrito"))
                }
            };
        }

        private static CustomerListing GenerateListing(StdBELista queryResult)
        {
            return new CustomerListing()
            {
                Identifier = TypeParser.String(queryResult.Valor("Cliente")),
                Name = TypeParser.String(queryResult.Valor("Nome")),
                Status = TypeParser.String(queryResult.Valor("Situacao")),
                Debito = TypeParser.Double(queryResult.Valor("TotalDeb")),
                Pending = TypeParser.Double(queryResult.Valor("EncomendasPendentes")),
                DateModified = TypeParser.Date(queryResult.Valor("DataUltimaActualizacao")),
                Address = TypeParser.String(queryResult.Valor("Fac_Mor")),
                Country = TypeParser.String(queryResult.Valor("Pais")),
                State = TypeParser.String(queryResult.Valor("Distrito"))
            };
        }

        private static Reference GenerateReference(StdBELista queryResult)
        {
            return new Reference
            {
                Identifier = TypeParser.String(queryResult.Valor("Cliente")),
                Name = TypeParser.String(queryResult.Valor("Nome"))
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
                if (lhs.Identifier == null || rhs.Identifier == null)
                {
                    return -1;
                }

                return lhs.Identifier.CompareTo(rhs.Identifier);
            });

            return queryResult;
        }

        public static Customer View(string sessionId, string paramId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.Comercial.Clientes.Existe(paramId) == false)
            {
                return null;
            }

            return GenerateFull(PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("CLIENTES")
                .Columns(sqlColumnsFull)
                .Where("CLIENTES.Cliente", Comparison.Equals, paramId)));
        }

        public static Reference Reference(string paramId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.Comercial.Clientes.Existe(paramId) == false)
            {
                throw new NotFoundException();
            }

            return GenerateReference(PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("CLIENTES")
                .Columns(sqlColumnsReference)
                .Where("Cliente", Comparison.Equals, paramId)));
        }

        private static void SetFields(GcpBECliente selectedRow, Customer paramObject)
        {
            if (paramObject.Name != null)
            {
                selectedRow.set_Nome(paramObject.Name.Trim());
            }

            if (paramObject.Status != null)
            {
                selectedRow.set_Situacao(paramObject.Status.Trim());
            }

            if (paramObject.Currency != null)
            {
                selectedRow.set_Moeda(paramObject.Currency.Trim());
            }

            if (paramObject.TaxNumber != null)
            {
                selectedRow.set_NumContribuinte(paramObject.TaxNumber.Trim());
            }

            if (paramObject.DateModified != null)
            {
                selectedRow.set_DataUltimaActualizacao(paramObject.DateModified);
            }

            if (paramObject.Phone != null)
            {
                selectedRow.set_Telefone(paramObject.Phone.Trim());
            }

            if (paramObject.Website != null)
            {
                selectedRow.set_EnderecoWeb(paramObject.Website.Trim());
            }

            if (paramObject.Location != null)
            {
                var objectLocation = paramObject.Location;

                if (objectLocation.Street != null)
                {
                    selectedRow.set_Morada(paramObject.Location.Street.Trim());
                }

                if (objectLocation.State != null)
                {
                    selectedRow.set_Distrito(paramObject.Location.State.Trim());
                }

                if (objectLocation.Parish != null)
                {
                    selectedRow.set_Localidade(paramObject.Location.Parish.Trim());
                }

                if (objectLocation.PostalCode != null)
                {
                    selectedRow.set_CodigoPostal(paramObject.Location.PostalCode.Trim());
                }

                if (objectLocation.Country != null)
                {
                    selectedRow.set_Pais(paramObject.Location.Country.Trim());
                }
            }
        }

        public static bool Update(string sessionId, string customerId, Customer paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var customerTable = PrimaveraEngine.Engine.Comercial.Clientes;

            if (customerTable.Existe(customerId) == false)
            {
                return false;
            }

            var linhaTabela = customerTable.Edita(customerId);

            if (linhaTabela.get_Vendedor() != customerId)
            {
                return false;
            }

            linhaTabela.set_EmModoEdicao(true);
            SetFields(linhaTabela, paramObject);
            customerTable.Actualiza(linhaTabela);

            return true;
        }

        public static bool Insert(string sessionId, Customer paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var selectedId = paramObject.Identifier;
            var selectedTable = PrimaveraEngine.Engine.Comercial.Clientes;

            if (selectedTable.Existe(selectedId))
            {
                return false;
            }

            var selectedRow = new GcpBECliente();
            
            selectedRow.set_Cliente(selectedId);
            SetFields(selectedRow, paramObject);
            selectedTable.Actualiza(selectedRow);

            return true;
        }

        public static bool Delete(string sessionId, string customerId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var customerTable = PrimaveraEngine.Engine.Comercial.Clientes;

            if (customerTable.Existe(customerId) == false)
            {
                return false;
            }

            if (customerTable.Consulta(customerId).get_Vendedor() != customerId)
            {
                return false;
            }

            customerTable.Remove(customerId);

            return true;
        }
    }
}