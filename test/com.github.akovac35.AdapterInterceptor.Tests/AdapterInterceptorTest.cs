// Author: Aleksander Kovač

using Autofac;
using AutoMapper;
using com.github.akovac35.AdapterInterceptor.Tests.TestServices;
using com.github.akovac35.AdapterInterceptor.Tests.TestTypes;
using DeepEqual.Syntax;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace com.github.akovac35.AdapterInterceptor.Tests
{
    [TestFixture]
    public class AdapterInterceptorTest: AdapterInterceptor
    {
        public AdapterInterceptorTest(): base(TestHelper.LoggerFactory)
        {

        }

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
                Func<object, object[], object> ICustomTestServiceSimpleMethod = (service, args) => { ((ICustomTestService)service).SimpleMethod(); return null; };
                Func<object, object[], object> ICustomTestServiceMethodUsingType = (service, args) => ((ICustomTestService)service).MethodUsingType((CustomTestType)args[0]);
                Func<object, object[], object> ICustomTestServiceVirtualMethod = (service, args) => ((ICustomTestService)service).VirtualMethod((int)args[0], (string)args[1]);

                return new[]
                {
                    new object[] { Container.Resolve<ICustomTestService>(), null, ICustomTestServiceSimpleMethod, null },
                    new object[] { Container.Resolve<ICustomTestService>(), new object[] { new CustomTestType(new TestType()) }, ICustomTestServiceMethodUsingType, new CustomTestType(new TestType()) },
                    new object[] { Container.Resolve<ICustomTestService>(), new object[] { 100, "testing" }, ICustomTestServiceVirtualMethod, new object() }

                };
            }
        }

        [TestCaseSource("SynchronousInvocation_Cases")]
        public void Intercept_ForSynchronousInvocation_Adapts(object service, object[] inputs, Func<object, object[], object> function, object expectedResult)
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

        
        static object[] AsynchronousInvocation_Cases
        {
            get
            {
                Func<object, object[], Task<CustomTestType>> ICustomTestServiceTestMethodAsync = (async (service, args) => await ((ICustomTestService)service).TestMethodAsync((CustomTestType)args[0]).ConfigureAwait(false));
                Func<object, object[], Task<CustomTestType>> ICustomTestServiceTestMethodReturningTask = (async (service, args) => await ((ICustomTestService)service).TestMethodReturningTask((CustomTestType)args[0]).ConfigureAwait(false));

                return new[]
                {
                    new object[] { Container.Resolve<ICustomTestService>(), new object[] { new CustomTestType(new TestType()) }, ICustomTestServiceTestMethodAsync, new CustomTestType(new TestType()) },
                    new object[] { Container.Resolve<ICustomTestService>(), new object[] { new CustomTestType(new TestType()) }, ICustomTestServiceTestMethodReturningTask, new CustomTestType(new TestType()) }
                };
            }
        }

        [TestCaseSource("AsynchronousInvocation_Cases")]
        public void Intercept_ForAsynchronousInvocation_Adapts(object service, object[] inputs, Func<object, object[], Task> function, object expectedResult)
        {
            object result = ((dynamic)function(service, inputs)).Result;
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

        static object[] KnownMethod_Cases =
        {
            new object[] { typeof(ICustomTestService), "SimpleMethod", new Type[] { } },
            new object[] { typeof(ICustomTestService), "VirtualMethod", new Type[] { typeof(int), typeof(string) } },
            new object[] { typeof(ICustomTestService), "TestMethodAsync", new Type[] {typeof(CustomTestType), typeof(bool)} }
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


    }
}
