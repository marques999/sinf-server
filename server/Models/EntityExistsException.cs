using System;

namespace FirstREST.Lib_Primavera.Model
{
    public class EntityExistsException : Exception
    {
        public EntityExistsException() : base("entityExists")
        {
        }
    }
}