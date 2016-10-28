using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstREST.Lib_Primavera
{
    public class TypeParser
    {
        internal static string String(dynamic paramObject)
        {
            if (paramObject is string)
            {
                return paramObject.Trim();
            }

            return "";
        }

        internal static double Double(dynamic paramObject)
        {
            if (paramObject is string)
            {
                return Convert.ToDouble(paramObject);
            }

            if (paramObject is double)
            {
                return paramObject;
            }

            return 0.0;
        }

        internal static DateTime Date(dynamic paramObject)
        {
            if (paramObject is string)
            {
                if (string.IsNullOrEmpty(paramObject))
                {
                    return DateTime.MinValue;
                }

                try
                {
                    return DateTime.ParseExact(paramObject.Trim(), "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.CurrentCulture);
                }
                catch (FormatException)
                {
                    return DateTime.MinValue;
                }
            }
            else
            {
                return paramObject;
            }
        }

        internal static bool Boolean(dynamic paramObject)
        {
            if (paramObject is string)
            {
                return Convert.ToBoolean(paramObject);
            }

            if (paramObject is double)
            {
                return paramObject;
            }

            return false;
        }
    }
}