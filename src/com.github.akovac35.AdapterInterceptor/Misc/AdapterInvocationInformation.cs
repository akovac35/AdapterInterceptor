// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using System;
using System.Reflection;

namespace com.github.akovac35.AdapterInterceptor.Misc
{
    /// <summary>
    /// Contains basic information about adapter invocation.
    /// </summary>
    public class AdapterInvocationInformation
    {
        public MethodInfo TargetMethod { get; }

        public MethodInfo? InvocationHelper { get; }

        public Type[] AdapterMethodParameterTypes { get; }

        public Type[] TargetMethodParameterTypes { get; }

        public InvocationTypes TargetInvocationType { get; }

        /// <summary>
        /// Initializes a new AdapterInvocationInformation instance.
        /// </summary>
        /// <param name="targetMethod">Target method.</param>
        /// <param name="adapterMethodParameterTypes">Adapter method parameter types.</param>
        /// <param name="targetMethodParameterTypes">Target method parameter types.</param>
        /// <param name="targetInvocationType">Target invocation type.</param>
        /// <param name="invocationHelper">Generic method instance of AdapterInterceptor's generic method definition for invoking methods returning generic tasks.</param>
        public AdapterInvocationInformation(MethodInfo targetMethod, Type[] adapterMethodParameterTypes, Type[] targetMethodParameterTypes, InvocationTypes targetInvocationType, MethodInfo? invocationHelper = null)
        {
            TargetMethod = targetMethod;
            AdapterMethodParameterTypes = adapterMethodParameterTypes;
            TargetMethodParameterTypes = targetMethodParameterTypes;
            TargetInvocationType = targetInvocationType;
            InvocationHelper = invocationHelper;
        }
    }
}
