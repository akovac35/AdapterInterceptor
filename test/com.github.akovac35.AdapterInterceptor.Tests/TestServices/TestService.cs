// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using com.github.akovac35.AdapterInterceptor.Tests.TestTypes;
using System;
using System.Threading.Tasks;

namespace com.github.akovac35.AdapterInterceptor.Tests.TestServices
{
    public class TestService: TestServiceBase, IDisposable
    {
        public override string ReturnObject_VirtualMethodWithValueTypeParameters(int a, string b)
        {
            return a + b;
        }

        public async Task<TestType> ReturnGenericTask_MethodWithMixedTypeParametersAsync(TestType a, bool delay = true, bool notProvided1 = true, string notProvided2 = "I was not provided")
        {
            if (!notProvided1) throw new ArgumentException(nameof(notProvided1));
            if (notProvided2 != "I was not provided") throw new ArgumentException(nameof(notProvided2));

            var result = await Task.FromResult(a).ConfigureAwait(false);
            if (delay)
            {
                // Simulate work 
                await Task.Delay(100).ConfigureAwait(false);
            }
            return result;
        }

        public Task<TestType> ReturnGenericTask_MethodWithClassTypeParameters(TestType a)
        {
            var result = Task.FromResult(a);
            return result;
        }

        public Task<TestType> ReturnGenericTask_MethodReturnsNullGenericTask()
        {
            return null;
        }

        public Task ReturnTask_MethodReturnsNullTask()
        {
            return null;
        }

        public async Task ReturnTask_MethodWithMixedTypeParametersAsync(TestType a, bool delay = true)
        {
            if (delay)
            {
                // Simulate work 
                await Task.Delay(100).ConfigureAwait(false);
            }
        }

        public Task ReturnTask_MethodWithClassTypeParameters(TestType a)
        {
            return Task.CompletedTask;
        }

        public async ValueTask<TestType> ReturnGenericValueTask_MethodWithMixedTypeParametersAsync(TestType a, bool delay = true)
        {
            if (delay)
            {
                // Simulate work 
                await Task.Delay(100).ConfigureAwait(false);
            }
            return a;
        }

        public ValueTask<TestType> ReturnGenericValueTask_MethodWithClassTypeParameters(TestType a)
        {
            return new ValueTask<TestType>(a);
        }

        public async ValueTask ReturnValueTask_MethodWithMixedTypeParametersAsync(TestType a, bool delay = true)
        {
            if (delay)
            {
                // Simulate work 
                await Task.Delay(100).ConfigureAwait(false);
            }
        }

        public ValueTask ReturnValueTask_MethodWithClassTypeParameters(TestType a)
        {
            return new ValueTask();
        }

        public bool ReturnBool_MethodWithRefAndOutMixedTypeParameters(ref int a, out TestType b)
        {
            a = 100;
            b = new TestType();
            return true;
        }

        public TestType ReturnTestType_MethodRequiringNullArgumentValueAndReturningNullWithClassTypeParameters(TestType a)
        {
            if (a != null) throw new Exception("Expected null argument value");
            return null;
        }

        public UnknownType ReturnUnknownType_MethodUsingFiveArgumentsOfUnknownType(UnknownType a, UnknownType b, UnknownType c, UnknownType d, UnknownType e)
        {
            return new UnknownType();
        }

        public UnknownType ReturnUnknownType_MethodUsingTwoArgumentsOfUnknownType(UnknownType a, UnknownType b)
        {
            return new UnknownType();
        }

        public async Task<UnknownType> ReturnUnknownType_MethodUsingFiveArgumentsOfUnknownTypeAsync(UnknownType a, UnknownType b, UnknownType c, UnknownType d, UnknownType e)
        {
            return await Task.FromResult(new UnknownType()).ConfigureAwait(false);
        }

        public async Task<UnknownType> ReturnUnknownType_MethodUsingTwoArgumentsOfUnknownTypeAsync(UnknownType a, UnknownType b)
        {
            return await Task.FromResult(new UnknownType()).ConfigureAwait(false);
        }

        public void ThrowsException()
        {
            throw new NotImplementedException();
        }

        public async Task ThrowsExceptionAsync()
        {
            await Task.FromException(new NotImplementedException());
        }

        public TestType MethodUsingOneArgument(TestType a)
        {
            return new TestType();
        }

        public TestType MethodUsingFiveArguments(TestType a, TestType b, TestType c, TestType d, TestType e)
        {
            return new TestType();
        }

        public bool Disposed { get; set; }

        public void Dispose()
        {
            Disposed = true;
        }
    }
}
