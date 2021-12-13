using System;

namespace Core.Configurations.Extensions
{
    public static class EnumExtension
    {
        public static string GetDescription<T>(this Enum value) where T : Attribute
        {
            return Enum.GetName(typeof(T), value);
        }
    }
}