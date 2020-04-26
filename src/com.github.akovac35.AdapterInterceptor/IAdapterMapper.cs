// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using System;

namespace com.github.akovac35.AdapterInterceptor
{
    /// <summary>
    /// Represents a type used to perform mapping from the source object to a new destination object. Must support reverse mapping for method invocation result mapping.
    /// </summary>
    public interface IAdapterMapper
    {
        /// <summary>
        /// Execute a mapping from the source object to a new destination object.
        /// </summary>
        /// <param name="source">Source object.</param>
        /// <param name="sourceType">Source object type.</param>
        /// <param name="destinationType">Destination object type.</param>
        /// <returns>New mapped destination object.</returns>
        object? Map(object? source, Type sourceType, Type destinationType);
    }
}
