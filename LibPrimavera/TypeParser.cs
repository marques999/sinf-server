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

        public static ActivityInterval Activity_Interval(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return ActivityInterval.Today;
            }

            ActivityInterval parseResult;

            return Enum.TryParse(value, true, out parseResult) ? parseResult : ActivityInterval.Today;
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////

        #region ActivityType Parser

        public static ActivityType Activity_Type(dynamic value)
        {
            if (object.Equals(value, null))
            {
                return ActivityType.ANY;
            }

            ActivityType parseResult = ActivityType.ANY;

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

        public static ActivityStatus Activity_Status(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return ActivityStatus.Any;
            }

            ActivityStatus parseResult;

            if (Enum.TryParse(value, true, out parseResult))
            {
                return parseResult;
            }

            return ActivityStatus.Any;
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////

        #region EntityType Parser

        public static EntityType Entity_Type(string value)
        {
            if (object.Equals(value, null))
            {
                return EntityType.N;
            }

            if (value is string)
            {
                EntityType parseResult;

                if (Enum.TryParse(value as string, true, out parseResult))
                {
                    return parseResult;
                }
            }

            return EntityType.N;
        }

        #endregion
    }
}