using System;
using System.Linq;
using System.Reflection;

namespace Downlink.Infrastructure
{
    public static class ReflectionExtensions
    {
        public static bool IsAssignableTo(this Type type, Type otherType)
        {
            var typeInfo = type.GetTypeInfo();
            var otherTypeInfo = otherType.GetTypeInfo();

            if (otherTypeInfo.IsGenericTypeDefinition)
            {
                if (typeInfo.IsGenericTypeDefinition)
                {
                    return typeInfo.Equals(otherTypeInfo);
                }

                return typeInfo.IsAssignableToGenericTypeDefinition(otherTypeInfo);
            }

            return otherTypeInfo.IsAssignableFrom(typeInfo);
        }

        public static bool IsAssignableTo<TTarget>(this object o) {
            var otherTypeInfo = typeof(TTarget);
            var typeInfo = o.GetType();
            return typeInfo.IsAssignableTo(otherTypeInfo);
        }

        private static bool IsAssignableToGenericTypeDefinition(this TypeInfo typeInfo, TypeInfo genericTypeInfo)
        {
            var interfaceTypes = typeInfo.ImplementedInterfaces.Select(t => t.GetTypeInfo());

            foreach (var interfaceType in interfaceTypes)
            {
                if (interfaceType.IsGenericType)
                {
                    var typeDefinitionTypeInfo = interfaceType
                        .GetGenericTypeDefinition()
                        .GetTypeInfo();

                    if (typeDefinitionTypeInfo.Equals(genericTypeInfo))
                    {
                        return true;
                    }
                }
            }

            if (typeInfo.IsGenericType)
            {
                var typeDefinitionTypeInfo = typeInfo
                    .GetGenericTypeDefinition()
                    .GetTypeInfo();

                if (typeDefinitionTypeInfo.Equals(genericTypeInfo))
                {
                    return true;
                }
            }

            var baseTypeInfo = typeInfo.BaseType?.GetTypeInfo();

            if (baseTypeInfo == null)
            {
                return false;
            }

            return baseTypeInfo.IsAssignableToGenericTypeDefinition(genericTypeInfo);
        }
    }
}