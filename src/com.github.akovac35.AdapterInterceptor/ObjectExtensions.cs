// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using Castle.DynamicProxy;
using com.github.akovac35.AdapterInterceptor.Misc;
using Microsoft.Extensions.Logging;

namespace com.github.akovac35.AdapterInterceptor
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Generates an adapter which imitates a proxy - no type mapping is performed and inputs are passed by reference for
        /// reference types and as value for value types. When compared to a real proxy, the drawback is a slower invocation 
        /// performance because of using reflection, but the benefit is that virtual methods are not required on the target.
        /// </summary>
        /// <typeparam name="TProxyInterface">The proxy imitator interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <param name="target">The adaptee.</param>
        /// <param name="adapterMapper">Optional adapter mapper.</param>
        /// <param name="loggerFactory">Optional logger factory. Logging will be enabled if one is provided.</param>
        /// <returns>The proxy imitator.</returns>
        public static TProxyInterface GenerateProxyImitator<TProxyInterface, TTarget>(this TTarget target, IAdapterMapper? adapterMapper = null, ILoggerFactory? loggerFactory = null)
            where TProxyInterface : class
            where TTarget : notnull
        {
            var adapterInterceptorTmp = new AdapterInterceptor<TTarget, PlaceholderType, PlaceholderType>(target, adapterMapper ?? new NoOperationAdapterMapper(), loggerFactory);
            var proxyGenerator = new ProxyGenerator();
            var adapter = proxyGenerator.CreateInterfaceProxyWithoutTarget<TProxyInterface>(adapterInterceptorTmp);
            return adapter;
        }

        /// <summary>
        /// Generates an adapter using the provided adapter mapper.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <param name="target">The adaptee.</param>
        /// <param name="adapterMapper">The adapter mapper.</param>
        /// <param name="loggerFactory">Optional logger factory. Logging will be enabled if one is provided.</param>
        /// <returns>The adapter.</returns>
        public static TAdapterInterface GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1>(this TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var adapterInterceptorTmp = new AdapterInterceptor<TTarget, TSource1, TDestination1>(target, adapterMapper, loggerFactory);
            var proxyGenerator = new ProxyGenerator();
            var adapter = proxyGenerator.CreateInterfaceProxyWithoutTarget<TAdapterInterface>(adapterInterceptorTmp);
            return adapter;
        }


        /// <summary>
        /// Generates an adapter using the provided adapter mapper.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination2">Destination type 2.</typeparam>
        /// <param name="target">The adaptee.</param>
        /// <param name="adapterMapper">The adapter mapper.</param>
        /// <param name="loggerFactory">Optional logger factory. Logging will be enabled if one is provided.</param>
        /// <returns>The adapter.</returns>
        public static TAdapterInterface GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2>(this TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var adapterInterceptorTmp = new AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2>(target, adapterMapper, loggerFactory);
            var proxyGenerator = new ProxyGenerator();
            var adapter = proxyGenerator.CreateInterfaceProxyWithoutTarget<TAdapterInterface>(adapterInterceptorTmp);
            return adapter;
        }

        /// <summary>
        /// Generates an adapter using the provided adapter mapper.
        /// </summary>
        /// <typeparam name="TAdapterInterface">The adapter interface type.</typeparam>
        /// <typeparam name="TTarget">The type of adaptee.</typeparam>
        /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination1">Destination type 1.</typeparam>
        /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination2">Destination type 2.</typeparam>
        /// <typeparam name="TSource3">Source type 3. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
        /// <typeparam name="TDestination3">Destination type 3.</typeparam>
        /// <param name="target">The adaptee.</param>
        /// <param name="adapterMapper">The adapter mapper.</param>
        /// <param name="loggerFactory">Optional logger factory. Logging will be enabled if one is provided.</param>
        /// <returns>The adapter.</returns>
        public static TAdapterInterface GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3>(this TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var adapterInterceptorTmp = new AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3>(target, adapterMapper, loggerFactory);
            var proxyGenerator = new ProxyGenerator();
            var adapter = proxyGenerator.CreateInterfaceProxyWithoutTarget<TAdapterInterface>(adapterInterceptorTmp);
            return adapter;
        }

        /// <summary>
        /// Generates an adapter using the provided adapter mapper.
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
        /// <param name="target">The adaptee.</param>
        /// <param name="adapterMapper">The adapter mapper.</param>
        /// <param name="loggerFactory">Optional logger factory. Logging will be enabled if one is provided.</param>
        /// <returns>The adapter.</returns>
        public static TAdapterInterface GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4>(this TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var adapterInterceptorTmp = new AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4>(target, adapterMapper, loggerFactory);
            var proxyGenerator = new ProxyGenerator();
            var adapter = proxyGenerator.CreateInterfaceProxyWithoutTarget<TAdapterInterface>(adapterInterceptorTmp);
            return adapter;
        }

        /// <summary>
        /// Generates an adapter using the provided adapter mapper.
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
        /// <param name="target">The adaptee.</param>
        /// <param name="adapterMapper">The adapter mapper.</param>
        /// <param name="loggerFactory">Optional logger factory. Logging will be enabled if one is provided.</param>
        /// <returns>The adapter.</returns>
        public static TAdapterInterface GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5>(this TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var adapterInterceptorTmp = new AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5>(target, adapterMapper, loggerFactory);
            var proxyGenerator = new ProxyGenerator();
            var adapter = proxyGenerator.CreateInterfaceProxyWithoutTarget<TAdapterInterface>(adapterInterceptorTmp);
            return adapter;
        }

        /// <summary>
        /// Generates an adapter using the provided adapter mapper.
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
        /// <param name="target">The adaptee.</param>
        /// <param name="adapterMapper">The adapter mapper.</param>
        /// <param name="loggerFactory">Optional logger factory. Logging will be enabled if one is provided.</param>
        /// <returns>The adapter.</returns>
        public static TAdapterInterface GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6>(this TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var adapterInterceptorTmp = new AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6>(target, adapterMapper, loggerFactory);
            var proxyGenerator = new ProxyGenerator();
            var adapter = proxyGenerator.CreateInterfaceProxyWithoutTarget<TAdapterInterface>(adapterInterceptorTmp);
            return adapter;
        }

        /// <summary>
        /// Generates an adapter using the provided adapter mapper.
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
        /// <param name="target">The adaptee.</param>
        /// <param name="adapterMapper">The adapter mapper.</param>
        /// <param name="loggerFactory">Optional logger factory. Logging will be enabled if one is provided.</param>
        /// <returns>The adapter.</returns>
        public static TAdapterInterface GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7>(this TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var adapterInterceptorTmp = new AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7>(target, adapterMapper, loggerFactory);
            var proxyGenerator = new ProxyGenerator();
            var adapter = proxyGenerator.CreateInterfaceProxyWithoutTarget<TAdapterInterface>(adapterInterceptorTmp);
            return adapter;
        }

        /// <summary>
        /// Generates an adapter using the provided adapter mapper.
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
        /// <param name="target">The adaptee.</param>
        /// <param name="adapterMapper">The adapter mapper.</param>
        /// <param name="loggerFactory">Optional logger factory. Logging will be enabled if one is provided.</param>
        /// <returns>The adapter.</returns>
        public static TAdapterInterface GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8>(this TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var adapterInterceptorTmp = new AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8>(target, adapterMapper, loggerFactory);
            var proxyGenerator = new ProxyGenerator();
            var adapter = proxyGenerator.CreateInterfaceProxyWithoutTarget<TAdapterInterface>(adapterInterceptorTmp);
            return adapter;
        }

        /// <summary>
        /// Generates an adapter using the provided adapter mapper.
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
        /// <param name="target">The adaptee.</param>
        /// <param name="adapterMapper">The adapter mapper.</param>
        /// <param name="loggerFactory">Optional logger factory. Logging will be enabled if one is provided.</param>
        /// <returns>The adapter.</returns>
        public static TAdapterInterface GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9>(this TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var adapterInterceptorTmp = new AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9>(target, adapterMapper, loggerFactory);
            var proxyGenerator = new ProxyGenerator();
            var adapter = proxyGenerator.CreateInterfaceProxyWithoutTarget<TAdapterInterface>(adapterInterceptorTmp);
            return adapter;
        }

        /// <summary>
        /// Generates an adapter using the provided adapter mapper.
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
        /// <param name="target">The adaptee.</param>
        /// <param name="adapterMapper">The adapter mapper.</param>
        /// <param name="loggerFactory">Optional logger factory. Logging will be enabled if one is provided.</param>
        /// <returns>The adapter.</returns>
        public static TAdapterInterface GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10>(this TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var adapterInterceptorTmp = new AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10>(target, adapterMapper, loggerFactory);
            var proxyGenerator = new ProxyGenerator();
            var adapter = proxyGenerator.CreateInterfaceProxyWithoutTarget<TAdapterInterface>(adapterInterceptorTmp);
            return adapter;
        }

        /// <summary>
        /// Generates an adapter using the provided adapter mapper.
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
        /// <param name="target">The adaptee.</param>
        /// <param name="adapterMapper">The adapter mapper.</param>
        /// <param name="loggerFactory">Optional logger factory. Logging will be enabled if one is provided.</param>
        /// <returns>The adapter.</returns>
        public static TAdapterInterface GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11>(this TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var adapterInterceptorTmp = new AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11>(target, adapterMapper, loggerFactory);
            var proxyGenerator = new ProxyGenerator();
            var adapter = proxyGenerator.CreateInterfaceProxyWithoutTarget<TAdapterInterface>(adapterInterceptorTmp);
            return adapter;
        }

        /// <summary>
        /// Generates an adapter using the provided adapter mapper.
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
        /// <param name="target">The adaptee.</param>
        /// <param name="adapterMapper">The adapter mapper.</param>
        /// <param name="loggerFactory">Optional logger factory. Logging will be enabled if one is provided.</param>
        /// <returns>The adapter.</returns>
        public static TAdapterInterface GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12>(this TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var adapterInterceptorTmp = new AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12>(target, adapterMapper, loggerFactory);
            var proxyGenerator = new ProxyGenerator();
            var adapter = proxyGenerator.CreateInterfaceProxyWithoutTarget<TAdapterInterface>(adapterInterceptorTmp);
            return adapter;
        }

        /// <summary>
        /// Generates an adapter using the provided adapter mapper.
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
        /// <param name="target">The adaptee.</param>
        /// <param name="adapterMapper">The adapter mapper.</param>
        /// <param name="loggerFactory">Optional logger factory. Logging will be enabled if one is provided.</param>
        /// <returns>The adapter.</returns>
        public static TAdapterInterface GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13>(this TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var adapterInterceptorTmp = new AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13>(target, adapterMapper, loggerFactory);
            var proxyGenerator = new ProxyGenerator();
            var adapter = proxyGenerator.CreateInterfaceProxyWithoutTarget<TAdapterInterface>(adapterInterceptorTmp);
            return adapter;
        }

        /// <summary>
        /// Generates an adapter using the provided adapter mapper.
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
        /// <param name="target">The adaptee.</param>
        /// <param name="adapterMapper">The adapter mapper.</param>
        /// <param name="loggerFactory">Optional logger factory. Logging will be enabled if one is provided.</param>
        /// <returns>The adapter.</returns>
        public static TAdapterInterface GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13, TSource14, TDestination14>(this TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var adapterInterceptorTmp = new AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13, TSource14, TDestination14>(target, adapterMapper, loggerFactory);
            var proxyGenerator = new ProxyGenerator();
            var adapter = proxyGenerator.CreateInterfaceProxyWithoutTarget<TAdapterInterface>(adapterInterceptorTmp);
            return adapter;
        }

        /// <summary>
        /// Generates an adapter using the provided adapter mapper.
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
        /// <param name="target">The adaptee.</param>
        /// <param name="adapterMapper">The adapter mapper.</param>
        /// <param name="loggerFactory">Optional logger factory. Logging will be enabled if one is provided.</param>
        /// <returns>The adapter.</returns>
        public static TAdapterInterface GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13, TSource14, TDestination14, TSource15, TDestination15>(this TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var adapterInterceptorTmp = new AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13, TSource14, TDestination14, TSource15, TDestination15>(target, adapterMapper, loggerFactory);
            var proxyGenerator = new ProxyGenerator();
            var adapter = proxyGenerator.CreateInterfaceProxyWithoutTarget<TAdapterInterface>(adapterInterceptorTmp);
            return adapter;
        }

        /// <summary>
        /// Generates an adapter using the provided adapter mapper.
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
        /// <param name="target">The adaptee.</param>
        /// <param name="adapterMapper">The adapter mapper.</param>
        /// <param name="loggerFactory">Optional logger factory. Logging will be enabled if one is provided.</param>
        /// <returns>The adapter.</returns>
        public static TAdapterInterface GenerateAdapter<TAdapterInterface, TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13, TSource14, TDestination14, TSource15, TDestination15, TSource16, TDestination16>(this TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            where TAdapterInterface : class
            where TTarget : notnull
        {
            var adapterInterceptorTmp = new AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13, TSource14, TDestination14, TSource15, TDestination15, TSource16, TDestination16>(target, adapterMapper, loggerFactory);
            var proxyGenerator = new ProxyGenerator();
            var adapter = proxyGenerator.CreateInterfaceProxyWithoutTarget<TAdapterInterface>(adapterInterceptorTmp);
            return adapter;
        }
    }
}
