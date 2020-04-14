// Author: Aleksander Kovač

using Castle.DynamicProxy;
using com.github.akovac35.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace com.github.akovac35.AdapterInterceptor
{
    public class AdapterInterceptor<TTarget> : AdapterInterceptor where TTarget : notnull
    {
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, typeof(TTarget), adapterMapper, loggerFactory)
        {
        }
    }

    public class AdapterInterceptor : IInterceptor
    {
        public AdapterInterceptor(object target, Type targetType, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null)
        {
            _logger = (loggerFactory ?? LoggerFactoryProvider.LoggerFactory).CreateLogger<AdapterInterceptor>();

            Target = target ?? throw new ArgumentNullException(nameof(target));
            TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
            AdapterMapper = adapterMapper ?? throw new ArgumentNullException(nameof(adapterMapper));

            _invokeTargetGenericAsync_Method = AssignTargetGenericAsyncMethod();
        }

        public IAdapterMapper AdapterMapper { get; protected set; }

        public object Target { get; protected set; }

        public Type TargetType { get; protected set; }

        private ILogger _logger;

        protected MethodInfo _invokeTargetGenericAsync_Method;

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

            Type[] targetMethodTypes = AdapterMapper.MapSupportedTypes(adapterMethodTypes);
            MethodInfo targetMethod = AdapterMapper.MapTargetMethod(TargetType, adapterMethod, targetMethodTypes);

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

        protected virtual object? InvokeTarget(MethodInfo adapterMethod, MethodInfo targetMethod, object?[] targetArguments)
        {
            _logger.Here(l => l.Entering());

            AdapterHelper.VerifyIsValidMethodCombination(adapterMethod, targetMethod);

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

        protected virtual MethodInfo AssignTargetGenericAsyncMethod()
        {
            // Will apply to the actual most derived type: https://stackoverflow.com/questions/5780584/will-gettype-return-the-most-derived-type-when-called-from-the-base-class
            return this.GetType().GetMethod("InvokeTargetGenericAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}
