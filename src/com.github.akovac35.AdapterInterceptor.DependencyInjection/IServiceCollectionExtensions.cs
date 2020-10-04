// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace com.github.akovac35.AdapterInterceptor.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add proxy imitator to the service collection.
        /// Proxy imitator is an adapter which imitates a proxy - no type mapping is performed and inputs are passed by reference for
        /// reference types and as value for value types. When compared to a real proxy, the drawback is a slower invocation 
        /// performance because of using reflection, but the benefit is that virtual methods are not required on the target.
        /// </summary>
        /// <typeparam name="TProxyInterface">The proxy imitator interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the proxy imitator target.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the proxy imitator in an <see cref="IServiceCollection"/>.</param>
        /// <param name="adapterMapperFactory">Optional, a factory which generates an instance of <see cref="IAdapterMapper"/> for the proxy imitator.</param>
        /// <param name="useLoggerFactory">Optional, whether to enable logging.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddProxyImitator<TProxyInterface, TTarget>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, Func<IServiceProvider, IAdapterMapper>? adapterMapperFactory = null, bool useLoggerFactory = true)
            where TProxyInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TProxyInterface), fact =>
            {
                IAdapterMapper adapterMapper = adapterMapperFactory == null ? new NoOperationAdapterMapper() : adapterMapperFactory(fact);
                TTarget target = targetFactory(fact);
                ILoggerFactory? loggerFactory = useLoggerFactory ? fact.GetService<ILoggerFactory>() : null;
                var imitator = target.GenerateProxyImitator<TProxyInterface, TTarget>(adapterMapper, loggerFactory);
                return imitator;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }

        /// <summary>
        /// Add adapter to the service collection.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the adapter target.</param>
        /// <param name="adapterMapperFactory">The factory which generates an instance of <see cref="IAdapterMapper"/> for the adapter.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the adapter in an <see cref="IServiceCollection"/>.</param>
        /// <param name="useLoggerFactory">Optional, whether to enable logging.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAdapter<TAdapterInterface, TTarget, TSource1, TDestination1>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, Func<IServiceProvider, IAdapterMapper> adapterMapperFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, bool useLoggerFactory = true)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TAdapterInterface), fact =>
            {
                IAdapterMapper adapterMapper = adapterMapperFactory(fact);
                TTarget target = targetFactory(fact);
                ILoggerFactory? loggerFactory = useLoggerFactory ? fact.GetService<ILoggerFactory>() : null;
                var adapter = target.GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1>(adapterMapper, loggerFactory);
                return adapter;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }

        /// <summary>
        /// Add adapter to the service collection.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination2">Destination type 2.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the adapter target.</param>
        /// <param name="adapterMapperFactory">The factory which generates an instance of <see cref="IAdapterMapper"/> for the adapter.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the adapter in an <see cref="IServiceCollection"/>.</param>
        /// <param name="useLoggerFactory">Optional, whether to enable logging.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, Func<IServiceProvider, IAdapterMapper> adapterMapperFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, bool useLoggerFactory = true)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TAdapterInterface), fact =>
            {
                IAdapterMapper adapterMapper = adapterMapperFactory(fact);
                TTarget target = targetFactory(fact);
                ILoggerFactory? loggerFactory = useLoggerFactory ? fact.GetService<ILoggerFactory>() : null;
                var adapter = target.GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2>(adapterMapper, loggerFactory);
                return adapter;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }

        /// <summary>
        /// Add adapter to the service collection.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination2">Destination type 2.</typeparam>
        /// <typeparam name="TSource3">Source type 3. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination3">Destination type 3.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the adapter target.</param>
        /// <param name="adapterMapperFactory">The factory which generates an instance of <see cref="IAdapterMapper"/> for the adapter.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the adapter in an <see cref="IServiceCollection"/>.</param>
        /// <param name="useLoggerFactory">Optional, whether to enable logging.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, Func<IServiceProvider, IAdapterMapper> adapterMapperFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, bool useLoggerFactory = true)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TAdapterInterface), fact =>
            {
                IAdapterMapper adapterMapper = adapterMapperFactory(fact);
                TTarget target = targetFactory(fact);
                ILoggerFactory? loggerFactory = useLoggerFactory ? fact.GetService<ILoggerFactory>() : null;
                var adapter = target.GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3>(adapterMapper, loggerFactory);
                return adapter;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }

        /// <summary>
        /// Add adapter to the service collection.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination2">Destination type 2.</typeparam>
        /// <typeparam name="TSource3">Source type 3. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination3">Destination type 3.</typeparam>
        /// <typeparam name="TSource4">Source type 4. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination4">Destination type 4.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the adapter target.</param>
        /// <param name="adapterMapperFactory">The factory which generates an instance of <see cref="IAdapterMapper"/> for the adapter.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the adapter in an <see cref="IServiceCollection"/>.</param>
        /// <param name="useLoggerFactory">Optional, whether to enable logging.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, Func<IServiceProvider, IAdapterMapper> adapterMapperFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, bool useLoggerFactory = true)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TAdapterInterface), fact =>
            {
                IAdapterMapper adapterMapper = adapterMapperFactory(fact);
                TTarget target = targetFactory(fact);
                ILoggerFactory? loggerFactory = useLoggerFactory ? fact.GetService<ILoggerFactory>() : null;
                var adapter = target.GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4>(adapterMapper, loggerFactory);
                return adapter;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }

        /// <summary>
        /// Add adapter to the service collection.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination2">Destination type 2.</typeparam>
        /// <typeparam name="TSource3">Source type 3. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination3">Destination type 3.</typeparam>
        /// <typeparam name="TSource4">Source type 4. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination4">Destination type 4.</typeparam>
        /// <typeparam name="TSource5">Source type 5. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination5">Destination type 5.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the adapter target.</param>
        /// <param name="adapterMapperFactory">The factory which generates an instance of <see cref="IAdapterMapper"/> for the adapter.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the adapter in an <see cref="IServiceCollection"/>.</param>
        /// <param name="useLoggerFactory">Optional, whether to enable logging.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, Func<IServiceProvider, IAdapterMapper> adapterMapperFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, bool useLoggerFactory = true)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TAdapterInterface), fact =>
            {
                IAdapterMapper adapterMapper = adapterMapperFactory(fact);
                TTarget target = targetFactory(fact);
                ILoggerFactory? loggerFactory = useLoggerFactory ? fact.GetService<ILoggerFactory>() : null;
                var adapter = target.GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5>(adapterMapper, loggerFactory);
                return adapter;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }

        /// <summary>
        /// Add adapter to the service collection.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination2">Destination type 2.</typeparam>
        /// <typeparam name="TSource3">Source type 3. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination3">Destination type 3.</typeparam>
        /// <typeparam name="TSource4">Source type 4. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination4">Destination type 4.</typeparam>
        /// <typeparam name="TSource5">Source type 5. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination5">Destination type 5.</typeparam>
        /// <typeparam name="TSource6">Source type 6. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination6">Destination type 6.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the adapter target.</param>
        /// <param name="adapterMapperFactory">The factory which generates an instance of <see cref="IAdapterMapper"/> for the adapter.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the adapter in an <see cref="IServiceCollection"/>.</param>
        /// <param name="useLoggerFactory">Optional, whether to enable logging.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, Func<IServiceProvider, IAdapterMapper> adapterMapperFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, bool useLoggerFactory = true)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TAdapterInterface), fact =>
            {
                IAdapterMapper adapterMapper = adapterMapperFactory(fact);
                TTarget target = targetFactory(fact);
                ILoggerFactory? loggerFactory = useLoggerFactory ? fact.GetService<ILoggerFactory>() : null;
                var adapter = target.GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6>(adapterMapper, loggerFactory);
                return adapter;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }

        /// <summary>
        /// Add adapter to the service collection.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination2">Destination type 2.</typeparam>
        /// <typeparam name="TSource3">Source type 3. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination3">Destination type 3.</typeparam>
        /// <typeparam name="TSource4">Source type 4. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination4">Destination type 4.</typeparam>
        /// <typeparam name="TSource5">Source type 5. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination5">Destination type 5.</typeparam>
        /// <typeparam name="TSource6">Source type 6. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination6">Destination type 6.</typeparam>
        /// <typeparam name="TSource7">Source type 7. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination7">Destination type 7.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the adapter target.</param>
        /// <param name="adapterMapperFactory">The factory which generates an instance of <see cref="IAdapterMapper"/> for the adapter.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the adapter in an <see cref="IServiceCollection"/>.</param>
        /// <param name="useLoggerFactory">Optional, whether to enable logging.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, Func<IServiceProvider, IAdapterMapper> adapterMapperFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, bool useLoggerFactory = true)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TAdapterInterface), fact =>
            {
                IAdapterMapper adapterMapper = adapterMapperFactory(fact);
                TTarget target = targetFactory(fact);
                ILoggerFactory? loggerFactory = useLoggerFactory ? fact.GetService<ILoggerFactory>() : null;
                var adapter = target.GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7>(adapterMapper, loggerFactory);
                return adapter;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }

        /// <summary>
        /// Add adapter to the service collection.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination2">Destination type 2.</typeparam>
        /// <typeparam name="TSource3">Source type 3. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination3">Destination type 3.</typeparam>
        /// <typeparam name="TSource4">Source type 4. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination4">Destination type 4.</typeparam>
        /// <typeparam name="TSource5">Source type 5. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination5">Destination type 5.</typeparam>
        /// <typeparam name="TSource6">Source type 6. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination6">Destination type 6.</typeparam>
        /// <typeparam name="TSource7">Source type 7. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination7">Destination type 7.</typeparam>
        /// <typeparam name="TSource8">Source type 8. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination8">Destination type 8.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the adapter target.</param>
        /// <param name="adapterMapperFactory">The factory which generates an instance of <see cref="IAdapterMapper"/> for the adapter.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the adapter in an <see cref="IServiceCollection"/>.</param>
        /// <param name="useLoggerFactory">Optional, whether to enable logging.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, Func<IServiceProvider, IAdapterMapper> adapterMapperFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, bool useLoggerFactory = true)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TAdapterInterface), fact =>
            {
                IAdapterMapper adapterMapper = adapterMapperFactory(fact);
                TTarget target = targetFactory(fact);
                ILoggerFactory? loggerFactory = useLoggerFactory ? fact.GetService<ILoggerFactory>() : null;
                var adapter = target.GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8>(adapterMapper, loggerFactory);
                return adapter;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }

        /// <summary>
        /// Add adapter to the service collection.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination2">Destination type 2.</typeparam>
        /// <typeparam name="TSource3">Source type 3. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination3">Destination type 3.</typeparam>
        /// <typeparam name="TSource4">Source type 4. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination4">Destination type 4.</typeparam>
        /// <typeparam name="TSource5">Source type 5. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination5">Destination type 5.</typeparam>
        /// <typeparam name="TSource6">Source type 6. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination6">Destination type 6.</typeparam>
        /// <typeparam name="TSource7">Source type 7. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination7">Destination type 7.</typeparam>
        /// <typeparam name="TSource8">Source type 8. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination8">Destination type 8.</typeparam>
        /// <typeparam name="TSource9">Source type 9. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination9">Destination type 9.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the adapter target.</param>
        /// <param name="adapterMapperFactory">The factory which generates an instance of <see cref="IAdapterMapper"/> for the adapter.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the adapter in an <see cref="IServiceCollection"/>.</param>
        /// <param name="useLoggerFactory">Optional, whether to enable logging.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, Func<IServiceProvider, IAdapterMapper> adapterMapperFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, bool useLoggerFactory = true)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TAdapterInterface), fact =>
            {
                IAdapterMapper adapterMapper = adapterMapperFactory(fact);
                TTarget target = targetFactory(fact);
                ILoggerFactory? loggerFactory = useLoggerFactory ? fact.GetService<ILoggerFactory>() : null;
                var adapter = target.GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9>(adapterMapper, loggerFactory);
                return adapter;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }

        /// <summary>
        /// Add adapter to the service collection.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination2">Destination type 2.</typeparam>
        /// <typeparam name="TSource3">Source type 3. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination3">Destination type 3.</typeparam>
        /// <typeparam name="TSource4">Source type 4. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination4">Destination type 4.</typeparam>
        /// <typeparam name="TSource5">Source type 5. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination5">Destination type 5.</typeparam>
        /// <typeparam name="TSource6">Source type 6. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination6">Destination type 6.</typeparam>
        /// <typeparam name="TSource7">Source type 7. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination7">Destination type 7.</typeparam>
        /// <typeparam name="TSource8">Source type 8. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination8">Destination type 8.</typeparam>
        /// <typeparam name="TSource9">Source type 9. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination9">Destination type 9.</typeparam>
        /// <typeparam name="TSource10">Source type 10. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination10">Destination type 10.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the adapter target.</param>
        /// <param name="adapterMapperFactory">The factory which generates an instance of <see cref="IAdapterMapper"/> for the adapter.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the adapter in an <see cref="IServiceCollection"/>.</param>
        /// <param name="useLoggerFactory">Optional, whether to enable logging.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, Func<IServiceProvider, IAdapterMapper> adapterMapperFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, bool useLoggerFactory = true)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TAdapterInterface), fact =>
            {
                IAdapterMapper adapterMapper = adapterMapperFactory(fact);
                TTarget target = targetFactory(fact);
                ILoggerFactory? loggerFactory = useLoggerFactory ? fact.GetService<ILoggerFactory>() : null;
                var adapter = target.GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10>(adapterMapper, loggerFactory);
                return adapter;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }

        /// <summary>
        /// Add adapter to the service collection.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination2">Destination type 2.</typeparam>
        /// <typeparam name="TSource3">Source type 3. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination3">Destination type 3.</typeparam>
        /// <typeparam name="TSource4">Source type 4. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination4">Destination type 4.</typeparam>
        /// <typeparam name="TSource5">Source type 5. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination5">Destination type 5.</typeparam>
        /// <typeparam name="TSource6">Source type 6. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination6">Destination type 6.</typeparam>
        /// <typeparam name="TSource7">Source type 7. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination7">Destination type 7.</typeparam>
        /// <typeparam name="TSource8">Source type 8. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination8">Destination type 8.</typeparam>
        /// <typeparam name="TSource9">Source type 9. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination9">Destination type 9.</typeparam>
        /// <typeparam name="TSource10">Source type 10. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination10">Destination type 10.</typeparam>
        /// <typeparam name="TSource11">Source type 11. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination11">Destination type 11.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the adapter target.</param>
        /// <param name="adapterMapperFactory">The factory which generates an instance of <see cref="IAdapterMapper"/> for the adapter.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the adapter in an <see cref="IServiceCollection"/>.</param>
        /// <param name="useLoggerFactory">Optional, whether to enable logging.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, Func<IServiceProvider, IAdapterMapper> adapterMapperFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, bool useLoggerFactory = true)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TAdapterInterface), fact =>
            {
                IAdapterMapper adapterMapper = adapterMapperFactory(fact);
                TTarget target = targetFactory(fact);
                ILoggerFactory? loggerFactory = useLoggerFactory ? fact.GetService<ILoggerFactory>() : null;
                var adapter = target.GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11>(adapterMapper, loggerFactory);
                return adapter;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }

        /// <summary>
        /// Add adapter to the service collection.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination2">Destination type 2.</typeparam>
        /// <typeparam name="TSource3">Source type 3. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination3">Destination type 3.</typeparam>
        /// <typeparam name="TSource4">Source type 4. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination4">Destination type 4.</typeparam>
        /// <typeparam name="TSource5">Source type 5. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination5">Destination type 5.</typeparam>
        /// <typeparam name="TSource6">Source type 6. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination6">Destination type 6.</typeparam>
        /// <typeparam name="TSource7">Source type 7. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination7">Destination type 7.</typeparam>
        /// <typeparam name="TSource8">Source type 8. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination8">Destination type 8.</typeparam>
        /// <typeparam name="TSource9">Source type 9. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination9">Destination type 9.</typeparam>
        /// <typeparam name="TSource10">Source type 10. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination10">Destination type 10.</typeparam>
        /// <typeparam name="TSource11">Source type 11. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination11">Destination type 11.</typeparam>
        /// <typeparam name="TSource12">Source type 12. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination12">Destination type 12.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the adapter target.</param>
        /// <param name="adapterMapperFactory">The factory which generates an instance of <see cref="IAdapterMapper"/> for the adapter.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the adapter in an <see cref="IServiceCollection"/>.</param>
        /// <param name="useLoggerFactory">Optional, whether to enable logging.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, Func<IServiceProvider, IAdapterMapper> adapterMapperFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, bool useLoggerFactory = true)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TAdapterInterface), fact =>
            {
                IAdapterMapper adapterMapper = adapterMapperFactory(fact);
                TTarget target = targetFactory(fact);
                ILoggerFactory? loggerFactory = useLoggerFactory ? fact.GetService<ILoggerFactory>() : null;
                var adapter = target.GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12>(adapterMapper, loggerFactory);
                return adapter;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }

        /// <summary>
        /// Add adapter to the service collection.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination2">Destination type 2.</typeparam>
        /// <typeparam name="TSource3">Source type 3. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination3">Destination type 3.</typeparam>
        /// <typeparam name="TSource4">Source type 4. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination4">Destination type 4.</typeparam>
        /// <typeparam name="TSource5">Source type 5. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination5">Destination type 5.</typeparam>
        /// <typeparam name="TSource6">Source type 6. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination6">Destination type 6.</typeparam>
        /// <typeparam name="TSource7">Source type 7. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination7">Destination type 7.</typeparam>
        /// <typeparam name="TSource8">Source type 8. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination8">Destination type 8.</typeparam>
        /// <typeparam name="TSource9">Source type 9. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination9">Destination type 9.</typeparam>
        /// <typeparam name="TSource10">Source type 10. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination10">Destination type 10.</typeparam>
        /// <typeparam name="TSource11">Source type 11. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination11">Destination type 11.</typeparam>
        /// <typeparam name="TSource12">Source type 12. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination12">Destination type 12.</typeparam>
        /// <typeparam name="TSource13">Source type 13. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination13">Destination type 13.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the adapter target.</param>
        /// <param name="adapterMapperFactory">The factory which generates an instance of <see cref="IAdapterMapper"/> for the adapter.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the adapter in an <see cref="IServiceCollection"/>.</param>
        /// <param name="useLoggerFactory">Optional, whether to enable logging.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, Func<IServiceProvider, IAdapterMapper> adapterMapperFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, bool useLoggerFactory = true)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TAdapterInterface), fact =>
            {
                IAdapterMapper adapterMapper = adapterMapperFactory(fact);
                TTarget target = targetFactory(fact);
                ILoggerFactory? loggerFactory = useLoggerFactory ? fact.GetService<ILoggerFactory>() : null;
                var adapter = target.GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13>(adapterMapper, loggerFactory);
                return adapter;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }

        /// <summary>
        /// Add adapter to the service collection.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination2">Destination type 2.</typeparam>
        /// <typeparam name="TSource3">Source type 3. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination3">Destination type 3.</typeparam>
        /// <typeparam name="TSource4">Source type 4. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination4">Destination type 4.</typeparam>
        /// <typeparam name="TSource5">Source type 5. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination5">Destination type 5.</typeparam>
        /// <typeparam name="TSource6">Source type 6. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination6">Destination type 6.</typeparam>
        /// <typeparam name="TSource7">Source type 7. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination7">Destination type 7.</typeparam>
        /// <typeparam name="TSource8">Source type 8. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination8">Destination type 8.</typeparam>
        /// <typeparam name="TSource9">Source type 9. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination9">Destination type 9.</typeparam>
        /// <typeparam name="TSource10">Source type 10. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination10">Destination type 10.</typeparam>
        /// <typeparam name="TSource11">Source type 11. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination11">Destination type 11.</typeparam>
        /// <typeparam name="TSource12">Source type 12. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination12">Destination type 12.</typeparam>
        /// <typeparam name="TSource13">Source type 13. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination13">Destination type 13.</typeparam>
        /// <typeparam name="TSource14">Source type 14. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination14">Destination type 14.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the adapter target.</param>
        /// <param name="adapterMapperFactory">The factory which generates an instance of <see cref="IAdapterMapper"/> for the adapter.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the adapter in an <see cref="IServiceCollection"/>.</param>
        /// <param name="useLoggerFactory">Optional, whether to enable logging.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13, TSource14, TDestination14>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, Func<IServiceProvider, IAdapterMapper> adapterMapperFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, bool useLoggerFactory = true)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TAdapterInterface), fact =>
            {
                IAdapterMapper adapterMapper = adapterMapperFactory(fact);
                TTarget target = targetFactory(fact);
                ILoggerFactory? loggerFactory = useLoggerFactory ? fact.GetService<ILoggerFactory>() : null;
                var adapter = target.GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13, TSource14, TDestination14>(adapterMapper, loggerFactory);
                return adapter;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }

        /// <summary>
        /// Add adapter to the service collection.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination2">Destination type 2.</typeparam>
        /// <typeparam name="TSource3">Source type 3. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination3">Destination type 3.</typeparam>
        /// <typeparam name="TSource4">Source type 4. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination4">Destination type 4.</typeparam>
        /// <typeparam name="TSource5">Source type 5. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination5">Destination type 5.</typeparam>
        /// <typeparam name="TSource6">Source type 6. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination6">Destination type 6.</typeparam>
        /// <typeparam name="TSource7">Source type 7. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination7">Destination type 7.</typeparam>
        /// <typeparam name="TSource8">Source type 8. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination8">Destination type 8.</typeparam>
        /// <typeparam name="TSource9">Source type 9. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination9">Destination type 9.</typeparam>
        /// <typeparam name="TSource10">Source type 10. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination10">Destination type 10.</typeparam>
        /// <typeparam name="TSource11">Source type 11. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination11">Destination type 11.</typeparam>
        /// <typeparam name="TSource12">Source type 12. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination12">Destination type 12.</typeparam>
        /// <typeparam name="TSource13">Source type 13. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination13">Destination type 13.</typeparam>
        /// <typeparam name="TSource14">Source type 14. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination14">Destination type 14.</typeparam>
        /// <typeparam name="TSource15">Source type 15. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination15">Destination type 15.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the adapter target.</param>
        /// <param name="adapterMapperFactory">The factory which generates an instance of <see cref="IAdapterMapper"/> for the adapter.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the adapter in an <see cref="IServiceCollection"/>.</param>
        /// <param name="useLoggerFactory">Optional, whether to enable logging.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13, TSource14, TDestination14, TSource15, TDestination15>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, Func<IServiceProvider, IAdapterMapper> adapterMapperFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, bool useLoggerFactory = true)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TAdapterInterface), fact =>
            {
                IAdapterMapper adapterMapper = adapterMapperFactory(fact);
                TTarget target = targetFactory(fact);
                ILoggerFactory? loggerFactory = useLoggerFactory ? fact.GetService<ILoggerFactory>() : null;
                var adapter = target.GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13, TSource14, TDestination14, TSource15, TDestination15>(adapterMapper, loggerFactory);
                return adapter;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }

        /// <summary>
        /// Add adapter to the service collection.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination2">Destination type 2.</typeparam>
        /// <typeparam name="TSource3">Source type 3. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination3">Destination type 3.</typeparam>
        /// <typeparam name="TSource4">Source type 4. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination4">Destination type 4.</typeparam>
        /// <typeparam name="TSource5">Source type 5. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination5">Destination type 5.</typeparam>
        /// <typeparam name="TSource6">Source type 6. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination6">Destination type 6.</typeparam>
        /// <typeparam name="TSource7">Source type 7. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination7">Destination type 7.</typeparam>
        /// <typeparam name="TSource8">Source type 8. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination8">Destination type 8.</typeparam>
        /// <typeparam name="TSource9">Source type 9. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination9">Destination type 9.</typeparam>
        /// <typeparam name="TSource10">Source type 10. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination10">Destination type 10.</typeparam>
        /// <typeparam name="TSource11">Source type 11. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination11">Destination type 11.</typeparam>
        /// <typeparam name="TSource12">Source type 12. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination12">Destination type 12.</typeparam>
        /// <typeparam name="TSource13">Source type 13. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination13">Destination type 13.</typeparam>
        /// <typeparam name="TSource14">Source type 14. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination14">Destination type 14.</typeparam>
        /// <typeparam name="TSource15">Source type 15. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination15">Destination type 15.</typeparam>
        /// <typeparam name="TSource16">Source type 16. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination16">Destination type 16.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the adapter target.</param>
        /// <param name="adapterMapperFactory">The factory which generates an instance of <see cref="IAdapterMapper"/> for the adapter.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the adapter in an <see cref="IServiceCollection"/>.</param>
        /// <param name="useLoggerFactory">Optional, whether to enable logging.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13, TSource14, TDestination14, TSource15, TDestination15, TSource16, TDestination16>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, Func<IServiceProvider, IAdapterMapper> adapterMapperFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped, bool useLoggerFactory = true)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TAdapterInterface), fact =>
            {
                IAdapterMapper adapterMapper = adapterMapperFactory(fact);
                TTarget target = targetFactory(fact);
                ILoggerFactory? loggerFactory = useLoggerFactory ? fact.GetService<ILoggerFactory>() : null;
                var adapter = target.GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13, TSource14, TDestination14, TSource15, TDestination15, TSource16, TDestination16>(adapterMapper, loggerFactory);
                return adapter;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }
    }
}
