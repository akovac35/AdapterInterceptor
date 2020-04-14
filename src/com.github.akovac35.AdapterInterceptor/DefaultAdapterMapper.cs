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
    // TODO: read supportedTypePairs from IMapper

    public class DefaultAdapterMapper : IAdapterMapper
    {
        public DefaultAdapterMapper(IMapper mapper, IList<TypePair> supportedTypePairs, ILoggerFactory? loggerFactory = null)
        {
            _logger = (loggerFactory ?? LoggerFactoryProvider.LoggerFactory).CreateLogger<DefaultAdapterMapper>();

            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            SupportedTypePairs = supportedTypePairs ?? throw new ArgumentNullException(nameof(supportedTypePairs));
        }

        private ILogger _logger;

        public IList<TypePair> SupportedTypePairs { get; protected set; }

        public IMapper Mapper { get; protected set; }

        public virtual object? Map(object? source, Type sourceType, Type destinationType)
        {
            if (_logger.IsEnteringExitingEnabled()) _logger.Here(l => l.Entering(source, sourceType.ToLoggerString(simpleType: true), destinationType.ToLoggerString(simpleType: true)));

            object? destination = null;
            if (sourceType == destinationType)
            {
                destination = source;
            }
            else
            {
                destination = Mapper.Map(source, sourceType, destinationType);
            }

            _logger.Here(l => l.Exiting(destination));
            return destination;
        }

        public virtual Type[] MapSupportedTypes(Type[] sourceTypes)
        {
            if (_logger.IsEnteringExitingEnabled()) _logger.Here(l => l.Entering(sourceTypes.ToLoggerString(simpleType: true)));

            Type[] destinationTypes = new Type[sourceTypes.Length];
            for (int i = 0; i < destinationTypes.Length; i++)
            {
                bool isMapped = false;

                var supportedTypePairs = SupportedTypePairs.Where(item => item.SourceType == sourceTypes[i]).ToList();
                if (supportedTypePairs.Count > 0)
                {
                    destinationTypes[i] = supportedTypePairs.First().DestinationType;
                    isMapped = true;
                }

                if (!isMapped) destinationTypes[i] = sourceTypes[i];
            }

            if (_logger.IsEnteringExitingEnabled()) _logger.Here(l => l.Exiting(destinationTypes.ToLoggerString(simpleType: true)));
            return destinationTypes;
        }

        public virtual MethodInfo MapTargetMethod(Type targetType, MethodInfo sourceMethod, Type[] destinationTypes)
        {
            if (_logger.IsEnteringExitingEnabled()) _logger.Here(l => l.Entering(targetType.ToLoggerString(simpleType: true), sourceMethod.ToLoggerString(simpleType: true), destinationTypes.ToLoggerString(simpleType: true)));

            MethodInfo targetMethodInfo = targetType.GetMethod(sourceMethod.Name, destinationTypes);

            if (targetMethodInfo == null) throw new AdapterInterceptorException($"Target method cannot be found. This is likely due to missing supported type pairs or because target type changed. Target type: {targetType.ToLoggerString()} Target method: {sourceMethod.ToLoggerString()} Target method parameter types: {destinationTypes.ToLoggerString()}");

            if (_logger.IsEnteringExitingEnabled()) _logger.Here(l => l.Exiting(targetMethodInfo.ToLoggerString(simpleType: true)));
            return targetMethodInfo;
        }
    }
}
