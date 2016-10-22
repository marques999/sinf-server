using System;
using System.Collections.Generic;

using Interop.GcpBE900;

using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Lib_Primavera.Integration
{
    public class AccountIntegration
    {
        public static List<Account> listAccounts()
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var accounts = new List<Account>();
            var queryResult = PriEngine.Consulta("SELECT Cliente, Nome, Moeda, NumContrib as NumContribuinte, Fac_Mor AS campo_exemplo FROM CLIENTES");

            while (!queryResult.NoFim())
            {
                accounts.Add(new Account
                {
                    Name = queryResult.Valor("Nome"),
                    Currency = queryResult.Valor("Moeda"),
                    TaxNumber = queryResult.Valor("NumContribuinte"),
                    /*Address = new Address
                    {
                        Street = queryResult.Valor("Morada"),
                        PostalCode = queryResult.Valor("CodigoPostal"),
                        Country = queryResult.Valor("Pais"),
                        City = queryResult.Valor("Zona")
                    }*/
                });

                queryResult.Seguinte();
            }

            return accounts;
        }

        public static Account getAccount(string paramId)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PriEngine.Clientes.Existe(paramId) == false)
            {
                throw new NotFoundException();
            }

            var queryResult = PriEngine.Clientes.Edita(paramId);

            return new Account
            {
                Name = queryResult.get_Nome(),
                Currency = queryResult.get_Moeda(),
                TaxNumber = queryResult.get_NumContribuinte(),

                Location = new Address
                {
                    Street = queryResult.get_Morada(),
                    PostalCode = queryResult.get_CodigoPostal(),
                    Country = queryResult.get_Pais(),
                    City = queryResult.get_Zona()
                }
            };
        }

        public static void UpdateAccount(string paramId, Account paramInstance)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PriEngine.Clientes.Existe(paramId) == false)
            {
                throw new NotFoundException();
            }

            var myAccount = PriEngine.Clientes.Edita(paramId);

            myAccount.set_EmModoEdicao(true);
            myAccount.set_Nome(paramInstance.Name);
            myAccount.set_NumContribuinte(paramInstance.TaxNumber);
            myAccount.set_Moeda(paramInstance.Currency);
            myAccount.set_Morada(paramInstance.Location.Street);
            myAccount.set_Zona(paramInstance.Location.City);
            myAccount.set_Pais(paramInstance.Location.Country);
            myAccount.set_CodigoPostal(paramInstance.Location.PostalCode);
            myAccount.set_Pais(paramInstance.Location.Coordinates);
            PriEngine.Clientes.Actualiza(myAccount);
        }

        public static void CreateAccount(string paramId, Account paramInstance)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PriEngine.Clientes.Existe(paramId) == true)
            {
                throw new EntityExistsException();
            }

            var myAccount = new GcpBECliente();

            myAccount.set_Cliente(paramId);
            myAccount.set_Nome(paramInstance.Name);
            myAccount.set_NumContribuinte(paramInstance.TaxNumber);
            myAccount.set_Moeda(paramInstance.Currency);
            myAccount.set_Morada(paramInstance.Location.Street);
            PriEngine.Clientes.Actualiza(myAccount);
        }
    }
}