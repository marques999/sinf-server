using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstREST.LibPrimavera
{
    public class TypeValidator
    {
        internal static bool ValidateString(string str)
        {
            return string.IsNullOrWhiteSpace(str) == false;
        }
    }
}