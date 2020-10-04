// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using com.github.akovac35.AdapterInterceptor.Helper;
using com.github.akovac35.AdapterInterceptor.Misc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace com.github.akovac35.AdapterInterceptor
{
    /// <summary>
    /// Generates an adapter interceptor variant which imitates a proxy - no type mapping is performed and inputs are passed by reference for
    /// reference types and as value for value types. When compared to a real proxy, the drawback is a slower invocation 
    /// performance because of using reflection, but the benefit is that virtual methods are not required on the target.
    /// </summary>
    /// <typeparam name="TTarget">Type of the target being proxied.</typeparam>
    public class ProxyImitatorInterceptor<TTarget> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        /// <summary>
        /// Initializes a new ProxyImitatorInterceptor instance.
        /// </summary>
        /// <param name="target">The object being proxied.</param>
        /// <param name="loggerFactory">Logger factory instace.</param>
        public ProxyImitatorInterceptor(TTarget target, ILoggerFactory? loggerFactory = null) : base(target, new NoOperationAdapterMapper(), loggerFactory)
        {
            SupportedTypePairs = _supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }

        protected static ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>();

        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }
}
