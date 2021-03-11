using System;

namespace DevExpressGrid.Extensions.Helpers
{
    internal static class TypeHelper
    {
        public static Type ToNullableType(this Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type.IsValueType)
                return typeof(Nullable<>).MakeGenericType(type);
            else
                return type;
        }

        public static bool IsNullableType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }
    }
}
