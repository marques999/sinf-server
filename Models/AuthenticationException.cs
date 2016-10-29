using System;

namespace FirstREST.LibPrimavera.Model
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException() : base("authenticationFailed")
        {
        }
    }
}