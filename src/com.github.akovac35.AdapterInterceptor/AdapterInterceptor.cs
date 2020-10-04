// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using Castle.DynamicProxy;
using com.github.akovac35.AdapterInterceptor.Helper;
using com.github.akovac35.AdapterInterceptor.Misc;
using com.github.akovac35.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
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
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, IReadOnlyDictionary<Type, Type> supportedTypePairs, ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> adapterToTargetMethodDictionary, ILoggerFactory? loggerFactory = null) :
            this(target, adapterMapper, loggerFactory)
        {
            SupportedTypePairs = supportedTypePairs ?? throw new ArgumentNullException(nameof(supportedTypePairs));
            AdapterToTargetMethodDictionary = adapterToTargetMethodDictionary ?? throw new ArgumentNullException(nameof(adapterToTargetMethodDictionary));
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
            _logger = loggerFactory?.CreateLogger<AdapterInterceptor<TTarget>>() ?? NullLogger<AdapterInterceptor<TTarget>>.Instance;

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

        protected ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> AdapterToTargetMethodDictionary { get; set; }

        private readonly ILogger _logger;

        protected MethodInfo _invokeTargetGenericTaskAsync_Method;

        protected MethodInfo _invokeTargetGenericValueTaskAsync_Method;

        /// <summary>
        /// Applies type and method mappings and invokes target method with mapped argument values. Does not call Proceed() because there is nothing to proceed to - this interceptor should always be the last one.
        /// </summary>
        /// <param name="invocation">Encapsulates an invocation of a proxied method.</param>
        public virtual void Intercept(IInvocation invocation)
        {
            if (_logger.IsEnteringExitingEnabled()) _logger.Here(l => l.Entering(invocation.ToLoggerString(simpleType: true)!, invocation.Arguments, invocation.ReturnValue));

            MethodInfo adapterMethod = invocation.Method;
            AdapterInvocationInformation adapterInvocationInformation = AdapterToTargetMethodDictionary.GetOrAdd(adapterMethod, item =>
            {
                Type[] adapterMethodTypes = adapterMethod.GetParameters().Select(item => item.ParameterType).ToArray();
                Type[] targetMethodTypes = MapSupportedTypes(adapterMethodTypes);
                MethodInfo targetMethod = MapTargetMethod(TargetType, adapterMethod, targetMethodTypes);
                AdapterInvocationInformation result = PrepareAdapterInvocationInformation(adapterMethod, targetMethod, adapterMethodTypes, targetMethodTypes);
                return result;
            });

            object?[] adapterArguments = invocation.Arguments;
            object?[] targetArguments = new object?[adapterArguments.Length];

            for (int i = 0; i < targetArguments.Length; i++)
            {
                targetArguments[i] = AdapterMapper.Map(adapterArguments[i], adapterInvocationInformation.AdapterMethodParameterTypes[i], adapterInvocationInformation.TargetMethodParameterTypes[i]);
            }

            object? mappedReturnValue;
            switch (adapterInvocationInformation.TargetInvocationType)
            {
                case InvocationTypes.Sync:
                    mappedReturnValue = InvokeTargetSync(adapterMethod, adapterInvocationInformation, targetArguments, invocation);
                    break;
                case InvocationTypes.GenericTask:
                    mappedReturnValue = adapterInvocationInformation.InvocationHelper!.Invoke(this, new object[] { adapterMethod, adapterInvocationInformation, targetArguments });
                    break;
                case InvocationTypes.Task:
                    mappedReturnValue = InvokeTargetTaskAsync(adapterMethod, adapterInvocationInformation, targetArguments);
                    break;
                case InvocationTypes.GenericValueTask:
                    mappedReturnValue = adapterInvocationInformation.InvocationHelper!.Invoke(this, new object[] { adapterMethod, adapterInvocationInformation, targetArguments });
                    break;
                case InvocationTypes.ValueTask:
                    mappedReturnValue = InvokeTargetValueTaskAsync(adapterMethod, adapterInvocationInformation, targetArguments);
                    break;
                default: throw new NotImplementedException();
            }
            invocation.ReturnValue = mappedReturnValue;

            // Return values are logged elsewhere
            _logger.Here(l => l.Exiting());
        }

        protected virtual object? InvokeTargetSync(MethodInfo adapterMethod, AdapterInvocationInformation adapterInvocationInformation, object?[] targetArguments, IInvocation invocation)
        {
            object? returnValue;
            try
            {
                returnValue = adapterInvocationInformation.TargetMethod.Invoke(Target, targetArguments);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                // Should not be reached
                throw ex;
            }

            object? mappedReturnValue = AdapterMapper.Map(returnValue, adapterInvocationInformation.TargetMethod.ReturnType, adapterMethod.ReturnType);
            if (_logger.IsEnabled(LogLevel.Trace)) _logger.Here(l => l.LogTrace("Target method result: {@0}, adapter method result: {@1}", returnValue, mappedReturnValue));

            for (int i = 0; i < adapterInvocationInformation.AdapterMethodParameterTypes.Length; i++)
            {
                if (adapterInvocationInformation.AdapterMethodParameterTypes[i].IsByRef)
                {
                    invocation.Arguments[i] = AdapterMapper.Map(targetArguments[i], adapterInvocationInformation.TargetMethodParameterTypes[i], adapterInvocationInformation.AdapterMethodParameterTypes[i]);
                    if (_logger.IsEnabled(LogLevel.Trace)) _logger.Here(l => l.LogTrace($"Updated adapter method argument. Target method argument[{i}]: {{@0}}, adapter method argument[{i}]: {{@1}}", targetArguments[i], invocation.Arguments[i]));
                }
            }

            return mappedReturnValue;
        }

        protected virtual async Task<TAdapter> InvokeTargetGenericTaskAsync<TAdapter>(MethodInfo adapterMethod, AdapterInvocationInformation adapterInvocationInformation, object?[] targetArguments)
        {
            dynamic task;
            // Tasks are invariant and casting them is limited. Since await is forced to Task<TDestination> by compiler,
            // dynamic cast must be used to box the invocation result
            try
            {
                task = (dynamic)adapterInvocationInformation.TargetMethod.Invoke(Target, targetArguments);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                // Should not be reached
                throw ex;
            }

            if (task == null)
            {
                _logger.Here(l => l.LogTrace("Target method arguments: {@0}", targetArguments));
                throw new AdapterInterceptorException($"Target method result should be a generic task but is null.{Environment.NewLine}Adapter method: {adapterMethod.ToLoggerString(simpleType: true)}{Environment.NewLine}Target method: {adapterInvocationInformation.TargetMethod.ToLoggerString(simpleType: true)}");
            }
            object? targetMethodReturnValue = await task.ConfigureAwait(false);
            // Get type of T from Task<T>
            Type adapterMethodReturnTypeWithoutTask = adapterMethod.ReturnType.GenericTypeArguments[0];
            Type targetMethodReturnTypeWithoutTask = adapterInvocationInformation.TargetMethod.ReturnType.GenericTypeArguments[0];
            TAdapter mappedReturnValue = (TAdapter)AdapterMapper.Map(targetMethodReturnValue, targetMethodReturnTypeWithoutTask, adapterMethodReturnTypeWithoutTask);
            if (_logger.IsEnabled(LogLevel.Trace)) _logger.Here(l => l.LogTrace("Target method result: {@0}, adapter method result: {@1}", targetMethodReturnValue, mappedReturnValue));

            return mappedReturnValue!;
        }

        protected virtual async Task InvokeTargetTaskAsync(MethodInfo adapterMethod, AdapterInvocationInformation adapterInvocationInformation, object?[] targetArguments)
        {
            Task task;
            try
            {
                task = (Task)adapterInvocationInformation.TargetMethod.Invoke(Target, targetArguments);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                // Should not be reached
                throw ex;
            }

            if (task == null)
            {
                _logger.Here(l => l.LogTrace("Target method arguments: {@0}", targetArguments));
                throw new AdapterInterceptorException($"Target method result should be a task but is null.{Environment.NewLine}Adapter method: {adapterMethod.ToLoggerString(simpleType: true)}{Environment.NewLine}Target method: {adapterInvocationInformation.TargetMethod.ToLoggerString(simpleType: true)}");
            }
            await task.ConfigureAwait(false);
        }

        protected virtual async ValueTask<TAdapter> InvokeTargetGenericValueTaskAsync<TAdapter>(MethodInfo adapterMethod, AdapterInvocationInformation adapterInvocationInformation, object?[] targetArguments)
        {
            dynamic task;
            // ValueTasks are invariant and casting them is limited. Since await is forced to ValueTask<TDestination> by compiler,
            // dynamic cast must be used to box the invocation result
            try
            {
                task = (dynamic)adapterInvocationInformation.TargetMethod.Invoke(Target, targetArguments);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                // Should not be reached
                throw ex;
            }

            object? targetMethodReturnValue = await task.ConfigureAwait(false);
            // Get type of T from ValueTask<T>
            Type adapterMethodReturnTypeWithoutTask = adapterMethod.ReturnType.GenericTypeArguments[0];
            Type targetMethodReturnTypeWithoutTask = adapterInvocationInformation.TargetMethod.ReturnType.GenericTypeArguments[0];
            TAdapter mappedReturnValue = (TAdapter)AdapterMapper.Map(targetMethodReturnValue, targetMethodReturnTypeWithoutTask, adapterMethodReturnTypeWithoutTask);
            if (_logger.IsEnabled(LogLevel.Trace)) _logger.Here(l => l.LogTrace("Target method result: {@0}, adapter method result: {@1}", targetMethodReturnValue, mappedReturnValue));

            return mappedReturnValue!;
        }

        protected virtual async ValueTask InvokeTargetValueTaskAsync(MethodInfo adapterMethod, AdapterInvocationInformation adapterInvocationInformation, object?[] targetArguments)
        {
            ValueTask task;
            try
            {
                task = (ValueTask)adapterInvocationInformation.TargetMethod.Invoke(Target, targetArguments);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                // Should not be reached
                throw ex;
            }

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

        protected virtual AdapterInvocationInformation PrepareAdapterInvocationInformation(MethodInfo adapterMethod, MethodInfo targetMethod, Type[] adapterMethodParameterTypes, Type[] targetMethodParmeterTypes)
        {
            Type targetMethodReturnType = targetMethod.ReturnType;
            // Task<>
            if (AdapterHelper.IsGenericTask(targetMethodReturnType))
            {
                MethodInfo genericInvokeTargetGenericTaskAsyncMethod = _invokeTargetGenericTaskAsync_Method.MakeGenericMethod(adapterMethod.ReturnType.GenericTypeArguments[0]);
                InvocationTypes targetInvocationType = InvocationTypes.GenericTask;
                AssertReturnTypeCombination(adapterMethod, targetMethod, targetInvocationType);
                return new AdapterInvocationInformation(targetMethod, adapterMethodParameterTypes, targetMethodParmeterTypes, targetInvocationType, genericInvokeTargetGenericTaskAsyncMethod);
            }
            // Task
            else if (AdapterHelper.IsTask(targetMethodReturnType))
            {
                InvocationTypes targetInvocationType = InvocationTypes.Task;
                AssertReturnTypeCombination(adapterMethod, targetMethod, targetInvocationType);
                return new AdapterInvocationInformation(targetMethod, adapterMethodParameterTypes, targetMethodParmeterTypes, targetInvocationType);
            }
            // ValueTask<>
            else if (AdapterHelper.IsGenericValueTask(targetMethodReturnType))
            {
                MethodInfo genericInvokeTargetGenericValueTaskAsyncMethod = _invokeTargetGenericValueTaskAsync_Method.MakeGenericMethod(adapterMethod.ReturnType.GenericTypeArguments[0]);
                InvocationTypes targetInvocationType = InvocationTypes.GenericValueTask;
                AssertReturnTypeCombination(adapterMethod, targetMethod, targetInvocationType);
                return new AdapterInvocationInformation(targetMethod, adapterMethodParameterTypes, targetMethodParmeterTypes, targetInvocationType, genericInvokeTargetGenericValueTaskAsyncMethod);
            }
            // ValueTask
            else if (AdapterHelper.IsValueTask(targetMethodReturnType))
            {
                InvocationTypes targetInvocationType = InvocationTypes.ValueTask;
                AssertReturnTypeCombination(adapterMethod, targetMethod, targetInvocationType);
                return new AdapterInvocationInformation(targetMethod, adapterMethodParameterTypes, targetMethodParmeterTypes, targetInvocationType);
            }
            else
            // Likely synchronous
            {
                InvocationTypes targetInvocationType = InvocationTypes.Sync;
                AssertReturnTypeCombination(adapterMethod, targetMethod, targetInvocationType);
                return new AdapterInvocationInformation(targetMethod, adapterMethodParameterTypes, targetMethodParmeterTypes, targetInvocationType);
            }
        }

        protected virtual void AssertReturnTypeCombination(MethodInfo adapterMethod, MethodInfo targetMethod, InvocationTypes targetInvocationType)
        {
            Type targetMethodReturnType = targetMethod.ReturnType;
            switch (targetInvocationType)
            {
                case InvocationTypes.Sync:
                    if (AdapterHelper.IsVoid(adapterMethod.ReturnType) && adapterMethod.ReturnType != targetMethodReturnType) throw new AdapterInterceptorException($"Adapter and target method return types should match if either is void.{Environment.NewLine}Adapter method: {adapterMethod.ToLoggerString()}{Environment.NewLine}Target method: {targetMethod.ToLoggerString()}");
                    break;
                case InvocationTypes.GenericTask:
                    if (!AdapterHelper.IsGenericTask(adapterMethod.ReturnType)) throw new AdapterInterceptorException($"Adapter and target method return types should both be a generic Task.{Environment.NewLine}Adapter method: {adapterMethod.ToLoggerString()}{Environment.NewLine}Target method: {targetMethod.ToLoggerString()}");
                    break;
                case InvocationTypes.Task:
                    if (adapterMethod.ReturnType != targetMethodReturnType) throw new AdapterInterceptorException($"Adapter and target method return types should match if target return type is Task.{Environment.NewLine}Adapter method: {adapterMethod.ToLoggerString()}{Environment.NewLine}Target method: {targetMethod.ToLoggerString()}");
                    break;
                case InvocationTypes.GenericValueTask:
                    if (!AdapterHelper.IsGenericValueTask(adapterMethod.ReturnType)) throw new AdapterInterceptorException($"Adapter and target method return types should both be a generic ValueTask.{Environment.NewLine}Adapter method: {adapterMethod.ToLoggerString()}{Environment.NewLine}Target method: {targetMethod.ToLoggerString()}");
                    break;
                case InvocationTypes.ValueTask:
                    if (!AdapterHelper.IsValueTask(adapterMethod.ReturnType)) throw new AdapterInterceptorException($"Adapter and target method return types should match if target return type is ValueTask.{Environment.NewLine}Adapter method: {adapterMethod.ToLoggerString()}{Environment.NewLine}Target method: {targetMethod.ToLoggerString()}");
                    break;
                default: throw new NotImplementedException();
            }
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

            if (targetMethodInfo == null) throw new AdapterInterceptorException($"Target method cannot be found. This is likely due to missing supported type pairs or because target type changed.{Environment.NewLine}Target type: {targetType.ToLoggerString()}{Environment.NewLine}Target method: {sourceMethod.ToLoggerString()}{Environment.NewLine}Target method parameter types: {destinationTypes.ToLoggerString()}");

            return targetMethodInfo;
        }
    }
}
