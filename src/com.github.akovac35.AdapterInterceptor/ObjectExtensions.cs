// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using Castle.DynamicProxy;
using System;

namespace com.github.akovac35.AdapterInterceptor
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Generates a proxy imitator.
        /// </summary>
        /// <typeparam name="TProxyInterface">The proxy imitator interface type.</typeparam>
        /// <typeparam name="TTarget">Type of the target being proxied.</typeparam>
        /// <param name="target">The object being proxied.</param>
        /// <param name="proxyImitatorInterceptorFactory">The factory which generates a proxy imitator interceptor.</param>
        /// <returns>The proxy imitator.</returns>
        public static TProxyInterface GenerateProxyImitator<TProxyInterface, TTarget>(this TTarget target, Func<TTarget, ProxyImitatorInterceptor<TTarget>> proxyImitatorInterceptorFactory)
            where TProxyInterface : class
            where TTarget : notnull
        {
            var interceptor = proxyImitatorInterceptorFactory(target);
            var proxyGenerator = new ProxyGenerator();
            var result = proxyGenerator.CreateInterfaceProxyWithoutTarget<TProxyInterface>(interceptor);
            return result;
        }

        /// <summary>
        /// Generates an adapter.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <param name="target">The adaptee.</param>
        /// <param name="adapterInterceptorFactory">The factory which generates an adapter interceptor.</param>
        /// <returns>The adapter.</returns>
        public static TAdapterInterface GenerateAdapter<TAdapterInterface, TTarget>(this TTarget target, Func<TTarget, AdapterInterceptor<TTarget>> adapterInterceptorFactory)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var interceptor = adapterInterceptorFactory(target);
            var proxyGenerator = new ProxyGenerator();
            var result = proxyGenerator.CreateInterfaceProxyWithoutTarget<TAdapterInterface>(interceptor);
            return result;
        }

    }
}
