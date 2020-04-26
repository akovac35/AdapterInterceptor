// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using AutoMapper;
using System;

namespace com.github.akovac35.AdapterInterceptor.Default
{
    /// <summary>
    /// Default type used to perform mapping from the source object to a new destination object using AutoMapper.
    /// </summary>
    public class DefaultAdapterMapper : IAdapterMapper
    {
        /// <summary>
        /// Initializes a new DefaultAdapterMapper instance.
        /// </summary>
        /// <param name="mapper">AutoMapper instance used for object mapping. Must support reverse mapping for method invocation result mapping.</param>
        public DefaultAdapterMapper(IMapper mapper) : this()
        {
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected DefaultAdapterMapper()
        {
            Mapper = null!;
        }

        protected IMapper Mapper { get; set; }

        public virtual object? Map(object? source, Type sourceType, Type destinationType)
        {
            object? destination = null;
            if (sourceType == destinationType)
            {
                destination = source;
            }
            else
            {
                destination = Mapper.Map(source, sourceType.IsByRef ? sourceType.GetElementType() : sourceType, destinationType.IsByRef ? destinationType.GetElementType() : destinationType);
            }

            return destination;
        }
    }
}
