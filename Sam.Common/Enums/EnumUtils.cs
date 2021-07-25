using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Reflection;




namespace Sam.Common.Enums
{
    /// <summary>
    /// Classe retirada de http://stackoverflow.com/questions/4249632/string-to-enum-with-description @20/02/2015 12:07
    /// </summary>
    public static class EnumUtils
    {
        public static Nullable<T> Parse<T>(string input) where T : struct
        {
            //since we cant do a generic type constraint
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Generic Type 'T' must be an Enum");
            }
            if (!string.IsNullOrEmpty(input))
            {
                if (Enum.GetNames(typeof(T)).Any(
                      e => e.Trim().ToUpperInvariant() == input.Trim().ToUpperInvariant()))
                {
                    return (T)Enum.Parse(typeof(T), input, true);
                }
            }
            return null;
        }

        public static string GetEnumDescription<T>(T value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
        public static string GetEnumExtraDescription<T>(T value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            ExtraDescriptionAttribute[] attributes = (ExtraDescriptionAttribute[])fi.GetCustomAttributes(typeof(ExtraDescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static T ParseDescriptionToEnum<T>(string description)
        {
            Array array = Enum.GetValues(typeof(T));
            var list = new List<T>(array.Length);

            for (int i = 0; i < array.Length; i++)
                list.Add((T)array.GetValue(i));

            var dict = list.Select(v => new
            {
                Value = v,
                Description = GetEnumDescription(v)}).ToDictionary(x => x.Description, x => x.Value);

            return dict[description];
        }
        public static T ParseExtraDescriptionToEnum<T>(string description)
        {
            Array array = Enum.GetValues(typeof(T));
            var list = new List<T>(array.Length);

            for (int i = 0; i < array.Length; i++)
                list.Add((T)array.GetValue(i));

            var dict = list.Select(v => new
            {
                Value = v,
                Description = GetEnumExtraDescription(v)
            }).ToDictionary(x => x.Description, x => x.Value);

            return dict[description];
        }
    }
}
