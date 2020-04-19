// Author: Aleksander Kovač

using System;

namespace com.github.akovac35.AdapterInterceptor
{
    public interface IAdapterMapper
    {
        object? Map(object? source, Type sourceType, Type destinationType);
    }
}
