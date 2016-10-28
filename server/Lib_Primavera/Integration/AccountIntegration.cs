using System;
using System.Collections.Generic;

using Interop.GcpBE900;
using Interop.StdBE900;

using FirstREST.Lib_Primavera.Enums;
using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Lib_Primavera.Integration
{
    public class AccountIntegration
    {
        private static SqlColumn[] sqlColumns =
        {
            new SqlColumn("CLIENTES.Cliente", null),
            new SqlColumn("CLIENTES.Situacao", null),
            new SqlColumn("CLIENTES.Nome", null),
            new SqlColumn("CLIENTES.EnderecoWeb", null),
            new SqlColumn("CLIENTES.DataCriacao", null),
            new SqlColumn("CLIENTES.DataUltimaActualizacao", null),
            new SqlColumn("CLIENTES.Fac_Tel", null),
            new SqlColumn("CLIENTES.Fac_Cp", null),
            new SqlColumn("CLIENTES.Fac_Mor", null),
            new SqlColumn("CLIENTES.Pais", null),
            new SqlColumn("CLIENTES.Fac_Local", null),
            new SqlColumn("CLIENTES.Distrito", null)
        };

        private static Account Generate(StdBELista queryResult)
        {
            return new Account()
            {
                Identifier = TypeParser.String(queryResult.Valor("Cliente")),
                Name = TypeParser.String(queryResult.Valor("Nome")),
                Status = TypeParser.String(queryResult.Valor("Situacao")),
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
                    State = TypeParser.String(queryResult.Valor("Distrito")),
                },
            };
        }

        public static List<Account> GetAccounts()
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<Account>();
            var queryObject = PriEngine.Consulta(new QueryBuilder().FromTable("CLIENTES").Columns(sqlColumns));

            while (!queryObject.NoFim())
            {
                queryResult.Add(Generate(queryObject));
                queryObject.Seguinte();
            }

            queryResult.Sort(delegate(Account lhs, Account rhs)
            {
                if (lhs.Identifier == null || rhs.Identifier == null)
                {
                    return -1;
                }

                return lhs.Identifier.CompareTo(rhs.Identifier);
            });

            return queryResult;
        }

        public static Account GetAccount(string paramId)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PriEngine.Engine.Comercial.Clientes.Existe(paramId) == false)
            {
                throw new NotFoundException();
            }

            return Generate(PriEngine.Consulta(new QueryBuilder()
                .FromTable("CLIENTES")
                .Columns(sqlColumns)
                .Where("CLIENTES.Cliente", Comparison.Equals, paramId)));
        }

        private static void SetFields(GcpBECliente selectedRow, Account paramObject)
        {
            if (paramObject.Name != null)
            {
                selectedRow.set_Nome(paramObject.Name.Trim());
            }

            if (paramObject.Status != null)
            {
                selectedRow.set_Situacao(paramObject.Status);
            }

            if (paramObject.Currency != null)
            {
                selectedRow.set_Moeda(paramObject.Currency);
            }

            if (paramObject.TaxNumber != null)
            {
                selectedRow.set_NumContribuinte(paramObject.TaxNumber);
            }

            if (paramObject.DateModified != null)
            {
                selectedRow.set_DataUltimaActualizacao(paramObject.DateModified);
            }

            if (paramObject.Phone != null)
            {
                selectedRow.set_Telefone(paramObject.Phone);
            }

            if (paramObject.Website != null)
            {
                selectedRow.set_EnderecoWeb(paramObject.Website);
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

        public static bool UpdateAccount(string paramId, Account paramObject)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var selectedTable = PriEngine.Engine.Comercial.Clientes;

            if (selectedTable.Existe(paramId) == false)
            {
                return false;
            }

            var selectedRow = selectedTable.Edita(paramId);

            selectedRow.set_EmModoEdicao(true);
            SetFields(selectedRow, paramObject);
            selectedTable.Actualiza(selectedRow);

            return true;
        }

        public static bool CreateAccount(string paramId, Account paramObject)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var selectedRow = new GcpBECliente();
            var selectedTable = PriEngine.Engine.Comercial.Clientes;

            if (selectedTable.Existe(paramId))
            {
                return false;
            }

            selectedRow.set_Cliente(paramId);
            SetFields(selectedRow, paramObject);
            selectedTable.Actualiza(selectedRow);

            return true;
        }
    }
}