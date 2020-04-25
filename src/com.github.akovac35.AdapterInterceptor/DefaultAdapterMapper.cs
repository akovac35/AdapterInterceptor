// Author: Aleksander Kovač

using AutoMapper;
using com.github.akovac35.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.github.akovac35.AdapterInterceptor
{
    public class DefaultAdapterMapper : IAdapterMapper
    {
        public DefaultAdapterMapper(IMapper mapper, ILoggerFactory? loggerFactory = null):this(loggerFactory)
        {
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected DefaultAdapterMapper(ILoggerFactory? loggerFactory = null)
        {
            _logger = (loggerFactory ?? LoggerFactoryProvider.LoggerFactory).CreateLogger<DefaultAdapterMapper>();

            Mapper = null!;
        }

        private readonly ILogger _logger;

        public IMapper Mapper { get; protected set; }

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
