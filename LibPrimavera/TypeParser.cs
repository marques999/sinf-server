using FirstREST.LibPrimavera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstREST.LibPrimavera
{
    public class TypeParser
    {
        #region String Parser

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

        internal static string ToString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////

        #region Double Parser

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

        public static bool IsNumber(Type paramType)
        {
            if (paramType.IsPrimitive)
            {
                return paramType != typeof(bool) && paramType != typeof(char) && paramType != typeof(IntPtr) && paramType != typeof(UIntPtr);
            }

            return paramType == typeof(decimal);
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////

        #region Date Parser

        private static DateTime DefaultDate = DateTime.MinValue;

        internal static DateTime Date(dynamic paramObject)
        {
            if (object.Equals(paramObject, null))
            {
                return DefaultDate;
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
                    return DefaultDate;
                }

                try
                {
                    return DateTime.ParseExact(paramObject.Trim(), "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.CurrentCulture);
                }
                catch (FormatException)
                {
                    return DefaultDate;
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
                    return DefaultDate;
                }
            }
            else
            {
                return DefaultDate;
            }
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////

        #region Boolean Parser

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

        #endregion

        ///////////////////////////////////////////////////////////////////////

        #region Integer Parser

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

        #endregion

        ///////////////////////////////////////////////////////////////////////

        #region ActivityInterval Parser

        public static EnumActivityInterval Activity_Interval(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return EnumActivityInterval.Today;
            }

            EnumActivityInterval parseResult;

            return Enum.TryParse(value, true, out parseResult) ? parseResult : EnumActivityInterval.Today;
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////

        #region ActivityType Parser

        public static EnumActivityType Activity_Type(dynamic value)
        {
            if (object.Equals(value, null))
            {
                return EnumActivityType.ANY;
            }

            EnumActivityType parseResult = EnumActivityType.ANY;

            if (value is string)
            {
                if (Enum.TryParse(value as string, true, out parseResult))
                {
                    return parseResult;
                }
            }

            return parseResult;
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////

        #region ActivityStatus Parser

        public static EnumActivityStatus Activity_Status(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return EnumActivityStatus.Any;
            }

            EnumActivityStatus parseResult;

            if (Enum.TryParse(value, true, out parseResult))
            {
                return parseResult;
            }

            return EnumActivityStatus.Any;
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////

        #region EntityType Parser

        private static Dictionary<String, EntityType> entities = new Dictionary<string, EntityType>
        {
            { "C", EntityType.Customer },
            { "N", EntityType.Unknown },
            { "O", EntityType.Contact },
            { "X", EntityType.Lead }
        };

        public static EntityType Entity_Type(string value)
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

        #endregion
    }
}