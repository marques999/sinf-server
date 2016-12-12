using System;
using System.Collections.Generic;

using FirstREST.LibPrimavera.Model;

namespace FirstREST.LibPrimavera
{
    public class TypeParser
    {
        internal static string String(dynamic paramObject)
        {
            if (object.Equals(paramObject, null))
            {
                return "";
            }

            if (paramObject is string)
            {
                return paramObject;
            }

            try
            {
                return Convert.ToString(paramObject);
            }
            catch
            {
                return "";
            }
        }

        ///////////////////////////////////////////////////////////////////////

        internal static double Double(dynamic paramObject)
        {
            if (paramObject is double)
            {
                return paramObject;
            }

            try
            {
                return Convert.ToDouble(paramObject);
            }
            catch
            {
                return 0.0;
            }
        }

        internal static bool IsNumber(Type paramType)
        {
            if (paramType.IsPrimitive)
            {
                return paramType != typeof(bool) && paramType != typeof(char) && paramType != typeof(IntPtr) && paramType != typeof(UIntPtr);
            }

            return paramType == typeof(decimal);
        }

        ///////////////////////////////////////////////////////////////////////

        internal static DateTime Date(dynamic paramObject)
        {
            if (object.Equals(paramObject, null))
            {
                return DateTime.MinValue;
            }

            var myType = (paramObject as object).GetType();

            if (myType == typeof(DateTime))
            {
                return paramObject;
            }
            else if (myType == typeof(string))
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
            else if (IsNumber(myType))
            {
                try
                {
                    return DateTime.FromBinary((long)paramObject);
                }
                catch (FormatException)
                {
                    return DateTime.MinValue;
                }
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        ///////////////////////////////////////////////////////////////////////

        internal static bool Boolean(dynamic paramObject)
        {
            if (paramObject is bool)
            {
                return paramObject;
            }

            try
            {
                return Convert.ToBoolean(paramObject);
            }
            catch
            {
                return false;
            }
        }

        ///////////////////////////////////////////////////////////////////////

        internal static int Integer(dynamic paramObject)
        {
            if (paramObject is int)
            {
                return paramObject;
            }

            try
            {
                return Convert.ToInt32(paramObject);
            }
            catch
            {
                return 0;
            }
        }

        ///////////////////////////////////////////////////////////////////////

        private static Dictionary<String, EntityType> entities = new Dictionary<string, EntityType>
        {
            { "C", EntityType.Customer },
            { "N", EntityType.Unknown },
            { "O", EntityType.Contact },
            { "X", EntityType.Lead }
        };

        internal static EntityType Entity_Type(string value)
        {
            if (object.Equals(value, null))
            {
                return EntityType.Unknown;
            }

            if (value is string && entities.ContainsKey(value as string))
            {
                return entities[value];
            }

            return EntityType.Unknown;
        }
    }
}