// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using Autofac;
using com.github.akovac35.AdapterInterceptor.Helper;
using com.github.akovac35.AdapterInterceptor.Tests.TestServices;
using com.github.akovac35.AdapterInterceptor.Tests.TestTypes;
using DeepEqual.Syntax;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace com.github.akovac35.AdapterInterceptor.Tests
{
    [TestFixture]
    public class AdapterInterceptorTest : AdapterInterceptor<object>
    {
        public AdapterInterceptorTest() : base(TestHelper.LoggerFactory)
        {
            _logger = TestHelper.LoggerFactory.CreateLogger<AdapterInterceptorTest>();
        }

        private readonly ILogger _logger;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [SetUp]
        public void SetUp()
        {
        }

        static IContainer Container { get; set; } = TestHelper.CreateContainerBuilder();

        static object[] SynchronousInvocation_Cases
        {
            get
            {
                Func<object, object[], object> ReturnVoid_MethodWithoutParameters = (service, args) => { ((ICustomTestService)service).ReturnVoid_MethodWithoutParameters(); return null; };
                Func<object, object[], object> ReturnTestType_MethodWithClassParameters = (service, args) => ((ICustomTestService)service).ReturnTestType_MethodWithClassParameters((CustomTestType)args[0]);
                Func<object, object[], object> ReturnObject_VirtualMethodWithValueTypeParameters = (service, args) => ((ICustomTestService)service).ReturnObject_VirtualMethodWithValueTypeParameters((int)args[0], (string)args[1]);
                Func<object, object[], object> ReturnTestType_MethodRequiringNullArgumentValueAndReturningNullWithClassTypeParameters = (service, args) => ((ICustomTestService)service).ReturnTestType_MethodRequiringNullArgumentValueAndReturningNullWithClassTypeParameters((CustomTestType)args[0]);

                return new[]
                {
                    new object[] { nameof(ReturnVoid_MethodWithoutParameters), Container.Resolve<ICustomTestService>(), null, ReturnVoid_MethodWithoutParameters, null },
                    new object[] { nameof(ReturnTestType_MethodWithClassParameters), Container.Resolve<ICustomTestService>(), new object[] { new CustomTestType(new TestType()) }, ReturnTestType_MethodWithClassParameters, new CustomTestType(new TestType()) },
                    new object[] { nameof(ReturnObject_VirtualMethodWithValueTypeParameters), Container.Resolve<ICustomTestService>(), new object[] { 100, "testing" }, ReturnObject_VirtualMethodWithValueTypeParameters, "100testing" },
                    new object[] { nameof(ReturnTestType_MethodRequiringNullArgumentValueAndReturningNullWithClassTypeParameters), Container.Resolve<ICustomTestService>(), new object[] { null }, ReturnTestType_MethodRequiringNullArgumentValueAndReturningNullWithClassTypeParameters, null }
                };
            }
        }

        [TestCaseSource("SynchronousInvocation_Cases")]
        public void Intercept_ForSynchronousInvocation_Adapts(string testName, object service, object[] inputs, Func<object, object[], object> function, object expectedResult)
        {
            object result = function(service, inputs);
            if (expectedResult != null)
            {
                Assert.IsNotNull(result);
                Assert.IsTrue(expectedResult.GetType() == result.GetType());
                Assert.IsTrue(expectedResult.IsDeepEqual(result));
            }
            else
            {
                Assert.IsTrue(expectedResult == result);
            }
        }

        [Test]
        public void Intercept_MethodWithRefAndOutParameters_Adapts()
        {
            var service = Container.Resolve<ICustomTestService>();
            object[] inputs = null;
            bool expectedResult = true;
            var expectedOut = new CustomTestType(new TestType());

            int a = 0;
            CustomTestType mappedOut = null;
            Func<object, object[], object> ICustomTestServiceMethodWithOutParameter = (service, args) => ((ICustomTestService)service).ReturnBool_MethodWithRefAndOutMixedTypeParameters(ref a, out mappedOut);
            object result = ICustomTestServiceMethodWithOutParameter(service, inputs);

            Assert.IsNotNull(result);
            Assert.IsTrue(expectedResult.GetType() == result.GetType());
            Assert.IsTrue(expectedResult.IsDeepEqual(result));

            Assert.IsTrue(a == 100);

            Assert.IsNotNull(mappedOut);
            Assert.IsTrue(expectedOut.GetType() == mappedOut.GetType());
            Assert.IsTrue(expectedOut.IsDeepEqual(mappedOut));
        }

        static object[] AsynchronousTaskInvocation_Cases
        {
            get
            {
                Func<object, object[], Task<CustomTestType>> ReturnGenericTask_MethodWithMixedTypeParametersAsync = (async (service, args) => await ((ICustomTestService)service).ReturnGenericTask_MethodWithMixedTypeParametersAsync((CustomTestType)args[0]).ConfigureAwait(false));
                Func<object, object[], Task<CustomTestType>> ReturnGenericTask_MethodWithClassTypeParameters = ((service, args) => ((ICustomTestService)service).ReturnGenericTask_MethodWithClassTypeParameters((CustomTestType)args[0]));
                Func<object, object[], Task<CustomTestType>> ReturnGenericTask_MethodReturnsNull = ((service, args) => ((ICustomTestService)service).ReturnGenericTask_MethodReturnsNullTask());

                Func<object, object[], Task> ReturnTask_MethodWithMixedTypeParametersAsync = (async (service, args) => await ((ICustomTestService)service).ReturnTask_MethodWithMixedTypeParametersAsync((CustomTestType)args[0]).ConfigureAwait(false));
                Func<object, object[], Task> ReturnTask_MethodWithClassTypeParameters = ((service, args) => ((ICustomTestService)service).ReturnTask_MethodWithClassTypeParameters((CustomTestType)args[0]));

                return new[]
                {
                    new object[] { nameof(ReturnGenericTask_MethodWithMixedTypeParametersAsync), Container.Resolve<ICustomTestService>(), new object[] { new CustomTestType(new TestType()) }, ReturnGenericTask_MethodWithMixedTypeParametersAsync, new CustomTestType(new TestType()), true, true },
                    new object[] { nameof(ReturnGenericTask_MethodWithMixedTypeParametersAsync), Container.Resolve<ICustomTestService>(), new object[] { null }, ReturnGenericTask_MethodWithMixedTypeParametersAsync, null, true, true },
                    new object[] { nameof(ReturnGenericTask_MethodWithClassTypeParameters), Container.Resolve<ICustomTestService>(), new object[] { new CustomTestType(new TestType()) }, ReturnGenericTask_MethodWithClassTypeParameters, new CustomTestType(new TestType()), true, false },
                    new object[] { nameof(ReturnGenericTask_MethodWithClassTypeParameters), Container.Resolve<ICustomTestService>(), new object[] { null }, ReturnGenericTask_MethodWithClassTypeParameters, null, true, false },
                    //new object[] { nameof(ReturnGenericTask_MethodReturnsNull), Container.Resolve<ICustomTestService>(), new object[] { null }, ReturnGenericTask_MethodReturnsNull, null, true, false },

                    new object[] { nameof(ReturnTask_MethodWithMixedTypeParametersAsync), Container.Resolve<ICustomTestService>(), new object[] { new CustomTestType(new TestType()) }, ReturnTask_MethodWithMixedTypeParametersAsync, null, false, true },
                    new object[] { nameof(ReturnTask_MethodWithClassTypeParameters), Container.Resolve<ICustomTestService>(), new object[] { new CustomTestType(new TestType()) }, ReturnTask_MethodWithClassTypeParameters, null, false, false }
                };
            }
        }

        [TestCaseSource("AsynchronousTaskInvocation_Cases")]
        public void Intercept_ForAsynchronousTaskInvocation_Adapts(string testName, object service, object[] inputs, Func<object, object[], Task> function, object expectedResult, bool isGeneric, bool isAsync)
        {
            Task tmp = function(service, inputs);
            object result = null;
            if (isGeneric)
            {
                result = ((dynamic)tmp).Result;
            }
            else if (isAsync)
            {
                tmp.ConfigureAwait(false).GetAwaiter().GetResult();
            }

            Assert.IsTrue(tmp.IsCompletedSuccessfully);
            if (expectedResult != null)
            {
                Assert.IsNotNull(result);
                Assert.IsTrue(expectedResult.GetType() == result.GetType());
                Assert.IsTrue(expectedResult.IsDeepEqual(result));
            }
            else
            {
                Assert.IsTrue(expectedResult == result);
            }
        }

        static object[] AsynchronousValueTaskInvocation_Cases
        {
            get
            {
                Func<object, object[], ValueTask<CustomTestType>> ReturnGenericValueTask_MethodWithMixedTypeParametersAsync = (async (service, args) => await ((ICustomTestService)service).ReturnGenericValueTask_MethodWithMixedTypeParametersAsync((CustomTestType)args[0]).ConfigureAwait(false));
                Func<object, object[], ValueTask<CustomTestType>> ReturnGenericValueTask_MethodWithClassTypeParameters = ((service, args) => ((ICustomTestService)service).ReturnGenericValueTask_MethodWithClassTypeParameters((CustomTestType)args[0]));

                Func<object, object[], ValueTask> ReturnValueTask_MethodWithMixedTypeParametersAsync = (async (service, args) => await ((ICustomTestService)service).ReturnValueTask_MethodWithMixedTypeParametersAsync((CustomTestType)args[0]).ConfigureAwait(false));
                Func<object, object[], ValueTask> ReturnValueTask_MethodWithClassTypeParameters = ((service, args) => ((ICustomTestService)service).ReturnValueTask_MethodWithClassTypeParameters((CustomTestType)args[0]));

                return new[]
                {
                    new object[] { nameof(ReturnGenericValueTask_MethodWithMixedTypeParametersAsync), Container.Resolve<ICustomTestService>(), new object[] { new CustomTestType(new TestType()) }, ReturnGenericValueTask_MethodWithMixedTypeParametersAsync, new CustomTestType(new TestType()), true, true },
                    new object[] { nameof(ReturnGenericValueTask_MethodWithClassTypeParameters), Container.Resolve<ICustomTestService>(), new object[] { new CustomTestType(new TestType()) }, ReturnGenericValueTask_MethodWithClassTypeParameters, new CustomTestType(new TestType()), true, false },
                    new object[] { nameof(ReturnGenericValueTask_MethodWithMixedTypeParametersAsync), Container.Resolve<ICustomTestService>(), new object[] { null }, ReturnGenericValueTask_MethodWithMixedTypeParametersAsync, null, true, true },
                    new object[] { nameof(ReturnGenericValueTask_MethodWithClassTypeParameters), Container.Resolve<ICustomTestService>(), new object[] { null }, ReturnGenericValueTask_MethodWithClassTypeParameters, null, true, false },

                    new object[] { nameof(ReturnValueTask_MethodWithMixedTypeParametersAsync), Container.Resolve<ICustomTestService>(), new object[] { new CustomTestType(new TestType()) }, ReturnValueTask_MethodWithMixedTypeParametersAsync, null, false, true },
                    new object[] { nameof(ReturnValueTask_MethodWithClassTypeParameters), Container.Resolve<ICustomTestService>(), new object[] { new CustomTestType(new TestType()) }, ReturnValueTask_MethodWithClassTypeParameters, null, false, false },
                };
            }
        }

        [TestCaseSource("AsynchronousValueTaskInvocation_Cases")]
        public void Intercept_ForAsynchronousValueTaskInvocation_Adapts(string testName, object service, object[] inputs, object function, object expectedResult, bool isGeneric, bool isAsync)
        {
            object result = null;
            if (isGeneric)
            {
                var tmp = ((dynamic)function)(service, inputs);
                result = tmp.Result;
                Assert.IsTrue(tmp.IsCompletedSuccessfully);
            }
            else if (isAsync)
            {
                var tmp = ((Func<object, object[], ValueTask>)function)(service, inputs);
                tmp.ConfigureAwait(false).GetAwaiter().GetResult();
                Assert.IsTrue(tmp.IsCompletedSuccessfully);
            }
            else
            {
                var tmp = ((Func<object, object[], ValueTask>)function)(service, inputs);
                Assert.IsTrue(tmp.IsCompletedSuccessfully);
            }

            if (expectedResult != null)
            {
                Assert.IsNotNull(result);
                Assert.IsTrue(expectedResult.GetType() == result.GetType());
                Assert.IsTrue(expectedResult.IsDeepEqual(result));
            }
            else
            {
                Assert.IsTrue(expectedResult == result);
            }
        }

        static object[] NullTask_Cases
        {
            get
            {
                Func<object, object[], Task<CustomTestType>> ReturnGenericTask_MethodReturnsNull = ((service, args) => ((ICustomTestService)service).ReturnGenericTask_MethodReturnsNullTask());

                return new[]
                {
            new object[] { nameof(ReturnGenericTask_MethodReturnsNull), Container.Resolve<ICustomTestService>(), new object[] { null }, ReturnGenericTask_MethodReturnsNull, null, true, false }
        };
            }
        }

        [TestCaseSource("NullTask_Cases")]
        public void Intercept_ForNullTask_ThrowsException(string testName, object service, object[] inputs, Func<object, object[], Task<CustomTestType>> function, object expectedResult, bool isGeneric, bool isAsync)
        {
            var exception = Assert.Throws<AggregateException>(() =>
            {
                var tmp = function(service, inputs).Result;
            });
            TestContext.WriteLine(exception.Message);
        }

        static object[] KnownMethod_Cases =
        {
            new object[] { typeof(ICustomTestService), nameof(TestService.ReturnVoid_MethodWithoutParameters), new Type[] { } },
            new object[] { typeof(ICustomTestService), nameof(TestService.ReturnObject_VirtualMethodWithValueTypeParameters), new Type[] { typeof(int), typeof(string) } },
            new object[] { typeof(ICustomTestService), nameof(TestService.ReturnGenericTask_MethodWithMixedTypeParametersAsync), new Type[] {typeof(CustomTestType), typeof(bool)} }
        };

        [TestCaseSource("KnownMethod_Cases")]
        public void MapTargetMethod_ForKnownMethod_FindsMethod(Type targetType, string sourceMethodName, Type[] destinationTypes)
        {
            var sourceMethodStub = new Mock<MethodInfo>();
            sourceMethodStub.Setup(instance => instance.Name).Returns(sourceMethodName);
            var targetMethodInfo = MapTargetMethod(targetType, sourceMethodStub.Object, destinationTypes);

            Assert.IsNotNull(targetMethodInfo);
            Assert.AreEqual(sourceMethodName, targetMethodInfo.Name);
        }

        [Test]
        public void MapTargetMethod_ForUnknownMethod_ThrowsException()
        {
            string sourceMethodName = "xyz";
            Type[] destinationTypes = new Type[] { typeof(int), typeof(string) };
            var sourceMethodStub = new Mock<MethodInfo>();
            sourceMethodStub.Setup(instance => instance.Name).Returns(sourceMethodName);

            var exception = Assert.Throws<AdapterInterceptorException>(() => { MapTargetMethod(typeof(ICustomTestService), sourceMethodStub.Object, destinationTypes); });
            TestContext.WriteLine(exception.Message);
        }

        static object[] MapSupportedTypes_ForKnownType_FindsType_Cases =
        {
            new object[] { typeof(TestType), typeof(CustomTestType), typeof(TestType), typeof(CustomTestType) },
            new object[] { typeof(TestType), typeof(CustomTestType), typeof(IList<TestType>), typeof(IList<CustomTestType>) },
            new object[] { typeof(TestType), typeof(CustomTestType), typeof(List<TestType>), typeof(List<CustomTestType>) },
            new object[] { typeof(TestType), typeof(CustomTestType), typeof(TestType[]), typeof(CustomTestType[]) },
            new object[] { typeof(TestType), typeof(CustomTestType), typeof(IEnumerable<TestType>), typeof(IEnumerable<CustomTestType>) },
            new object[] { typeof(TestType), typeof(CustomTestType), typeof(ICollection<TestType>), typeof(ICollection<CustomTestType>) }
        };

        [TestCaseSource("MapSupportedTypes_ForKnownType_FindsType_Cases")]
        public void MapSupportedTypes_ForKnownType_FindsType(Type configSourceType, Type configDestinationType, Type instanceSourceType, Type instanceDestinationType)
        {
            try
            {
                var supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
                AdapterHelper.AddTypePair(supportedTypePairs, configSourceType, configDestinationType);
                SupportedTypePairs = supportedTypePairs;

                var mappedInstanceDestinationTypes = MapSupportedTypes(new Type[] { instanceSourceType });

                Assert.IsNotNull(mappedInstanceDestinationTypes);
                Assert.IsTrue(mappedInstanceDestinationTypes.Length == 1);
                Assert.IsTrue(instanceSourceType != mappedInstanceDestinationTypes[0]);
                Assert.IsTrue(mappedInstanceDestinationTypes[0] == instanceDestinationType);
            }
            finally
            {
                SupportedTypePairs = null;
            }
        }

        [Test]
        public void MapSupportedTypes_ForUnknownType_FindsSourceType([Values(typeof(TestType))] Type sourceType)
        {
            try
            {
                SupportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();

                var mappedDestinationTypes = MapSupportedTypes(new Type[] { sourceType });

                Assert.IsNotNull(mappedDestinationTypes);
                Assert.IsTrue(mappedDestinationTypes.Length == 1);
                Assert.IsTrue(sourceType == mappedDestinationTypes[0]);
            }
            finally
            {
                SupportedTypePairs = null;
            }
        }

        [Test]
        public void AdapterInterceptor_GenericType_Works()
        {
            var service = Container.ResolveNamed<ICustomTestService>("GenericCustomTestService");
            var result = service.ReturnTestType_MethodWithClassParameters(new CustomTestType(new TestType()));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsDeepEqual(new CustomTestType(new TestType())));
        }

        [Test]
        public void AdapterInterceptor_InvalidDeclaration_ThrowsException()
        {
            var typePair = AdapterHelper.InitializeSupportedTypePairs();
            typePair.AddTypePair(typeof(TestType), typeof(CustomTestType));
            // Should not throw exception, we are just repeating what is already defined
            typePair.AddTypePair(typeof(CustomTestType), typeof(TestType));

            var ex1 = Assert.Throws<AdapterInterceptorException>(() =>
            {
                typePair.AddTypePair(typeof(TestType), typeof(UnknownType));
            }, "Source type mapping for type {Type: com.github.akovac35.AdapterInterceptor.Tests.TestTypes.TestType} is already defined for type {Type: com.github.akovac35.AdapterInterceptor.Tests.TestTypes.CustomTestType} and can't be added for type {Type: com.github.akovac35.AdapterInterceptor.Tests.TestTypes.UnknownType}. Review documentation and usage or use a less generic constructor AdapterInterceptor<TTarget>.");
            TestContext.WriteLine(ex1.Message);

            var ex2 = Assert.Throws<AdapterInterceptorException>(() =>
            {
                typePair.AddTypePair(typeof(CustomTestType), typeof(UnknownType));
            }, "Source type mapping for type {Type: com.github.akovac35.AdapterInterceptor.Tests.TestTypes.CustomTestType} is already defined for type {Type: com.github.akovac35.AdapterInterceptor.Tests.TestTypes.TestType} and can't be added for type {Type: com.github.akovac35.AdapterInterceptor.Tests.TestTypes.UnknownType}. Review documentation and usage or use a less generic constructor AdapterInterceptor<TTarget>.");
            TestContext.WriteLine(ex2.Message);
        }
    }
}
