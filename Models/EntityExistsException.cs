using System;

namespace FirstREST.LibPrimavera.Model
{
    public class EntityExistsException : Exception
    {
        public EntityExistsException(string entityType, bool gender) : base((gender ? "Uma " : "Um ") + entityType + " com este identificador já existe no sistema!")
        {
        }
    }
}