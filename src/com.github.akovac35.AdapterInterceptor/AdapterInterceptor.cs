// Author: Aleksander Kovač

using Castle.DynamicProxy;
using com.github.akovac35.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace com.github.akovac35.AdapterInterceptor
{
    public class AdapterInterceptor<TTarget> : IInterceptor
    {
        #region Constructors
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, IReadOnlyDictionary<Type, Type> supportedTypePairs, ConcurrentDictionary<MethodInfo, MethodInfo> adapterToTargetMethodDictionary, ILoggerFactory? loggerFactory = null):
            this(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = supportedTypePairs;
            AdapterToTargetMethodDictionary = adapterToTargetMethodDictionary;
        }

        protected AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            :this(loggerFactory)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            TargetType = typeof(TTarget);
            AdapterMapper = adapterMapper ?? throw new ArgumentNullException(nameof(adapterMapper));
        }

        protected AdapterInterceptor(ILoggerFactory? loggerFactory = null)
        {
            _logger = (loggerFactory ?? LoggerFactoryProvider.LoggerFactory).CreateLogger<AdapterInterceptor<TTarget>>();

            Target = null!;
            TargetType = null!;
            AdapterMapper = null!;
            SupportedTypePairs = null!;
            AdapterToTargetMethodDictionary = null!;

            _invokeTargetGenericTaskAsync_Method = Find_InvokeTargetGenericTaskAsync_Method();
            _invokeTargetGenericValueTaskAsync_Method = Find_InvokeTargetGenericValueTaskAsync_Method();
        }
        #endregion

        #region Fields and properties

        protected IAdapterMapper AdapterMapper { get; set; }

        protected object Target { get; set; }

        protected Type TargetType { get; set; }

        protected IReadOnlyDictionary<Type, Type> SupportedTypePairs { get; set; }

        protected ConcurrentDictionary<MethodInfo, MethodInfo> AdapterToTargetMethodDictionary { get; set; }

        private readonly ILogger _logger;

        protected MethodInfo _invokeTargetGenericTaskAsync_Method;

        protected MethodInfo _invokeTargetGenericValueTaskAsync_Method;

        #endregion

        /// <summary>
        /// Invokes target method. Does not call Proceed() because there is nothing to proceed to - this interceptor should always be the last one.
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

        #region Invocation

        protected virtual object? InvokeTarget(MethodInfo adapterMethod, MethodInfo targetMethod, object?[] targetArguments, IInvocation invocation, Type[] adapterMethodTypes, Type[] targetMethodTypes)
        {
            object? result;
            Type targetMethodReturnType = targetMethod.ReturnType;
            
            // Task<>
            if(AdapterHelper.IsGenericTask(targetMethodReturnType))
            {
                if (AdapterHelper.IsTask(adapterMethod.ReturnType) != AdapterHelper.IsTask(targetMethod.ReturnType)) throw new AdapterInterceptorException($"Adapter and target method return types should match if either is generic Task. Adapter method: {adapterMethod.ToLoggerString()}, Target method: {targetMethod.ToLoggerString()}");

                MethodInfo genericInvokeTargetGenericTaskAsyncMethod = AdapterToTargetMethodDictionary.GetOrAdd(adapterMethod, item => _invokeTargetGenericTaskAsync_Method.MakeGenericMethod(item.ReturnType.GenericTypeArguments[0]));
                result = genericInvokeTargetGenericTaskAsyncMethod.Invoke(this, new object[] { adapterMethod, targetMethod, targetArguments });
            }
            // Task
            else if(AdapterHelper.IsTask(targetMethodReturnType))
            {
                if (AdapterHelper.IsTask(adapterMethod.ReturnType) != AdapterHelper.IsTask(targetMethod.ReturnType)) throw new AdapterInterceptorException($"Adapter and target method return types should match if either is Task. Adapter method: {adapterMethod.ToLoggerString()}, Target method: {targetMethod.ToLoggerString()}");

                result = InvokeTargetTaskAsync(adapterMethod, targetMethod, targetArguments);
            }
            // ValueTask<>
            else if(AdapterHelper.IsGenericValueTask(targetMethodReturnType))
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
            if(_logger.IsEnteringExitingEnabled()) _logger.Here(l => l.LogTrace("Target method result: {@0}, adapter method result: {@1}", returnValue, mappedReturnValue));
            
            for (int i = 0; i < adapterMethodTypes.Length; i++)
            {
                if(adapterMethodTypes[i].IsByRef)
                {
                    invocation.Arguments[i] = AdapterMapper.Map(targetArguments[i], targetMethodTypes[i], adapterMethodTypes[i]);
                    if(_logger.IsEnteringExitingEnabled()) _logger.Here(l => l.LogTrace($"Target method argument[{i}]: {{@0}}, adapter method argument[{i}]: {{@1}}", targetArguments[i], invocation.Arguments[i]));
                }
            }
            
            return mappedReturnValue;
        }

        protected virtual async Task<TAdapter> InvokeTargetGenericTaskAsync<TAdapter>(MethodInfo adapterMethod, MethodInfo targetMethod, object?[] targetArguments)
        {
            // Tasks are invariant and casting them is limited. Since await is forced to Task<TDestination> by compiler,
            // dynamic cast must be used to box the invocation result
            dynamic? task = targetMethod.Invoke(Target, targetArguments) as dynamic;
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
            if (_logger.IsEnteringExitingEnabled()) _logger.Here(l => l.LogTrace("Target method result: {@0}, adapter method result: {@1}", targetMethodReturnValue, mappedReturnValue));

            return mappedReturnValue!;
        }

        protected virtual async Task InvokeTargetTaskAsync(MethodInfo adapterMethod, MethodInfo targetMethod, object?[] targetArguments)
        {
            Task? task = targetMethod.Invoke(Target, targetArguments) as Task;
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
            if (_logger.IsEnteringExitingEnabled()) _logger.Here(l => l.LogTrace("Target method result: {@0}, adapter method result: {@1}", targetMethodReturnValue, mappedReturnValue));

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

        #endregion

        #region Mapping
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

        #endregion
    }

    #region Generic variants

    public class AdapterInterceptor<TTarget, TSource1, TDestination1> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, bool supportCollectionTypeVariants = true, bool supportReverseMapping = true, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            var supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
            supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: supportCollectionTypeVariants, addReverseVariants: supportReverseMapping);
            SupportedTypePairs = supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }
        
        // Do note each generic type variant has its own copy
        private static ConcurrentDictionary<MethodInfo, MethodInfo> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
    }

    public class AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, bool supportCollectionTypeVariants = true, bool supportReverseMapping = true, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            var supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
            supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: supportCollectionTypeVariants, addReverseVariants: supportReverseMapping);
            supportedTypePairs.AddTypePair(typeof(TSource2), typeof(TDestination2), addCollectionVariants: supportCollectionTypeVariants, addReverseVariants: supportReverseMapping);
            SupportedTypePairs = supportedTypePairs;
            AdapterToTargetMethodDictionary = _adapterToTargetMethodDictionary;
        }

        // Do note each generic type variant has its own copy
        private static ConcurrentDictionary<MethodInfo, MethodInfo> _adapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
    }

    public class AdapterInterceptor<TTarget, TSource1, TDestination1, TSource2, TDestination2, TSource3, TDestination3> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, bool supportCollectionTypeVariants = true, bool supportReverseMapping = true, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            var supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
            supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: supportCollectionTypeVariants, addReverseVariants: supportReverseMapping);
            supportedTypePairs.AddTypePair(typeof(TSource2), typeof(TDestination2), addCollectionVariants: supportCollectionTypeVariants, addReverseVariants: supportReverseMapping);
            supportedTypePairs.AddTypePair(typeof(TSource3), typeof(TDestination3), addCollectionVariants: supportCollectionTypeVariants, addReverseVariants: supportReverseMapping);
            SupportedTypePairs = supportedTypePairs;
            AdapterToTargetMethodDictionary = _adapterToTargetMethodDictionary;
        }

        // Do note each generic type variant has its own copy
        private static ConcurrentDictionary<MethodInfo, MethodInfo> _adapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, MethodInfo>();
    }

    #endregion
}
