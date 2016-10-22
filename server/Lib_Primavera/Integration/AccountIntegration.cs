using FirstREST.Lib_Primavera.Model;
using Interop.GcpBE900;
using System;
using System.Collections.Generic;

namespace FirstREST.Lib_Primavera.Integration
{
    public class AccountIntegration
    {
        public static List<Account> listAccounts()
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseException("AccountIntegration");
            }

            var accounts = new List<Account>();
            var queryResult = PriEngine.Consulta("SELECT Cliente, Nome, Moeda, NumContrib as NumContribuinte, Fac_Mor AS campo_exemplo FROM CLIENTES");

            while (!queryResult.NoFim())
            {
                accounts.Add(new Account
                {
                    Nome = queryResult.Valor("Nome"),
                    Moeda = queryResult.Valor("Moeda"),
                    NumContribuinte = queryResult.Valor("NumContribuinte"),/*
                    Address = new Address
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
                throw new DatabaseException("AccountIntegration");
            }

            if (PriEngine.Clientes.Existe(paramId) == false)
            {
                return null;
            }

            var queryResult = PriEngine.Clientes.Edita(paramId);

            return new Account
            {
                Nome = queryResult.get_Nome(),
                Moeda = queryResult.get_Moeda(),
                NumContribuinte = queryResult.get_NumContribuinte(),

                Morada = new Address
                {
                    Rua = queryResult.get_Morada(),
                    CodigoPostal = queryResult.get_CodigoPostal(),
                    Pais = queryResult.get_Pais(),
                    Zona = queryResult.get_Zona()
                }
            };
        }

        public static bool UpdateAccount(string accountId, Account paramAccount)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseException("AccountIntegration");
            }

            if (PriEngine.Clientes.Existe(accountId) == false)
            {
                return false;
            }

            var myAccount = PriEngine.Clientes.Edita(accountId);

            myAccount.set_EmModoEdicao(true);
            myAccount.set_Nome(paramAccount.Nome);
            myAccount.set_NumContribuinte(paramAccount.NumContribuinte);
            myAccount.set_Moeda(paramAccount.Moeda);
            myAccount.set_Morada(paramAccount.Morada.Rua);
            myAccount.set_Zona(paramAccount.Morada.Zona);
            myAccount.set_Pais(paramAccount.Morada.Pais);
            myAccount.set_CodigoPostal(paramAccount.Morada.CodigoPostal);
            myAccount.set_Pais(paramAccount.Morada.Coordinates);
            PriEngine.Clientes.Actualiza(myAccount);

            return true;
        }

        public static bool deleteAccount(string accountId)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseException("AccountIntegration");
            }

            if (PriEngine.Clientes.Existe(accountId) == false)
            {
                return false;
            }

            PriEngine.Clientes.Remove(accountId);

            return true;
        }

        public static bool CreateAccount(string accountId, Account paramAccount)
        {
            if (PriEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseException("AccountIntegration");
            }

            var myAccount = new GcpBECliente();

            myAccount.set_Cliente(accountId);
            myAccount.set_Nome(paramAccount.Nome);
            myAccount.set_NumContribuinte(paramAccount.NumContribuinte);
            myAccount.set_Moeda(paramAccount.Moeda);
            myAccount.set_Morada(paramAccount.Morada.Rua);
            PriEngine.Clientes.Actualiza(myAccount);

            return true;
        }
    }
}