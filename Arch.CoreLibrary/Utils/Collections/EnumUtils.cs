using Arch.CoreLibrary.Utils.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CoreLibrary.Utils.Collections
{
    public static class EnumUtils
    {
        public static T ClearFlag<T>(this Enum variable, T flag)
        {
            return ClearFlags(variable, flag);
        }

        public static T ClearFlags<T>(this Enum variable, params T[] flags)
        {
            var result = Convert.ToUInt64(variable);
            foreach (T flag in flags)
                result &= ~Convert.ToUInt64(flag);
            return (T)Enum.Parse(variable.GetType(), result.ToString());
        }

        public static T SetFlag<T>(this Enum variable, T flag)
        {
            return SetFlags(variable, flag);
        }

        public static T SetFlags<T>(this Enum variable, params T[] flags)
        {
            var result = Convert.ToUInt64(variable);
            foreach (T flag in flags)
                result |= Convert.ToUInt64(flag);
            return (T)Enum.Parse(variable.GetType(), result.ToString());
        }

        public static bool HasFlags<TE>(this TE variable, params TE[] flags)
            where TE : struct, IComparable, IFormattable, IConvertible
        {
            if (!typeof(TE).IsEnum)
                throw new ArgumentException("variable must be an Enum", nameof(variable));

            foreach (var flag in flags)
            {
                if (!Enum.IsDefined(typeof(TE), flag))
                    return false;

                ulong numFlag = Convert.ToUInt64(flag);
                if ((Convert.ToUInt64(variable) & numFlag) != numFlag)
                    return false;
            }

            return true;
        }

        public static string DisplayString(this Enum value)
        {
            FieldInfo info = value.GetType().GetField(value.ToString());
            var attributes = (DisplayStringAttribute[])info.GetCustomAttributes(typeof(DisplayStringAttribute), false);
            return attributes.Length >= 1 ? attributes[0].DisplayString : value.ToString();
        }

        /*
        List<DayOfWeek> weekdays =
        EnumHelper.EnumToList<DayOfWeek>().FindAll(
        delegate (DayOfWeek x)
        {
            return x != DayOfWeek.Sunday && x != DayOfWeek.Saturday;
        });
         */
        public static List<T> EnumToList<T>()
        {
            Type enumType = typeof(T);

            // Can't use type constraints on value types, so have to do check like this
            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T must be of type System.Enum");

            Array enumValArray = Enum.GetValues(enumType);

            List<T> enumValList = new List<T>(enumValArray.Length);

            foreach (int val in enumValArray)
            {
                enumValList.Add((T)Enum.Parse(enumType, val.ToString()));
            }

            return enumValList;
        }

        public static string GetEnumDescription<T>(string value)
        {
            Type type = typeof(T);
            var name = Enum.GetNames(type).Where(f => f.Equals(value, StringComparison.CurrentCultureIgnoreCase)).Select(d => d).FirstOrDefault();

            if (name == null)
            {
                return string.Empty;
            }
            var field = type.GetField(name);
            var customAttribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return customAttribute.Length > 0 ? ((DescriptionAttribute)customAttribute[0]).Description : name;
        }

        public static T Parse<T>(string value)
        {
            return Parse<T>(value, true);
        }

        public static T Parse<T>(string value, bool ignoreCase)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static bool TryParse<T>(string value, out T returnedValue)
        {
            return TryParse(value, true, out returnedValue);
        }

        public static bool TryParse<T>(string value, bool ignoreCase, out T returnedValue)
        {
            try
            {
                returnedValue = (T)Enum.Parse(typeof(T), value, ignoreCase);
                return true;
            }
            catch
            {
                returnedValue = default(T);
                return false;
            }
        }
    }
}
