// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using System;

namespace com.github.akovac35.AdapterInterceptor
{
    /// <summary>
    /// An implementation of IAdapterMapper which simply assumes that source and destination types are the same and that no mapping is required.
	/// It passes reference types by reference and value types by value.
    /// </summary>
    public class NoOperationAdapterMapper : IAdapterMapper
    {
        /// <summary>
        /// Initializes a new NoOperationAdapterMapper instance.
        /// </summary>
        public NoOperationAdapterMapper()
        {
        }

        public virtual object? Map(object? source, Type sourceType, Type destinationType)
        {
            object? destination = null;
            if (sourceType == destinationType)
            {
                destination = source;
            }
            else
            {
                throw new InvalidOperationException($"{nameof(NoOperationAdapterMapper)} assumes that source and destination types are the same, which is currently not the case. Source type is {sourceType.FullName} and destination type is {destinationType.FullName}.");
            }

            return destination;
        }
    }
}
