using System;

namespace FirstREST.Lib_Primavera.Model
{
    public class DatabaseConnectionException : Exception
    {
        public DatabaseConnectionException() : base("databaseError")
        {
        }
    }
}