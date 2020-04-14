// Author: Aleksander Kovač

using AutoMapper;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace com.github.akovac35.AdapterInterceptor
{
    public static class AdapterHelper
    {
        public static IList<TypePair> CreateList()
        {
            return new List<TypePair>();
        }

        public static IList<TypePair> AddPair<TSource, TTarget>(this IList<TypePair> target, bool addCollectionVariants = true, bool addReverseVariants = false)
        {
            return target.AddPair(typeof(TSource), typeof(TTarget), addCollectionVariants, addReverseVariants);

        }

        public static IList<TypePair> AddPair(this IList<TypePair> target, Type sourceType, Type destinationType, bool addCollectionVariants = true, bool addReverseVariants = false)
        {
            target.Add(new TypePair(sourceType, destinationType));
            Type generic;
            if (addCollectionVariants)
            {
                generic = typeof(IEnumerable<>);
                target.Add(new TypePair(generic.MakeGenericType(sourceType), generic.MakeGenericType(destinationType)));

                target.Add(new TypePair(sourceType.MakeArrayType(), destinationType.MakeArrayType()));

                generic = typeof(IList<>);
                target.Add(new TypePair(generic.MakeGenericType(sourceType), generic.MakeGenericType(destinationType)));

                generic = typeof(List<>);
                target.Add(new TypePair(generic.MakeGenericType(sourceType), generic.MakeGenericType(destinationType)));

                generic = typeof(ICollection<>);
                target.Add(new TypePair(generic.MakeGenericType(sourceType), generic.MakeGenericType(destinationType)));
            }

            if (addReverseVariants) target.AddPair(destinationType, sourceType, addCollectionVariants, addReverseVariants: false);

            return target;
        }

        public static void VerifyIsValidMethodCombination(MethodInfo adapterMethod, MethodInfo targetMethod)
        {
            if (adapterMethod.ReturnType == typeof(void) && adapterMethod.ReturnType != targetMethod.ReturnType) throw new AdapterInterceptorException($"Adapter and target method return types should match if either is void. Adapter method: {adapterMethod.ToLoggerString()}, Target method: {targetMethod.ToLoggerString()}");

            if (typeof(Task).IsAssignableFrom(adapterMethod.ReturnType) != typeof(Task).IsAssignableFrom(targetMethod.ReturnType)) throw new AdapterInterceptorException($"Adapter and target method return types should match if either is Task. Adapter method: {adapterMethod.ToLoggerString()}, Target method: {targetMethod.ToLoggerString()}");
        }

        public static string? ToLoggerString(this Type? type, bool simpleType = false)
        {
            if (type == null) return null;
            return $"{{Type: {(simpleType ? type.FullName : type.AssemblyQualifiedName)}}}";
        }

        #region Logger
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
        #endregion
    }
}
