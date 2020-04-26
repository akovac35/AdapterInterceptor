// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using Castle.DynamicProxy;
using com.github.akovac35.AdapterInterceptor.Helper;
using com.github.akovac35.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace com.github.akovac35.AdapterInterceptor
{
    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// </summary>
    /// <typeparam name="TTarget">Type of target - adaptee</typeparam>
    public class AdapterInterceptor<TTarget> : IInterceptor
    {
        /// <summary>
        /// Initializes a new AdapterInterceptor instance.
        /// </summary>
        /// <param name="target">Adaptee instance.</param>
        /// <param name="adapterMapper">Adapter mapper instance.</param>
        /// <param name="supportedTypePairs">Read-only collection of source/target type pairs which will be mapped. Must also contain reverse mappings for method invocation result mapping.</param>
        /// <param name="adapterToTargetMethodDictionary">Thread-safe collection of adapter/target method mappings. Used for caching. Will be populated at runtime if empty.</param>
        /// <param name="loggerFactory">Logger factory instace.</param>
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, IReadOnlyDictionary<Type, Type> supportedTypePairs, ConcurrentDictionary<MethodInfo, MethodInfo> adapterToTargetMethodDictionary, ILoggerFactory? loggerFactory = null) :
            this(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = supportedTypePairs;
            AdapterToTargetMethodDictionary = adapterToTargetMethodDictionary;
        }

        protected AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            : this(loggerFactory)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            TargetType = typeof(TTarget);
            AdapterMapper = adapterMapper ?? throw new ArgumentNullException(nameof(adapterMapper));
        }

        protected AdapterInterceptor(ILoggerFactory? loggerFactory = null)
        {
            _logger = loggerFactory?.CreateLogger<AdapterInterceptor<TTarget>>() ?? new NullLogger<AdapterInterceptor<TTarget>>();

            Target = null!;
            TargetType = null!;
            AdapterMapper = null!;
            SupportedTypePairs = null!;
            AdapterToTargetMethodDictionary = null!;

            _invokeTargetGenericTaskAsync_Method = Find_InvokeTargetGenericTaskAsync_Method();
            _invokeTargetGenericValueTaskAsync_Method = Find_InvokeTargetGenericValueTaskAsync_Method();
        }

        protected IAdapterMapper AdapterMapper { get; set; }

        protected object Target { get; set; }

        protected Type TargetType { get; set; }

        protected IReadOnlyDictionary<Type, Type> SupportedTypePairs { get; set; }

        protected ConcurrentDictionary<MethodInfo, MethodInfo> AdapterToTargetMethodDictionary { get; set; }

        private readonly ILogger _logger;

        protected MethodInfo _invokeTargetGenericTaskAsync_Method;

        protected MethodInfo _invokeTargetGenericValueTaskAsync_Method;

        /// <summary>
        /// Applies type and method mappings and invokes target method with mapped argument values. Does not call Proceed() because there is nothing to proceed to - this interceptor should always be the last one.
        /// </summary>
        /// <param name="invocation">Encapsulates an invocation of a proxied method.</param>
        public virtual void Intercept(IInvocation invocation)
        {
            if (_logger.IsEnteringExitingEnabled()) _logger.Here(l => l.Entering(invocation.ToLoggerString(simpleType: true), invocation.Arguments, invocation.ReturnValue));

            MethodInfo adapterMethod = invocation.Method;
            Type[] adapterMethodTypes = adapterMethod.GetParameters().Select(item => item.ParameterType).ToArray();

            Type[] targetMethodTypes = MapSupportedTypes(adapterMethodTypes);
            MethodInfo targetMethod = MapTargetMethod(TargetType, adapterMethod, targetMethodTypes);

            object?[] adapterArguments = invocation.Arguments;
            object?[] targetArguments = new object?[adapterArguments.Length];

            for (int i = 0; i < adapterMethodTypes.Length; i++)
            {
                targetArguments[i] = AdapterMapper.Map(adapterArguments[i], adapterMethodTypes[i], targetMethodTypes[i]);
            }

            object? mappedReturnValue = InvokeTarget(adapterMethod, targetMethod, targetArguments, invocation, adapterMethodTypes, targetMethodTypes);
            invocation.ReturnValue = mappedReturnValue;

            // Return values are logged elsewhere
            _logger.Here(l => l.Exiting());
        }

        protected virtual object? InvokeTarget(MethodInfo adapterMethod, MethodInfo targetMethod, object?[] targetArguments, IInvocation invocation, Type[] adapterMethodTypes, Type[] targetMethodTypes)
        {
            object? result;
            Type targetMethodReturnType = targetMethod.ReturnType;

            // Task<>
            if (AdapterHelper.IsGenericTask(targetMethodReturnType))
            {
                if (AdapterHelper.IsTask(adapterMethod.ReturnType) != AdapterHelper.IsTask(targetMethod.ReturnType)) throw new AdapterInterceptorException($"Adapter and target method return types should match if either is generic Task. Adapter method: {adapterMethod.ToLoggerString()}, Target method: {targetMethod.ToLoggerString()}");

                MethodInfo genericInvokeTargetGenericTaskAsyncMethod = AdapterToTargetMethodDictionary.GetOrAdd(adapterMethod, item => _invokeTargetGenericTaskAsync_Method.MakeGenericMethod(item.ReturnType.GenericTypeArguments[0]));
                result = genericInvokeTargetGenericTaskAsyncMethod.Invoke(this, new object[] { adapterMethod, targetMethod, targetArguments });
            }
            // Task
            else if (AdapterHelper.IsTask(targetMethodReturnType))
            {
                if (AdapterHelper.IsTask(adapterMethod.ReturnType) != AdapterHelper.IsTask(targetMethod.ReturnType)) throw new AdapterInterceptorException($"Adapter and target method return types should match if either is Task. Adapter method: {adapterMethod.ToLoggerString()}, Target method: {targetMethod.ToLoggerString()}");

                result = InvokeTargetTaskAsync(adapterMethod, targetMethod, targetArguments);
            }
            // ValueTask<>
            else if (AdapterHelper.IsGenericValueTask(targetMethodReturnType))
            {
                if (AdapterHelper.IsGenericValueTask(adapterMethod.ReturnType) != AdapterHelper.IsGenericValueTask(targetMethod.ReturnType)) throw new AdapterInterceptorException($"Adapter and target method return types should match if either is generic ValueTask. Adapter method: {adapterMethod.ToLoggerString()}, Target method: {targetMethod.ToLoggerString()}");

                MethodInfo genericInvokeTargetGenericValueTaskAsyncMethod = AdapterToTargetMethodDictionary.GetOrAdd(adapterMethod, item => _invokeTargetGenericValueTaskAsync_Method.MakeGenericMethod(item.ReturnType.GenericTypeArguments[0]));
                result = genericInvokeTargetGenericValueTaskAsyncMethod.Invoke(this, new object[] { adapterMethod, targetMethod, targetArguments });
            }
            // ValueTask
            else if (AdapterHelper.IsValueTask(targetMethodReturnType))
            {
                if (AdapterHelper.IsValueTask(adapterMethod.ReturnType) != AdapterHelper.IsValueTask(targetMethod.ReturnType)) throw new AdapterInterceptorException($"Adapter and target method return types should match if either is ValueTask. Adapter method: {adapterMethod.ToLoggerString()}, Target method: {targetMethod.ToLoggerString()}");

                result = InvokeTargetValueTaskAsync(adapterMethod, targetMethod, targetArguments);
            }
            else
            // Likely synchronous
            {
                if (AdapterHelper.IsVoid(adapterMethod.ReturnType) && adapterMethod.ReturnType != targetMethod.ReturnType) throw new AdapterInterceptorException($"Adapter and target method return types should match if either is void. Adapter method: {adapterMethod.ToLoggerString()}, Target method: {targetMethod.ToLoggerString()}");

                result = InvokeTargetSync(adapterMethod, targetMethod, targetArguments, invocation, adapterMethodTypes, targetMethodTypes);
            }
            //TODO: add async enumerable

            return result;
        }

        protected virtual object? InvokeTargetSync(MethodInfo adapterMethod, MethodInfo targetMethod, object?[] targetArguments, IInvocation invocation, Type[] adapterMethodTypes, Type[] targetMethodTypes)
        {
            object? returnValue = targetMethod.Invoke(Target, targetArguments);
            object? mappedReturnValue = AdapterMapper.Map(returnValue, targetMethod.ReturnType, adapterMethod.ReturnType);
            if (_logger.IsEnabled(LogLevel.Trace)) _logger.Here(l => l.LogTrace("Target method result: {@0}, adapter method result: {@1}", returnValue, mappedReturnValue));

            for (int i = 0; i < adapterMethodTypes.Length; i++)
            {
                if (adapterMethodTypes[i].IsByRef)
                {
                    invocation.Arguments[i] = AdapterMapper.Map(targetArguments[i], targetMethodTypes[i], adapterMethodTypes[i]);
                    if (_logger.IsEnabled(LogLevel.Trace)) _logger.Here(l => l.LogTrace($"Target method argument[{i}]: {{@0}}, adapter method argument[{i}]: {{@1}}", targetArguments[i], invocation.Arguments[i]));
                }
            }

            return mappedReturnValue;
        }

        protected virtual async Task<TAdapter> InvokeTargetGenericTaskAsync<TAdapter>(MethodInfo adapterMethod, MethodInfo targetMethod, object?[] targetArguments)
        {
            // Tasks are invariant and casting them is limited. Since await is forced to Task<TDestination> by compiler,
            // dynamic cast must be used to box the invocation result
            dynamic task = (dynamic)targetMethod.Invoke(Target, targetArguments);
            if (task == null)
            {
                _logger.Here(l => l.LogTrace("Target method arguments: {@0}", targetArguments));
                throw new AdapterInterceptorException($"Target method result should be a task but is null. Adapter method: {adapterMethod.ToLoggerString()}, target method: {targetMethod.ToLoggerString()}");
            }
            object? targetMethodReturnValue = await task.ConfigureAwait(false);
            // Get type of T from Task<T>
            Type adapterMethodReturnTypeWithoutTask = adapterMethod.ReturnType.GenericTypeArguments[0];
            Type targetMethodReturnTypeWithoutTask = targetMethod.ReturnType.GenericTypeArguments[0];
            TAdapter mappedReturnValue = (TAdapter)AdapterMapper.Map(targetMethodReturnValue, targetMethodReturnTypeWithoutTask, adapterMethodReturnTypeWithoutTask);
            if (_logger.IsEnabled(LogLevel.Trace)) _logger.Here(l => l.LogTrace("Target method result: {@0}, adapter method result: {@1}", targetMethodReturnValue, mappedReturnValue));

            return mappedReturnValue!;
        }

        protected virtual async Task InvokeTargetTaskAsync(MethodInfo adapterMethod, MethodInfo targetMethod, object?[] targetArguments)
        {
            Task task = (Task)targetMethod.Invoke(Target, targetArguments);
            if (task == null)
            {
                _logger.Here(l => l.LogTrace("Target method arguments: {@0}", targetArguments));
                throw new AdapterInterceptorException($"Target method result should be a task but is null. Adapter method: {adapterMethod.ToLoggerString()}, target method: {targetMethod.ToLoggerString()}");
            }
            await task.ConfigureAwait(false);
        }

        protected virtual async ValueTask<TAdapter> InvokeTargetGenericValueTaskAsync<TAdapter>(MethodInfo adapterMethod, MethodInfo targetMethod, object?[] targetArguments)
        {
            // ValueTasks are invariant and casting them is limited. Since await is forced to ValueTask<TDestination> by compiler,
            // dynamic cast must be used to box the invocation result
            dynamic task = (dynamic)targetMethod.Invoke(Target, targetArguments);
            object? targetMethodReturnValue = await task.ConfigureAwait(false);
            // Get type of T from ValueTask<T>
            Type adapterMethodReturnTypeWithoutTask = adapterMethod.ReturnType.GenericTypeArguments[0];
            Type targetMethodReturnTypeWithoutTask = targetMethod.ReturnType.GenericTypeArguments[0];
            TAdapter mappedReturnValue = (TAdapter)AdapterMapper.Map(targetMethodReturnValue, targetMethodReturnTypeWithoutTask, adapterMethodReturnTypeWithoutTask);
            if (_logger.IsEnabled(LogLevel.Trace)) _logger.Here(l => l.LogTrace("Target method result: {@0}, adapter method result: {@1}", targetMethodReturnValue, mappedReturnValue));

            return mappedReturnValue!;
        }

        protected virtual async ValueTask InvokeTargetValueTaskAsync(MethodInfo adapterMethod, MethodInfo targetMethod, object?[] targetArguments)
        {
            ValueTask task = (ValueTask)targetMethod.Invoke(Target, targetArguments);
            await task.ConfigureAwait(false);
        }

        protected virtual MethodInfo Find_InvokeTargetGenericTaskAsync_Method()
        {
            // Will apply to the actual most derived type: https://stackoverflow.com/questions/5780584/will-gettype-return-the-most-derived-type-when-called-from-the-base-class
            return this.GetType().GetMethod(nameof(InvokeTargetGenericTaskAsync), BindingFlags.NonPublic | BindingFlags.Instance);
        }

        protected virtual MethodInfo Find_InvokeTargetGenericValueTaskAsync_Method()
        {
            // Will apply to the actual most derived type: https://stackoverflow.com/questions/5780584/will-gettype-return-the-most-derived-type-when-called-from-the-base-class
            return this.GetType().GetMethod(nameof(InvokeTargetGenericValueTaskAsync), BindingFlags.NonPublic | BindingFlags.Instance);
        }

        protected virtual Type[] MapSupportedTypes(Type[] sourceTypes)
        {
            Type[] destinationTypes = new Type[sourceTypes.Length];
            for (int i = 0; i < destinationTypes.Length; i++)
            {
                bool isMapped = SupportedTypePairs.TryGetValue(sourceTypes[i].IsByRef ? sourceTypes[i].GetElementType() : sourceTypes[i], out destinationTypes[i]);
                if (isMapped && sourceTypes[i].IsByRef) destinationTypes[i] = destinationTypes[i].MakeByRefType();

                if (!isMapped) destinationTypes[i] = sourceTypes[i];
            }

            return destinationTypes;
        }

        protected virtual MethodInfo MapTargetMethod(Type targetType, MethodInfo sourceMethod, Type[] destinationTypes)
        {
            MethodInfo targetMethodInfo = targetType.GetMethod(sourceMethod.Name, destinationTypes);

            if (targetMethodInfo == null) throw new AdapterInterceptorException($"Target method cannot be found. This is likely due to missing supported type pairs or because target type changed. Target type: {targetType.ToLoggerString()} Target method: {sourceMethod.ToLoggerString()} Target method parameter types: {destinationTypes.ToLoggerString()}");

            return targetMethodInfo;
        }
    }


    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic constructor AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
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
        protected static ConcurrentDictionary<MethodInfo, MethodInfo> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic constructor AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
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
        protected static ConcurrentDictionary<MethodInfo, MethodInfo> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic constructor AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
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
        protected static ConcurrentDictionary<MethodInfo, MethodInfo> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic constructor AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
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
        protected static ConcurrentDictionary<MethodInfo, MethodInfo> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic constructor AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
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
        protected static ConcurrentDictionary<MethodInfo, MethodInfo> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic constructor AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
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
        protected static ConcurrentDictionary<MethodInfo, MethodInfo> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic constructor AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
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
        protected static ConcurrentDictionary<MethodInfo, MethodInfo> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic constructor AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
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
        protected static ConcurrentDictionary<MethodInfo, MethodInfo> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic constructor AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
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
        protected static ConcurrentDictionary<MethodInfo, MethodInfo> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic constructor AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
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
        protected static ConcurrentDictionary<MethodInfo, MethodInfo> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic constructor AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
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
        protected static ConcurrentDictionary<MethodInfo, MethodInfo> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic constructor AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
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
        protected static ConcurrentDictionary<MethodInfo, MethodInfo> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic constructor AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
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
        protected static ConcurrentDictionary<MethodInfo, MethodInfo> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic constructor AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
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
        protected static ConcurrentDictionary<MethodInfo, MethodInfo> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic constructor AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
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
        protected static ConcurrentDictionary<MethodInfo, MethodInfo> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }

    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic constructor AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
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
        protected static ConcurrentDictionary<MethodInfo, MethodInfo> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }
}
