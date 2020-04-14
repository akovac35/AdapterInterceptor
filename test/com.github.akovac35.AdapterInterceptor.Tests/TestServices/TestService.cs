// Author: Aleksander Kovač

using com.github.akovac35.AdapterInterceptor.Tests.TestTypes;
using System.Threading.Tasks;

namespace com.github.akovac35.AdapterInterceptor.Tests.TestServices
{
    public class TestService
    {
        public void SimpleMethod()
        {
        }

        public TestType MethodUsingType(TestType a)
        {
            return new TestType();
        }

        public virtual object VirtualMethod(int a, string b)
        {
            return new object();
        }

        public async Task<TestType> TestMethodAsync(TestType a)
        {
            var result = await Task.FromResult(a).ConfigureAwait(false) ;
            return result;
        }

        public Task<TestType> TestMethodReturningTask(TestType a)
        {
            var result = Task.FromResult(a);
            return result;
        }
    }
}
