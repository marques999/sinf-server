using System;

namespace FirstREST.LibPrimavera.Model
{
    public class DatabaseConnectionException : Exception
    {
        public DatabaseConnectionException() : base("databaseError")
        {
        }
    }
}