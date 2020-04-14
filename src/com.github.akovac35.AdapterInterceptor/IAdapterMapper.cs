// Author: Aleksander Kovač

using System;
using System.Reflection;

namespace com.github.akovac35.AdapterInterceptor
{
    public interface IAdapterMapper
    {
        object? Map(object? source, Type sourceType, Type destinationType);

        Type[] MapSupportedTypes(Type[] sourceTypes);

        MethodInfo MapTargetMethod(Type targetType, MethodInfo sourceMethod, Type[] destinationTypes);
    }
}
