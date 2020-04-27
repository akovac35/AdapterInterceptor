// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace com.github.akovac35.AdapterInterceptor.Helper
{
    public static class AdapterHelper
    {
        public static Dictionary<Type, Type> InitializeSupportedTypePairs()
        {
            return new Dictionary<Type, Type>();
        }

        public static IDictionary<Type, Type> AddTypePair<TSource, TTarget>(this IDictionary<Type, Type> target, bool addCollectionVariants = true, bool addReverseVariants = false)
        {
            return target.AddTypePair(typeof(TSource), typeof(TTarget), addCollectionVariants, addReverseVariants);
        }

        public static IDictionary<Type, Type> AddTypePair(this IDictionary<Type, Type> target, Type sourceType, Type destinationType, bool addCollectionVariants = true, bool addReverseVariants = false)
        {
            target.AddWithValidation(sourceType, destinationType);
            Type generic;
            if (addCollectionVariants)
            {
                generic = typeof(IEnumerable<>);
                target.AddWithValidation(generic.MakeGenericType(sourceType), generic.MakeGenericType(destinationType));

                target.AddWithValidation(sourceType.MakeArrayType(), destinationType.MakeArrayType());

                generic = typeof(IList<>);
                target.AddWithValidation(generic.MakeGenericType(sourceType), generic.MakeGenericType(destinationType));

                generic = typeof(List<>);
                target.AddWithValidation(generic.MakeGenericType(sourceType), generic.MakeGenericType(destinationType));

                generic = typeof(ICollection<>);
                target.AddWithValidation(generic.MakeGenericType(sourceType), generic.MakeGenericType(destinationType));
            }

            if (addReverseVariants) target.AddTypePair(destinationType, sourceType, addCollectionVariants, addReverseVariants: false);

            return target;
        }

        private static void AddWithValidation(this IDictionary<Type, Type> target, Type key, Type value)
        {
            if (target.TryGetValue(key, out Type tmp))
            {
                if (tmp != value) throw new AdapterInterceptorException($"Source type mapping for type {key.ToLoggerString(simpleType: true)} is already defined for type {tmp.ToLoggerString(simpleType: true)} and can't be added for type {value.ToLoggerString(simpleType: true)}. Review documentation and usage or use a less generic variant AdapterInterceptor<TTarget>.");
            }
            else
            {
                target.Add(key, value);
            }
        }

        private static Type _void = typeof(void);

        public static bool IsVoid(Type t)
        {
            if (t == _void)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static Type _genericValueTask = typeof(ValueTask<>);

        public static bool IsGenericValueTask(Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == _genericValueTask)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static Type _valueTask = typeof(ValueTask);

        public static bool IsValueTask(Type t)
        {
            if (_valueTask.IsAssignableFrom(t))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static Type _task = typeof(Task);

        public static bool IsGenericTask(Type t)
        {
            if (t.IsGenericType && _task.IsAssignableFrom(t))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsTask(Type t)
        {
            if (_task.IsAssignableFrom(t))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string? ToLoggerString(this Type? type, bool simpleType = false)
        {
            if (type == null) return null;
            return $"{{Type: {(simpleType ? type.FullName : type.AssemblyQualifiedName)}}}";
        }

        public static string? ToLoggerString(this Type[]? types, bool simpleType = false)
        {
            if (types == null) return null;
            return $"{{Type[{types.Length}]: [{String.Join(",", types.Select(item => item.ToLoggerString(simpleType)).ToArray())}]}}";
        }

        public static string? ToLoggerString(this ParameterInfo? parameterInfo, bool simpleType = false)
        {
            if (parameterInfo == null) return null;
            return $"{{ParameterInfo: {parameterInfo.Name}, {parameterInfo.ParameterType.ToLoggerString(simpleType)}}}";
        }

        public static string? ToLoggerString(this ParameterInfo[]? parameterInfos, bool simpleType = false)
        {
            if (parameterInfos == null) return null;
            return $"{{ParameterInfo[{parameterInfos.Length}]: [{String.Join(",", parameterInfos.Select(item => item.ToLoggerString(simpleType)).ToArray())}]}}";
        }

        public static string? ToLoggerString(this MethodInfo? methodInfo, bool simpleType = false)
        {
            if (methodInfo == null) return null;
            return $"{{MethodInfo: {methodInfo.Name}, Parameters: {methodInfo.GetParameters().ToLoggerString(simpleType)}, ReturnType: {methodInfo.ReturnType.ToLoggerString(simpleType)}, DeclaringType: {methodInfo.DeclaringType.ToLoggerString(simpleType)}}}";
        }

        /// <summary>
        /// This method will not use Arguments and ReturnValue - pass them to the logger separately for destructuring.
        /// </summary>
        /// <param name="invocation">Proxy invocation object</param>
        /// <returns>Proxy invocation object converted to string</returns>
        public static string? ToLoggerString(this IInvocation? invocation, bool simpleType = false)
        {
            if (invocation == null) return null;
            return $"{{Invocation: {invocation.Method.ToLoggerString(simpleType)}}}";
        }
    }
}
