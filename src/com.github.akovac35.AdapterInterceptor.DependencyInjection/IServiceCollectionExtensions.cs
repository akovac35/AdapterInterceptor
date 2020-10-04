// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;

namespace com.github.akovac35.AdapterInterceptor.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add proxy imitator to the service collection.
        /// </summary>
        /// <typeparam name="TProxyInterface">The proxy imitator interface type.</typeparam>
        /// <typeparam name="TTarget">Type of the target being proxied.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the proxy imitator target.</param>
        /// <param name="proxyImitatorInterceptorFactory">The factory which generates a proxy imitator interceptor.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the proxy imitator in an <see cref="IServiceCollection"/>.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddProxyImitator<TProxyInterface, TTarget>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, Func<IServiceProvider, TTarget, ProxyImitatorInterceptor<TTarget>> proxyImitatorInterceptorFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TProxyInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TProxyInterface), fact =>
            {
                var target = targetFactory(fact);
                var result = target.GenerateProxyImitator<TProxyInterface, TTarget>(tmpTarget => proxyImitatorInterceptorFactory(fact, tmpTarget));
                return result;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }

        /// <summary>
        /// Add adapter to the service collection.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="targetFactory">The factory which generates the adapter target.</param>
        /// <param name="adapterInterceptorFactory">The factory which generates an adapter interceptor.</param>
        /// <param name="serviceLifetime">Optional, specifies the lifetime of the adapter in an <see cref="IServiceCollection"/>.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAdapter<TAdapterInterface, TTarget>(this IServiceCollection services, Func<IServiceProvider, TTarget> targetFactory, Func<IServiceProvider, TTarget, AdapterInterceptor<TTarget>> adapterInterceptorFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var descriptor = new ServiceDescriptor(typeof(TAdapterInterface), fact =>
            {
                var target = targetFactory(fact);
                var result = target.GenerateAdapter<TAdapterInterface, TTarget>(tmpTarget => adapterInterceptorFactory(fact, tmpTarget));
                return result;
            }, serviceLifetime);

            services.TryAdd(descriptor);

            return services;
        }

    }
}
