// Author: Aleksander Kovač

using AutoMapper;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.github.akovac35.AdapterInterceptor
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
            target.Add(sourceType, destinationType);
            Type generic;
            if (addCollectionVariants)
            {
                generic = typeof(IEnumerable<>);
                target.Add(generic.MakeGenericType(sourceType), generic.MakeGenericType(destinationType));

                target.Add(sourceType.MakeArrayType(), destinationType.MakeArrayType());

                generic = typeof(IList<>);
                target.Add(generic.MakeGenericType(sourceType), generic.MakeGenericType(destinationType));

                generic = typeof(List<>);
                target.Add(generic.MakeGenericType(sourceType), generic.MakeGenericType(destinationType));

                generic = typeof(ICollection<>);
                target.Add(generic.MakeGenericType(sourceType), generic.MakeGenericType(destinationType));
            }

            if (addReverseVariants) target.AddTypePair(destinationType, sourceType, addCollectionVariants, addReverseVariants: false);

            return target;
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
