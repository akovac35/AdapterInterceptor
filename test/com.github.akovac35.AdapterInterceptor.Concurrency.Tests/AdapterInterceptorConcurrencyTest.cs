// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using Autofac;
using com.github.akovac35.AdapterInterceptor.Tests.TestServices;
using com.github.akovac35.AdapterInterceptor.Tests.TestTypes;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace com.github.akovac35.AdapterInterceptor.Tests
{
    [TestFixture]
    public class AdapterInterceptorConcurrencyTest
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [SetUp]
        public void SetUp()
        {
        }

        static IContainer Container { get; set; } = TestHelper.CreateContainerBuilder();

        [Test]
        public void Concurrent_Intercept_Works()
        {
            // Must be without enabled logging to avoid consuming resources
            var service = Container.ResolveNamed<ICustomTestService<CustomTestType>>("GenericCustomTestService");

            // Aiming for about 10s of work
            Action action1 = () =>
            {
                for (int i = 0; i < 900000; i++)
                {                    
                    var tmp = service.ReturnGenericTask_MethodWithMixedTypeParametersAsync(new CustomTestType(), false).Result;
                }
            };
            Action action2 = () =>
            {
                for (int i = 0; i < 900000; i++)
                {
                    var tmp = service.ReturnGenericValueTask_MethodWithMixedTypeParametersAsync(new CustomTestType(), false).Result;
                }
            };
            Action action3 = () =>
            {
                for (int i = 0; i < 900000; i++)
                {
                    int a = 0;
                    var tmp = service.ReturnBool_MethodWithRefAndOutMixedTypeParameters(ref a, out _);
                }
            };
            Action action4 = () =>
            {
                for (int i = 0; i < 900000; i++)
                {
                    service.ReturnVoid_MethodWithoutParameters();
                }
            };

            Parallel.Invoke(action1, action2, action3, action4);
        }
    }
}
