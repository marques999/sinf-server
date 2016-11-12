using System;

namespace FirstREST.LibPrimavera.Model
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() : base("authenticationFailed")
        {
        }
    }
}