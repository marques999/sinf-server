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
                Identifier = queryResult.Valor("Cliente"),
                Name = queryResult.Valor("Nome"),
                Status = queryResult.Valor("Situacao"),
                DateCreated = queryResult.Valor("DataCriacao"),
                LastContact = queryResult.Valor("DataUltimaActualizacao"),
                Website = queryResult.Valor("EnderecoWeb"),
                Phone = queryResult.Valor("Fac_Tel"),

                Location = new Address
                {
                    PostalCode = queryResult.Valor("Fac_Cp"),
                    Street = queryResult.Valor("Fac_Mor"),
                    Country = queryResult.Valor("Pais"),
                    Parish = queryResult.Valor("Fac_Local"),
                    State = queryResult.Valor("Distrito"),
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

            /*var accountsTable = PriEngine.Engine.Comercial.Clientes;

            if (accountsTable.Existe(paramId) == false)
            {
                throw new NotFoundException();
            }*/

            return Generate(PriEngine.Consulta(new QueryBuilder()
                .FromTable("CLIENTES")
                .Columns(sqlColumns)
                .Where("CLIENTES.Cliente", Comparison.Equals, paramId)));
        }

        public static void UpdateAccount(string paramId, Account paramInstance)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var accountsTable = PriEngine.Engine.Comercial.Clientes;

            if (accountsTable.Existe(paramId) == false)
            {
                throw new NotFoundException();
            }

            var myAccount = accountsTable.Edita(paramId);

            myAccount.set_EmModoEdicao(true);
            myAccount.set_Nome(paramInstance.Name);
            myAccount.set_NumContribuinte(paramInstance.TaxNumber);
            myAccount.set_Moeda(paramInstance.Currency);
            myAccount.set_Morada(paramInstance.Location.Street);
            myAccount.set_Zona(paramInstance.Location.State);
            myAccount.set_Pais(paramInstance.Location.Country);
            myAccount.set_CodigoPostal(paramInstance.Location.PostalCode);
            myAccount.set_Pais(paramInstance.Location.Country);
            accountsTable.Actualiza(myAccount);
        }

        public static void CreateAccount(string paramId, Account paramInstance)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var accountsTable = PriEngine.Engine.Comercial.Clientes;

            if (accountsTable.Existe(paramId) == true)
            {
                throw new EntityExistsException();
            }

            var myAccount = new GcpBECliente();

            myAccount.set_Cliente(paramId);
            myAccount.set_Nome(paramInstance.Name);
            myAccount.set_NumContribuinte(paramInstance.TaxNumber);
            myAccount.set_Moeda(paramInstance.Currency);
            myAccount.set_Morada(paramInstance.Location.Street);
            accountsTable.Actualiza(myAccount);
        }
    }
}