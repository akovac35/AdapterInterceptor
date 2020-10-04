// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using com.github.akovac35.AdapterInterceptor.Tests.TestTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.github.akovac35.AdapterInterceptor.Tests.TestServices
{
    public interface ICustomTestService<T> : IDisposable
    {
        void ReturnVoid_MethodWithoutParameters();

        T ReturnTestType_MethodWithClassParameters(T a);

        string ReturnObject_VirtualMethodWithValueTypeParameters(int a, string b);

        Task<T> ReturnGenericTask_MethodWithMixedTypeParametersAsync(T a, bool delay = true, bool notProvided1 = true, string notProvided2 = "I was not provided");

        Task<T> ReturnGenericTask_MethodWithClassTypeParameters(T a);

        Task<T> ReturnGenericTask_MethodReturnsNullGenericTask();

        Task ReturnTask_MethodReturnsNullTask();

        Task ReturnTask_MethodWithMixedTypeParametersAsync(T a, bool delay = true);

        Task ReturnTask_MethodWithClassTypeParameters(T a);

        ValueTask<T> ReturnGenericValueTask_MethodWithMixedTypeParametersAsync(T a, bool delay = true);

        ValueTask<T> ReturnGenericValueTask_MethodWithClassTypeParameters(T a);

        ValueTask ReturnValueTask_MethodWithMixedTypeParametersAsync(T a, bool delay = true);

        ValueTask ReturnValueTask_MethodWithClassTypeParameters(T a);

        bool ReturnBool_MethodWithRefAndOutMixedTypeParameters(ref int a, out T b);

        T ReturnTestType_MethodRequiringNullArgumentValueAndReturningNullWithClassTypeParameters(T a);

        public UnknownType ReturnUnknownType_MethodUsingFiveArgumentsOfUnknownType(UnknownType a, UnknownType b, UnknownType c, UnknownType d, UnknownType e);

        public UnknownType ReturnUnknownType_MethodUsingTwoArgumentsOfUnknownType(UnknownType a, UnknownType b);

        Task<UnknownType> ReturnUnknownType_MethodUsingFiveArgumentsOfUnknownTypeAsync(UnknownType a, UnknownType b, UnknownType c, UnknownType d, UnknownType e);

        Task<UnknownType> ReturnUnknownType_MethodUsingTwoArgumentsOfUnknownTypeAsync(UnknownType a, UnknownType b);

        void ThrowsException();

        Task ThrowsExceptionAsync();

        T MethodUsingOneArgument(T a);

        T MethodUsingFiveArguments(T a, T b, T c, T d, T e);

        Task<KeyValuePair<int, int>> ReturnTask_MedthodUsingExecutionContext();

        bool IsDisposed { get; set; }

        // The Dispose() method signature is provided by the IDisposable interface ...
    }
}