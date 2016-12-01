using System;

namespace FirstREST.LibPrimavera.Model
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entityType, bool gender) : base((gender ? "A " : "O ") + entityType + " que seleccionou não existe no sistema!")
        {
        }
    }
}