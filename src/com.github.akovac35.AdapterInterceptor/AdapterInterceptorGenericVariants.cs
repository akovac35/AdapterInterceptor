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
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic variant AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
    /// Implicitly supported collection variants are: T[], IList<T>, List<T>, IEnumerable<T>, ICollection<T>
    /// Type mapping does not implicitly account for variance/covariance or inheritance relationships, only explicit type pairs will be mapped.
    /// </summary>
    /// <typeparam name="TTarget">Type of target - adaptee</typeparam>
    /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
    /// <typeparam name="TDestination1">Destination type 1.</typeparam>

    public class AdapterInterceptor<TTarget, TSource1, TDestination1> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        static AdapterInterceptor()
        {
            _supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: true, addReverseVariants: true);

        }

        /// <summary>
        /// Initializes a new AdapterInterceptor instance.
        /// </summary>
        /// <param name="target">Adaptee instance.</param>
        /// <param name="adapterMapper">Adapter mapper instance.</param>
        /// <param name="loggerFactory">Logger factory instace.</param>
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = _supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }

        // Do note each generic type variant has its own copy
        protected static ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic variant AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
    /// Implicitly supported collection variants are: T[], IList<T>, List<T>, IEnumerable<T>, ICollection<T>
    /// Type mapping does not implicitly account for variance/covariance or inheritance relationships, only explicit type pairs will be mapped.
    /// </summary>
    /// <typeparam name="TTarget">Type of target - adaptee</typeparam>
    /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
    /// <typeparam name="TDestination1">Destination type 1.</typeparam>
    /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
    /// <typeparam name="TDestination2">Destination type 2.</typeparam>
    public class AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        static AdapterInterceptor()
        {
            _supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource2), typeof(TDestination2), addCollectionVariants: true, addReverseVariants: true);
        }

        /// <summary>
        /// Initializes a new AdapterInterceptor instance.
        /// </summary>
        /// <param name="target">Adaptee instance.</param>
        /// <param name="adapterMapper">Adapter mapper instance.</param>
        /// <param name="loggerFactory">Logger factory instace.</param>
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = _supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }

        // Do note each generic type variant has its own copy
        protected static ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic variant AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
    /// Implicitly supported collection variants are: T[], IList<T>, List<T>, IEnumerable<T>, ICollection<T>
    /// Type mapping does not implicitly account for variance/covariance or inheritance relationships, only explicit type pairs will be mapped.
    /// </summary>
    /// <typeparam name="TTarget">Type of target - adaptee</typeparam>
    /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
    /// <typeparam name="TDestination1">Destination type 1.</typeparam>
    /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
    /// <typeparam name="TDestination2">Destination type 2.</typeparam>
    /// <typeparam name="TSource3">Source type 3. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
    /// <typeparam name="TDestination3">Destination type 3.</typeparam>
    public class AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        static AdapterInterceptor()
        {
            _supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource2), typeof(TDestination2), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource3), typeof(TDestination3), addCollectionVariants: true, addReverseVariants: true);
        }

        /// <summary>
        /// Initializes a new AdapterInterceptor instance.
        /// </summary>
        /// <param name="target">Adaptee instance.</param>
        /// <param name="adapterMapper">Adapter mapper instance.</param>
        /// <param name="loggerFactory">Logger factory instace.</param>
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = _supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }

        // Do note each generic type variant has its own copy
        protected static ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic variant AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
    /// Implicitly supported collection variants are: T[], IList<T>, List<T>, IEnumerable<T>, ICollection<T>
    /// Type mapping does not implicitly account for variance/covariance or inheritance relationships, only explicit type pairs will be mapped.
    /// </summary>
    /// <typeparam name="TTarget">Type of target - adaptee</typeparam>
    /// <typeparam name="TSource1">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
    /// <typeparam name="TDestination1">Destination type 1.</typeparam>
    /// <typeparam name="TSource2">Source type 2. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
    /// <typeparam name="TDestination2">Destination type 2.</typeparam>
    /// <typeparam name="TSource3">Source type 3. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
    /// <typeparam name="TDestination3">Destination type 3.</typeparam>
    /// <typeparam name="TSource4">Source type 4. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
    /// <typeparam name="TDestination4">Destination type 4.</typeparam>
    public class AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        static AdapterInterceptor()
        {
            _supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource2), typeof(TDestination2), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource3), typeof(TDestination3), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource4), typeof(TDestination4), addCollectionVariants: true, addReverseVariants: true);
        }

        /// <summary>
        /// Initializes a new AdapterInterceptor instance.
        /// </summary>
        /// <param name="target">Adaptee instance.</param>
        /// <param name="adapterMapper">Adapter mapper instance.</param>
        /// <param name="loggerFactory">Logger factory instace.</param>
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = _supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }

        // Do note each generic type variant has its own copy
        protected static ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic variant AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
    /// Implicitly supported collection variants are: T[], IList<T>, List<T>, IEnumerable<T>, ICollection<T>
    /// Type mapping does not implicitly account for variance/covariance or inheritance relationships, only explicit type pairs will be mapped.
    /// </summary>
    /// <typeparam name="TTarget">Type of target - adaptee</typeparam>
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
    public class AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        static AdapterInterceptor()
        {
            _supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource2), typeof(TDestination2), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource3), typeof(TDestination3), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource4), typeof(TDestination4), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource5), typeof(TDestination5), addCollectionVariants: true, addReverseVariants: true);
        }

        /// <summary>
        /// Initializes a new AdapterInterceptor instance.
        /// </summary>
        /// <param name="target">Adaptee instance.</param>
        /// <param name="adapterMapper">Adapter mapper instance.</param>
        /// <param name="loggerFactory">Logger factory instace.</param>
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = _supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }

        // Do note each generic type variant has its own copy
        protected static ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic variant AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
    /// Implicitly supported collection variants are: T[], IList<T>, List<T>, IEnumerable<T>, ICollection<T>
    /// Type mapping does not implicitly account for variance/covariance or inheritance relationships, only explicit type pairs will be mapped.
    /// </summary>
    /// <typeparam name="TTarget">Type of target - adaptee</typeparam>
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
    public class AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        static AdapterInterceptor()
        {
            _supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource2), typeof(TDestination2), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource3), typeof(TDestination3), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource4), typeof(TDestination4), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource5), typeof(TDestination5), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource6), typeof(TDestination6), addCollectionVariants: true, addReverseVariants: true);
        }

        /// <summary>
        /// Initializes a new AdapterInterceptor instance.
        /// </summary>
        /// <param name="target">Adaptee instance.</param>
        /// <param name="adapterMapper">Adapter mapper instance.</param>
        /// <param name="loggerFactory">Logger factory instace.</param>
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = _supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }

        // Do note each generic type variant has its own copy
        protected static ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic variant AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
    /// Implicitly supported collection variants are: T[], IList<T>, List<T>, IEnumerable<T>, ICollection<T>
    /// Type mapping does not implicitly account for variance/covariance or inheritance relationships, only explicit type pairs will be mapped.
    /// </summary>
    /// <typeparam name="TTarget">Type of target - adaptee</typeparam>
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
    public class AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        static AdapterInterceptor()
        {
            _supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource2), typeof(TDestination2), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource3), typeof(TDestination3), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource4), typeof(TDestination4), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource5), typeof(TDestination5), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource6), typeof(TDestination6), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource7), typeof(TDestination7), addCollectionVariants: true, addReverseVariants: true);
        }

        /// <summary>
        /// Initializes a new AdapterInterceptor instance.
        /// </summary>
        /// <param name="target">Adaptee instance.</param>
        /// <param name="adapterMapper">Adapter mapper instance.</param>
        /// <param name="loggerFactory">Logger factory instace.</param>
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = _supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }

        // Do note each generic type variant has its own copy
        protected static ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic variant AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
    /// Implicitly supported collection variants are: T[], IList<T>, List<T>, IEnumerable<T>, ICollection<T>
    /// Type mapping does not implicitly account for variance/covariance or inheritance relationships, only explicit type pairs will be mapped.
    /// </summary>
    /// <typeparam name="TTarget">Type of target - adaptee</typeparam>
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
    public class AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        static AdapterInterceptor()
        {
            _supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource2), typeof(TDestination2), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource3), typeof(TDestination3), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource4), typeof(TDestination4), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource5), typeof(TDestination5), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource6), typeof(TDestination6), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource7), typeof(TDestination7), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource8), typeof(TDestination8), addCollectionVariants: true, addReverseVariants: true);
        }

        /// <summary>
        /// Initializes a new AdapterInterceptor instance.
        /// </summary>
        /// <param name="target">Adaptee instance.</param>
        /// <param name="adapterMapper">Adapter mapper instance.</param>
        /// <param name="loggerFactory">Logger factory instace.</param>
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = _supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }

        // Do note each generic type variant has its own copy
        protected static ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic variant AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
    /// Implicitly supported collection variants are: T[], IList<T>, List<T>, IEnumerable<T>, ICollection<T>
    /// Type mapping does not implicitly account for variance/covariance or inheritance relationships, only explicit type pairs will be mapped.
    /// </summary>
    /// <typeparam name="TTarget">Type of target - adaptee</typeparam>
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
    public class AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        static AdapterInterceptor()
        {
            _supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource2), typeof(TDestination2), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource3), typeof(TDestination3), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource4), typeof(TDestination4), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource5), typeof(TDestination5), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource6), typeof(TDestination6), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource7), typeof(TDestination7), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource8), typeof(TDestination8), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource9), typeof(TDestination9), addCollectionVariants: true, addReverseVariants: true);
        }

        /// <summary>
        /// Initializes a new AdapterInterceptor instance.
        /// </summary>
        /// <param name="target">Adaptee instance.</param>
        /// <param name="adapterMapper">Adapter mapper instance.</param>
        /// <param name="loggerFactory">Logger factory instace.</param>
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = _supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }

        // Do note each generic type variant has its own copy
        protected static ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic variant AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
    /// Implicitly supported collection variants are: T[], IList<T>, List<T>, IEnumerable<T>, ICollection<T>
    /// Type mapping does not implicitly account for variance/covariance or inheritance relationships, only explicit type pairs will be mapped.
    /// </summary>
    /// <typeparam name="TTarget">Type of target - adaptee</typeparam>
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
    public class AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        static AdapterInterceptor()
        {
            _supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource2), typeof(TDestination2), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource3), typeof(TDestination3), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource4), typeof(TDestination4), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource5), typeof(TDestination5), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource6), typeof(TDestination6), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource7), typeof(TDestination7), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource8), typeof(TDestination8), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource9), typeof(TDestination9), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource10), typeof(TDestination10), addCollectionVariants: true, addReverseVariants: true);
        }

        /// <summary>
        /// Initializes a new AdapterInterceptor instance.
        /// </summary>
        /// <param name="target">Adaptee instance.</param>
        /// <param name="adapterMapper">Adapter mapper instance.</param>
        /// <param name="loggerFactory">Logger factory instace.</param>
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = _supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }

        // Do note each generic type variant has its own copy
        protected static ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic variant AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
    /// Implicitly supported collection variants are: T[], IList<T>, List<T>, IEnumerable<T>, ICollection<T>
    /// Type mapping does not implicitly account for variance/covariance or inheritance relationships, only explicit type pairs will be mapped.
    /// </summary>
    /// <typeparam name="TTarget">Type of target - adaptee</typeparam>
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
    public class AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        static AdapterInterceptor()
        {
            _supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource2), typeof(TDestination2), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource3), typeof(TDestination3), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource4), typeof(TDestination4), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource5), typeof(TDestination5), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource6), typeof(TDestination6), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource7), typeof(TDestination7), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource8), typeof(TDestination8), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource9), typeof(TDestination9), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource10), typeof(TDestination10), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource11), typeof(TDestination11), addCollectionVariants: true, addReverseVariants: true);
        }

        /// <summary>
        /// Initializes a new AdapterInterceptor instance.
        /// </summary>
        /// <param name="target">Adaptee instance.</param>
        /// <param name="adapterMapper">Adapter mapper instance.</param>
        /// <param name="loggerFactory">Logger factory instace.</param>
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = _supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }

        // Do note each generic type variant has its own copy
        protected static ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic variant AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
    /// Implicitly supported collection variants are: T[], IList<T>, List<T>, IEnumerable<T>, ICollection<T>
    /// Type mapping does not implicitly account for variance/covariance or inheritance relationships, only explicit type pairs will be mapped.
    /// </summary>
    /// <typeparam name="TTarget">Type of target - adaptee</typeparam>
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
    public class AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        static AdapterInterceptor()
        {
            _supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource2), typeof(TDestination2), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource3), typeof(TDestination3), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource4), typeof(TDestination4), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource5), typeof(TDestination5), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource6), typeof(TDestination6), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource7), typeof(TDestination7), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource8), typeof(TDestination8), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource9), typeof(TDestination9), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource10), typeof(TDestination10), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource11), typeof(TDestination11), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource12), typeof(TDestination12), addCollectionVariants: true, addReverseVariants: true);
        }

        /// <summary>
        /// Initializes a new AdapterInterceptor instance.
        /// </summary>
        /// <param name="target">Adaptee instance.</param>
        /// <param name="adapterMapper">Adapter mapper instance.</param>
        /// <param name="loggerFactory">Logger factory instace.</param>
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = _supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }

        // Do note each generic type variant has its own copy
        protected static ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic variant AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
    /// Implicitly supported collection variants are: T[], IList<T>, List<T>, IEnumerable<T>, ICollection<T>
    /// Type mapping does not implicitly account for variance/covariance or inheritance relationships, only explicit type pairs will be mapped.
    /// </summary>
    /// <typeparam name="TTarget">Type of target - adaptee</typeparam>
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
    public class AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        static AdapterInterceptor()
        {
            _supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource2), typeof(TDestination2), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource3), typeof(TDestination3), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource4), typeof(TDestination4), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource5), typeof(TDestination5), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource6), typeof(TDestination6), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource7), typeof(TDestination7), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource8), typeof(TDestination8), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource9), typeof(TDestination9), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource10), typeof(TDestination10), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource11), typeof(TDestination11), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource12), typeof(TDestination12), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource13), typeof(TDestination13), addCollectionVariants: true, addReverseVariants: true);
        }

        /// <summary>
        /// Initializes a new AdapterInterceptor instance.
        /// </summary>
        /// <param name="target">Adaptee instance.</param>
        /// <param name="adapterMapper">Adapter mapper instance.</param>
        /// <param name="loggerFactory">Logger factory instace.</param>
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = _supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }

        // Do note each generic type variant has its own copy
        protected static ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic variant AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
    /// Implicitly supported collection variants are: T[], IList<T>, List<T>, IEnumerable<T>, ICollection<T>
    /// Type mapping does not implicitly account for variance/covariance or inheritance relationships, only explicit type pairs will be mapped.
    /// </summary>
    /// <typeparam name="TTarget">Type of target - adaptee</typeparam>
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
    public class AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13, TSource14, TDestination14> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        static AdapterInterceptor()
        {
            _supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource2), typeof(TDestination2), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource3), typeof(TDestination3), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource4), typeof(TDestination4), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource5), typeof(TDestination5), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource6), typeof(TDestination6), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource7), typeof(TDestination7), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource8), typeof(TDestination8), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource9), typeof(TDestination9), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource10), typeof(TDestination10), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource11), typeof(TDestination11), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource12), typeof(TDestination12), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource13), typeof(TDestination13), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource14), typeof(TDestination14), addCollectionVariants: true, addReverseVariants: true);
        }

        /// <summary>
        /// Initializes a new AdapterInterceptor instance.
        /// </summary>
        /// <param name="target">Adaptee instance.</param>
        /// <param name="adapterMapper">Adapter mapper instance.</param>
        /// <param name="loggerFactory">Logger factory instace.</param>
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = _supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }

        // Do note each generic type variant has its own copy
        protected static ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic variant AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
    /// Implicitly supported collection variants are: T[], IList<T>, List<T>, IEnumerable<T>, ICollection<T>
    /// Type mapping does not implicitly account for variance/covariance or inheritance relationships, only explicit type pairs will be mapped.
    /// </summary>
    /// <typeparam name="TTarget">Type of target - adaptee</typeparam>
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
    public class AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13, TSource14, TDestination14, TSource15, TDestination15> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        static AdapterInterceptor()
        {
            _supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource2), typeof(TDestination2), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource3), typeof(TDestination3), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource4), typeof(TDestination4), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource5), typeof(TDestination5), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource6), typeof(TDestination6), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource7), typeof(TDestination7), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource8), typeof(TDestination8), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource9), typeof(TDestination9), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource10), typeof(TDestination10), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource11), typeof(TDestination11), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource12), typeof(TDestination12), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource13), typeof(TDestination13), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource14), typeof(TDestination14), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource15), typeof(TDestination15), addCollectionVariants: true, addReverseVariants: true);
        }

        /// <summary>
        /// Initializes a new AdapterInterceptor instance.
        /// </summary>
        /// <param name="target">Adaptee instance.</param>
        /// <param name="adapterMapper">Adapter mapper instance.</param>
        /// <param name="loggerFactory">Logger factory instace.</param>
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = _supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }

        // Do note each generic type variant has its own copy
        protected static ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic variant AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
    /// Implicitly supported collection variants are: T[], IList<T>, List<T>, IEnumerable<T>, ICollection<T>
    /// Type mapping does not implicitly account for variance/covariance or inheritance relationships, only explicit type pairs will be mapped.
    /// </summary>
    /// <typeparam name="TTarget">Type of target - adaptee</typeparam>
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
    public class AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3, TSource4, TDestination4, TSource5, TDestination5, TSource6, TDestination6, TSource7, TDestination7, TSource8, TDestination8, TSource9, TDestination9, TSource10, TDestination10, TSource11, TDestination11, TSource12, TDestination12, TSource13, TDestination13, TSource14, TDestination14, TSource15, TDestination15, TSource16, TDestination16> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        static AdapterInterceptor()
        {
            _supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource2), typeof(TDestination2), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource3), typeof(TDestination3), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource4), typeof(TDestination4), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource5), typeof(TDestination5), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource6), typeof(TDestination6), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource7), typeof(TDestination7), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource8), typeof(TDestination8), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource9), typeof(TDestination9), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource10), typeof(TDestination10), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource11), typeof(TDestination11), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource12), typeof(TDestination12), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource13), typeof(TDestination13), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource14), typeof(TDestination14), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource15), typeof(TDestination15), addCollectionVariants: true, addReverseVariants: true);
            _supportedTypePairs.AddTypePair(typeof(TSource16), typeof(TDestination16), addCollectionVariants: true, addReverseVariants: true);
        }

        /// <summary>
        /// Initializes a new AdapterInterceptor instance.
        /// </summary>
        /// <param name="target">Adaptee instance.</param>
        /// <param name="adapterMapper">Adapter mapper instance.</param>
        /// <param name="loggerFactory">Logger factory instace.</param>
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = _supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }

        // Do note each generic type variant has its own copy
        protected static ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }
}
