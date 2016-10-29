using System;

namespace FirstREST.LibPrimavera.Model
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : base("notFound")
        {
        }
    }
}