// Author: Aleksander Kovač

using Castle.DynamicProxy;
using com.github.akovac35.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace com.github.akovac35.AdapterInterceptor
{
    public class AdapterInterceptor : IInterceptor
    {
        #region Constructors
        public AdapterInterceptor(object target, Type targetType, IAdapterMapper adapterMapper, IReadOnlyDictionary<Type, Type> supportedTypePairs, ILoggerFactory? loggerFactory = null):
            this(target, targetType, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = supportedTypePairs;
        }

        protected AdapterInterceptor(object target, Type targetType, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
            :this(loggerFactory)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
            AdapterMapper = adapterMapper ?? throw new ArgumentNullException(nameof(adapterMapper));
        }

        protected AdapterInterceptor(ILoggerFactory? loggerFactory = null)
        {
            _logger = (loggerFactory ?? LoggerFactoryProvider.LoggerFactory).CreateLogger<AdapterInterceptor>();

            Target = null!;
            TargetType = null!;
            AdapterMapper = null!;
            SupportedTypePairs = null!;

            _invokeTargetGenericAsync_Method = AssignTargetGenericAsyncMethod();
        }
        #endregion

        #region Fields and properties

        public IAdapterMapper AdapterMapper { get; protected set; }

        public object Target { get; protected set; }

        public Type TargetType { get; protected set; }

        public IReadOnlyDictionary<Type, Type> SupportedTypePairs { get; protected set; }

        private readonly ILogger _logger;

        protected MethodInfo _invokeTargetGenericAsync_Method;

        #endregion

        /// <summary>
        /// Adapter can't Proceed(). If it is needed, configure proxy target.
        /// </summary>
        /// <param name="invocation"></param>
        public virtual void Intercept(IInvocation? invocation)
        {
            if (invocation == null) throw new ArgumentNullException(nameof(invocation));

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

            object? mappedReturnValue = InvokeTarget(adapterMethod, targetMethod, targetArguments);
            invocation.ReturnValue = mappedReturnValue;

            _logger.Here(l => l.Exiting(mappedReturnValue));
        }

        #region Invocation

        // TODO: optimize for performance
        // TODO: support value task
        // TODO: test ref and out

        protected virtual object? InvokeTarget(MethodInfo adapterMethod, MethodInfo targetMethod, object?[] targetArguments)
        {
            _logger.Here(l => l.Entering());

            AssertIsValidMethodCombination(adapterMethod, targetMethod);

            object? result;
            Type targetMethodReturnType = targetMethod.ReturnType;
            // Likely synchronous
            if (targetMethodReturnType == typeof(void) || !typeof(Task).IsAssignableFrom(targetMethodReturnType))
            {
                result = InvokeTargetSync(adapterMethod, targetMethod, targetArguments);
            }
            // Task<>
            else if(targetMethodReturnType.GetTypeInfo().IsGenericType)
            {                
                Type adapterMethodReturnTypeWithoutTask = adapterMethod.ReturnType.GenericTypeArguments[0];
                MethodInfo genericInvokeTargetGenericAsyncMethod = _invokeTargetGenericAsync_Method.MakeGenericMethod(adapterMethodReturnTypeWithoutTask);
                result = genericInvokeTargetGenericAsyncMethod.Invoke(this, new object[] { adapterMethod, targetMethod, targetArguments });
            }
            // Task
            else
            {
                result = InvokeTargetAsync(adapterMethod, targetMethod, targetArguments);
            }

            _logger.Here(l => l.Exiting());
            return result;
        }

        protected virtual object? InvokeTargetSync(MethodInfo adapterMethod, MethodInfo targetMethod, object?[] targetArguments)
        {
            _logger.Here(l => l.Entering());
            
            object? returnValue = targetMethod.Invoke(Target, targetArguments);
            object? mappedReturnValue = AdapterMapper.Map(returnValue, targetMethod.ReturnType, adapterMethod.ReturnType);

            _logger.Here(l => l.Exiting());
            return mappedReturnValue;
        }

        // Never rename this method, it is invoked by name
        protected virtual async Task<TDestination> InvokeTargetGenericAsync<TDestination>(MethodInfo adapterMethod, MethodInfo targetMethod, object?[] targetArguments)
        {
            _logger.Here(l => l.Entering());

            // Tasks are invariant, to get Task<object> from generic Task<T> target method result we first cast Task<T> to Task
            // and then retrieve result from the competed task by casting it to a dynamic runtime object
            Task task = (Task)targetMethod.Invoke(Target, targetArguments);
            await task.ConfigureAwait(false);
            object? targetMethodReturnValue = ((dynamic)task).Result;
            // Get type of T from Task<T>
            Type adapterMethodReturnTypeWithoutTask = adapterMethod.ReturnType.GenericTypeArguments[0];
            Type targetMethodReturnTypeWithoutTask = targetMethod.ReturnType.GenericTypeArguments[0];

            TDestination mappedReturnValue = (TDestination)AdapterMapper.Map(targetMethodReturnValue, targetMethodReturnTypeWithoutTask, adapterMethodReturnTypeWithoutTask);

            _logger.Here(l => l.Exiting());
            return mappedReturnValue!;
        }

        protected virtual async Task InvokeTargetAsync(MethodInfo adapterMethod, MethodInfo targetMethod, object?[] targetArguments)
        {
            _logger.Here(l => l.Entering());

            Task task = (Task)targetMethod.Invoke(Target, targetArguments);
            await task.ConfigureAwait(false);
            
            _logger.Here(l => l.Exiting());
        }

        protected virtual void AssertIsValidMethodCombination(MethodInfo adapterMethod, MethodInfo targetMethod)
        {
            if (adapterMethod.ReturnType == typeof(void) && adapterMethod.ReturnType != targetMethod.ReturnType) throw new AdapterInterceptorException($"Adapter and target method return types should match if either is void. Adapter method: {adapterMethod.ToLoggerString()}, Target method: {targetMethod.ToLoggerString()}");

            if (typeof(Task).IsAssignableFrom(adapterMethod.ReturnType) != typeof(Task).IsAssignableFrom(targetMethod.ReturnType)) throw new AdapterInterceptorException($"Adapter and target method return types should match if either is Task. Adapter method: {adapterMethod.ToLoggerString()}, Target method: {targetMethod.ToLoggerString()}");
        }

        protected virtual MethodInfo AssignTargetGenericAsyncMethod()
        {
            // Will apply to the actual most derived type: https://stackoverflow.com/questions/5780584/will-gettype-return-the-most-derived-type-when-called-from-the-base-class
            return this.GetType().GetMethod("InvokeTargetGenericAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        #endregion

        #region Mapping
        protected virtual Type[] MapSupportedTypes(Type[] sourceTypes)
        {
            if (_logger.IsEnteringExitingEnabled()) _logger.Here(l => l.Entering(sourceTypes.ToLoggerString(simpleType: true)));

            Type[] destinationTypes = new Type[sourceTypes.Length];
            for (int i = 0; i < destinationTypes.Length; i++)
            {
                bool isMapped = SupportedTypePairs.TryGetValue(sourceTypes[i], out destinationTypes[i]);

                if (!isMapped) destinationTypes[i] = sourceTypes[i];
            }

            if (_logger.IsEnteringExitingEnabled()) _logger.Here(l => l.Exiting(destinationTypes.ToLoggerString(simpleType: true)));
            return destinationTypes;
        }

        protected virtual MethodInfo MapTargetMethod(Type targetType, MethodInfo sourceMethod, Type[] destinationTypes)
        {
            if (_logger.IsEnteringExitingEnabled()) _logger.Here(l => l.Entering(targetType.ToLoggerString(simpleType: true), sourceMethod.ToLoggerString(simpleType: true), destinationTypes.ToLoggerString(simpleType: true)));

            MethodInfo targetMethodInfo = targetType.GetMethod(sourceMethod.Name, destinationTypes);

            if (targetMethodInfo == null) throw new AdapterInterceptorException($"Target method cannot be found. This is likely due to missing supported type pairs or because target type changed. Target type: {targetType.ToLoggerString()} Target method: {sourceMethod.ToLoggerString()} Target method parameter types: {destinationTypes.ToLoggerString()}");

            if (_logger.IsEnteringExitingEnabled()) _logger.Here(l => l.Exiting(targetMethodInfo.ToLoggerString(simpleType: true)));
            return targetMethodInfo;
        }

        #endregion
    }

    #region Generic variants

    public class AdapterInterceptor<TTarget> : AdapterInterceptor
        where TTarget : notnull
    {
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, IReadOnlyDictionary<Type, Type> supportedTypePairs, ILoggerFactory? loggerFactory = null) : base(target, typeof(TTarget), adapterMapper, supportedTypePairs, loggerFactory) { }

        protected AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, typeof(TTarget), adapterMapper, loggerFactory) { }
    }

    public class AdapterInterceptor<TTarget, TSource1, TDestination1> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, bool supportCollectionTypeVariants = true, bool supportReverseMapping = true, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {
            var supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
            supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: supportCollectionTypeVariants, addReverseVariants: supportReverseMapping);
            SupportedTypePairs = supportedTypePairs;
        }
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
        }
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
        }
    }

    #endregion
}
