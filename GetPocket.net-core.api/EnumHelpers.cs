using System;
using System.Reflection;

namespace Pocket.api
{
    public static class EnumHelpers
    {
        public static string GetStringValue(Enum value)
        {
            string output = null;
            Type type = value.GetType();

            FieldInfo fi = type.GetField(value.ToString());
            var attrs =
               fi.GetCustomAttributes(typeof(StringValueAttribute),
                                       false) as StringValueAttribute[];
            if (attrs != null && attrs.Length > 0)
            {
                output = attrs[0].Value;
            }

            return output;
        }
    }

    public class StringValueAttribute : Attribute
    {
        public StringValueAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
