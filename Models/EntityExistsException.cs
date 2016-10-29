using System;

namespace FirstREST.LibPrimavera.Model
{
    public class EntityExistsException : Exception
    {
        public EntityExistsException() : base("entityExists")
        {
        }
    }
}