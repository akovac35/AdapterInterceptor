// Author: Aleksander Kovač

using Autofac.Extras.DynamicProxy;
using com.github.akovac35.AdapterInterceptor.Tests.TestTypes;
using System.Threading.Tasks;

namespace com.github.akovac35.AdapterInterceptor.Tests.TestServices
{
    public interface ICustomTestService
    {
        void ReturnVoid_MethodWithoutParameters();

        CustomTestType ReturnTestType_MethodWithClassParameters(CustomTestType a);

        string ReturnObject_VirtualMethodWithValueTypeParameters(int a, string b);

        Task<CustomTestType> ReturnGenericTask_MethodWithMixedTypeParametersAsync(CustomTestType a, bool delay = true);

        Task<CustomTestType> ReturnGenericTask_MethodWithClassTypeParameters(CustomTestType a);

        Task<CustomTestType> ReturnGenericTask_MethodReturnsNullTask();

        Task ReturnTask_MethodWithMixedTypeParametersAsync(CustomTestType a, bool delay = true);

        Task ReturnTask_MethodWithClassTypeParameters(CustomTestType a);

        ValueTask<CustomTestType> ReturnGenericValueTask_MethodWithMixedTypeParametersAsync(CustomTestType a, bool delay = true);

        ValueTask<CustomTestType> ReturnGenericValueTask_MethodWithClassTypeParameters(CustomTestType a);

        ValueTask ReturnValueTask_MethodWithMixedTypeParametersAsync(CustomTestType a, bool delay = true);

        ValueTask ReturnValueTask_MethodWithClassTypeParameters(CustomTestType a);

        bool ReturnBool_MethodWithRefAndOutMixedTypeParameters(ref int a, out CustomTestType b);

        CustomTestType ReturnTestType_MethodRequiringNullArgumentValueAndReturningNullWithClassTypeParameters(CustomTestType a);

        public UnknownType ReturnUnknownType_MethodUsingFiveArgumentsOfUnknownType(UnknownType a, UnknownType b, UnknownType c, UnknownType d, UnknownType e);

        public UnknownType ReturnUnknownType_MethodUsingTwoArgumentsOfUnknownType(UnknownType a, UnknownType b);

        Task<UnknownType> ReturnUnknownType_MethodUsingFiveArgumentsOfUnknownTypeAsync(UnknownType a, UnknownType b, UnknownType c, UnknownType d, UnknownType e);

        Task<UnknownType> ReturnUnknownType_MethodUsingTwoArgumentsOfUnknownTypeAsync(UnknownType a, UnknownType b);

        CustomTestType MethodUsingOneArgument(CustomTestType a);

        CustomTestType MethodUsingFiveArguments(CustomTestType a, CustomTestType b, CustomTestType c, CustomTestType d, CustomTestType e);
    }
}
