using System;
using System.Collections.Generic;

using Interop.CrmBE900;
using Interop.StdBE900;

using FirstREST.QueryBuilder;
using FirstREST.QueryBuilder.Enums;
using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera.Integration
{
    public class LeadIntegration
    {
        private static SqlColumn[] sqlColumnsFull =
        {
            new SqlColumn("ENTIDADESEXTERNAS.Entidade", null),
            new SqlColumn("ENTIDADESEXTERNAS.Nome", null),
            new SqlColumn("ENTIDADESEXTERNAS.Email", null),
            new SqlColumn("ENTIDADESEXTERNAS.Activo", null),
            new SqlColumn("ENTIDADESEXTERNAS.DataCriacao", null),
            new SqlColumn("ENTIDADESEXTERNAS.DataUltAct", null),
            new SqlColumn("ENTIDADESEXTERNAS.Telefone", null),
            new SqlColumn("ENTIDADESEXTERNAS.Telemovel", null),
            new SqlColumn("ENTIDADESEXTERNAS.CodPostal", null),
            new SqlColumn("ENTIDADESEXTERNAS.Distrito", null),
            new SqlColumn("ENTIDADESEXTERNAS.Localidade", null),
            new SqlColumn("ENTIDADESEXTERNAS.Morada", null),
            new SqlColumn("ENTIDADESEXTERNAS.Pais", null)            
        };

        private static SqlColumn[] sqlColumnsListing =
        {
            new SqlColumn("ENTIDADESEXTERNAS.Entidade", null),
            new SqlColumn("ENTIDADESEXTERNAS.Nome", null),
            new SqlColumn("ENTIDADESEXTERNAS.Email", null),
            new SqlColumn("ENTIDADESEXTERNAS.Activo", null),
            new SqlColumn("ENTIDADESEXTERNAS.DataUltAct", null),
            new SqlColumn("ENTIDADESEXTERNAS.Telemovel", null),
            new SqlColumn("ENTIDADESEXTERNAS.Distrito", null),
            new SqlColumn("ENTIDADESEXTERNAS.Morada", null),
            new SqlColumn("ENTIDADESEXTERNAS.Pais", null)            
        };

        private static SqlColumn[] sqlColumnsReference =
        {
            new SqlColumn("ENTIDADESEXTERNAS.Entidade", null),
            new SqlColumn("ENTIDADESEXTERNAS.Nome", null),
        };

        private static Lead GenerateFull(StdBELista queryObject)
        {
            return new Lead()
            {
                Identficador = TypeParser.String(queryObject.Valor("Entidade")),
                Active = TypeParser.Boolean(queryObject.Valor("Activo")),
                NomeFiscal = TypeParser.String(queryObject.Valor("Nome")),
                Email = TypeParser.String(queryObject.Valor("Email")),
                Telefone = TypeParser.String(queryObject.Valor("Telefone")),
                DateCreated = TypeParser.Date(queryObject.Valor("DataCriacao")),
                ModificadoEm = TypeParser.Date(queryObject.Valor("DataUltAct")),
                Telemovel = TypeParser.String(queryObject.Valor("Telemovel")),

                Localizacao = new Address
                {
                    CodigoPostal = TypeParser.String(queryObject.Valor("CodPostal")),
                    Distrito = TypeParser.String(queryObject.Valor("Distrito")),
                    Localidade = TypeParser.String(queryObject.Valor("Localidade")),
                    Morada = TypeParser.String(queryObject.Valor("Morada")),
                    Pais = TypeParser.String(queryObject.Valor("Pais"))
                },
            };
        }

        private static LeadListing GenerateListing(StdBELista queryObject)
        {
            return new LeadListing()
            {
                Identifier = TypeParser.String(queryObject.Valor("Entidade")),
                Active = TypeParser.Boolean(queryObject.Valor("Activo")),
                Name = TypeParser.String(queryObject.Valor("Nome")),
                Email = TypeParser.String(queryObject.Valor("Email")),
                DateModified = TypeParser.Date(queryObject.Valor("DataUltAct")),
                MobilePhone = TypeParser.String(queryObject.Valor("Telemovel")),
                State = TypeParser.String(queryObject.Valor("Distrito")),
                Address = TypeParser.String(queryObject.Valor("Morada")),
                Country = TypeParser.String(queryObject.Valor("Pais"))
            };
        }

        private static Reference GenerateReference(StdBELista queryObject)
        {
            return new Reference
            {
                Identifier = TypeParser.String(queryObject.Valor("Entidade")),
                Name = TypeParser.String(queryObject.Valor("Nome"))
            };
        }

        public static List<LeadListing> List(string sessionId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var queryResult = new List<LeadListing>();
            var queryObject = PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ENTIDADESEXTERNAS")
                .Columns(sqlColumnsListing)
                .Where("PotencialCliente", Comparison.Equals, "TRUE")
                .Where(new WhereClause("Vendedor", Comparison.Equals, sessionId).AddClause(LogicOperator.Or, Comparison.Equals, null)));

            while (!queryObject.NoFim())
            {
                queryResult.Add(GenerateListing(queryObject));
                queryObject.Seguinte();
            }

            queryResult.Sort(delegate(LeadListing lhs, LeadListing rhs)
            {
                if (lhs.Identifier == null || rhs.Identifier == null)
                {
                    return -1;
                }

                return lhs.Identifier.CompareTo(rhs.Identifier);
            });

            return queryResult;
        }

        public static Lead View(string sessionId, string paramId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.CRM.EntidadesExternas.Existe(paramId) == false)
            {
                return null;
            }

            return GenerateFull(PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ENTIDADESEXTERNAS")
                .Columns(sqlColumnsFull)
                .Where("Entidade", Comparison.Equals, paramId)
                .Where("PotencialCliente", Comparison.Equals, "TRUE")
                .Where(new WhereClause("Vendedor", Comparison.Equals, sessionId).AddClause(LogicOperator.Or, Comparison.Equals, null))));
        }

        public static Reference LeadReference(string paramId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            if (PrimaveraEngine.Engine.CRM.EntidadesExternas.Existe(paramId) == false)
            {
                throw new NotFoundException();
            }

            return GenerateReference(PrimaveraEngine.Consulta(new SqlBuilder()
                .FromTable("ENTIDADESEXTERNAS")
                .Columns(sqlColumnsReference)
                .Where("ENTIDADESEXTERNAS.Entidade", Comparison.Equals, paramId)
                .Where("PotencialCliente", Comparison.Equals, "TRUE")));
        }

        private static void SetFields(CrmBEEntidadeExterna selectedRow, Lead paramObject)
        {
            selectedRow.set_Activo(paramObject.Active);
            selectedRow.set_PotencialCliente(true);

            if (paramObject.NomeFiscal != null)
            {
                selectedRow.set_Nome(paramObject.NomeFiscal.Trim());
            }

            if (paramObject.Email != null)
            {
                selectedRow.set_Email(paramObject.Email.Trim());
            }

            if (paramObject.Telefone != null)
            {
                selectedRow.set_Telefone(paramObject.Telefone.Trim());
            }

            if (paramObject.Telemovel != null)
            {
                selectedRow.set_Telemovel(paramObject.Telemovel.Trim());
            }

            if (paramObject.ModificadoEm != null)
            {
                selectedRow.set_DataUltAct(paramObject.ModificadoEm);
            }

            if (paramObject.Localizacao != null)
            {
                var objectLocation = paramObject.Localizacao;

                if (objectLocation.Morada != null)
                {
                    selectedRow.set_Morada(paramObject.Localizacao.Morada.Trim());
                }

                if (objectLocation.Distrito != null)
                {
                    selectedRow.set_Distrito(paramObject.Localizacao.Distrito.Trim());
                }

                if (objectLocation.Localidade != null)
                {
                    selectedRow.set_Localidade(paramObject.Localizacao.Localidade.Trim());
                }

                if (objectLocation.CodigoPostal != null)
                {
                    selectedRow.set_CodPostal(paramObject.Localizacao.CodigoPostal.Trim());
                }

                if (objectLocation.Pais != null)
                {
                    selectedRow.set_Pais(paramObject.Localizacao.Pais.Trim());
                }
            }
        }

        public static bool Update(string sessionUsername, Lead paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var selectedId = paramObject.Identficador;
            var selectedTable = PrimaveraEngine.Engine.CRM.EntidadesExternas;

            if (selectedTable.Existe(selectedId) == false)
            {
                return false;
            }

            var selectedRow = selectedTable.Edita(selectedId);

            selectedRow.set_EmModoEdicao(true);
            SetFields(selectedRow, paramObject);
            selectedTable.Actualiza(selectedRow);

            return true;
        }

        public static bool Insert(string sessionUsername, Lead paramObject)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            var selectedId = paramObject.Identficador;
            var selectedRow = new CrmBEEntidadeExterna();
            var selectedTable = PrimaveraEngine.Engine.CRM.EntidadesExternas;

            if (selectedTable.Existe(selectedId))
            {
                return false;
            }

            selectedRow.set_Entidade(selectedId);
            SetFields(selectedRow, paramObject);
            selectedTable.Actualiza(selectedRow);

            return true;
        }

        public static bool Delete(string sessionId, string paramId)
        {
            if (PrimaveraEngine.InitializeCompany(Properties.Settings.Default.Company.Trim(), Properties.Settings.Default.User.Trim(), Properties.Settings.Default.Password.Trim()) == false)
            {
                throw new DatabaseConnectionException();
            }

            System.Diagnostics.Debug.Print("TESTING DELETE METHOD!");

            return true;
        }
    }
}