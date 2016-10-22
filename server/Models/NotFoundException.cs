using System;

namespace FirstREST.Lib_Primavera.Model
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : base("notFound")
        {
        }
    }
}