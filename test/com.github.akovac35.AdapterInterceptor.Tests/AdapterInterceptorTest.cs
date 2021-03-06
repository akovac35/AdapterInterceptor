﻿// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using Autofac;
using Castle.DynamicProxy;
using com.github.akovac35.AdapterInterceptor.Helper;
using com.github.akovac35.AdapterInterceptor.Misc;
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
                Func<object, object[], object> ReturnVoid_MethodWithoutParameters = (service, args) => { ((ICustomTestService<CustomTestType>)service).ReturnVoid_MethodWithoutParameters(); return null; };
                Func<object, object[], object> ReturnTestType_MethodWithClassParameters = (service, args) => ((ICustomTestService<CustomTestType>)service).ReturnTestType_MethodWithClassParameters((CustomTestType)args[0]);
                Func<object, object[], object> ReturnObject_VirtualMethodWithValueTypeParameters = (service, args) => ((ICustomTestService<CustomTestType>)service).ReturnObject_VirtualMethodWithValueTypeParameters((int)args[0], (string)args[1]);
                Func<object, object[], object> ReturnTestType_MethodRequiringNullArgumentValueAndReturningNullWithClassTypeParameters = (service, args) => ((ICustomTestService<CustomTestType>)service).ReturnTestType_MethodRequiringNullArgumentValueAndReturningNullWithClassTypeParameters((CustomTestType)args[0]);
                Func<object, object[], object> Disposable = (service, args) => { ((IDisposable)service).Dispose(); return ((ICustomTestService<CustomTestType>)service).IsDisposed; };

                return new[]
                {
                    new object[] { nameof(ReturnVoid_MethodWithoutParameters), Container.Resolve<ICustomTestService<CustomTestType>>(), null, ReturnVoid_MethodWithoutParameters, null },
                    new object[] { nameof(ReturnTestType_MethodWithClassParameters), Container.Resolve<ICustomTestService<CustomTestType>>(), new object[] { new CustomTestType(new TestType()) }, ReturnTestType_MethodWithClassParameters, new CustomTestType(new TestType()) },
                    new object[] { nameof(ReturnObject_VirtualMethodWithValueTypeParameters), Container.Resolve<ICustomTestService<CustomTestType>>(), new object[] { 100, "testing" }, ReturnObject_VirtualMethodWithValueTypeParameters, "100testing" },
                    new object[] { nameof(ReturnTestType_MethodRequiringNullArgumentValueAndReturningNullWithClassTypeParameters), Container.Resolve<ICustomTestService<CustomTestType>>(), new object[] { null }, ReturnTestType_MethodRequiringNullArgumentValueAndReturningNullWithClassTypeParameters, null },
                    new object[] { nameof(Disposable), Container.Resolve<ICustomTestService<CustomTestType>>(), new object[] { null }, Disposable, true }
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
            var service = Container.Resolve<ICustomTestService<CustomTestType>>();
            object[] inputs = null;
            bool expectedResult = true;
            var expectedOut = new CustomTestType(new TestType());

            int a = 0;
            CustomTestType mappedOut = null;
            Func<object, object[], object> ICustomTestServiceMethodWithOutParameter = (service, args) => ((ICustomTestService<CustomTestType>)service).ReturnBool_MethodWithRefAndOutMixedTypeParameters(ref a, out mappedOut);
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
                Func<object, object[], Task<CustomTestType>> ReturnGenericTask_MethodWithMixedTypeParametersAsync = (async (service, args) => await ((ICustomTestService<CustomTestType>)service).ReturnGenericTask_MethodWithMixedTypeParametersAsync((CustomTestType)args[0]).ConfigureAwait(false));
                Func<object, object[], Task<CustomTestType>> ReturnGenericTask_MethodWithClassTypeParameters = ((service, args) => ((ICustomTestService<CustomTestType>)service).ReturnGenericTask_MethodWithClassTypeParameters((CustomTestType)args[0]));
                Func<object, object[], Task<CustomTestType>> ReturnGenericTask_MethodReturnsNull = ((service, args) => ((ICustomTestService<CustomTestType>)service).ReturnGenericTask_MethodReturnsNullGenericTask());

                Func<object, object[], Task> ReturnTask_MethodWithMixedTypeParametersAsync = (async (service, args) => await ((ICustomTestService<CustomTestType>)service).ReturnTask_MethodWithMixedTypeParametersAsync((CustomTestType)args[0]).ConfigureAwait(false));
                Func<object, object[], Task> ReturnTask_MethodWithClassTypeParameters = ((service, args) => ((ICustomTestService<CustomTestType>)service).ReturnTask_MethodWithClassTypeParameters((CustomTestType)args[0]));

                return new[]
                {
                    new object[] { nameof(ReturnGenericTask_MethodWithMixedTypeParametersAsync), Container.Resolve<ICustomTestService<CustomTestType>>(), new object[] { new CustomTestType(new TestType()) }, ReturnGenericTask_MethodWithMixedTypeParametersAsync, new CustomTestType(new TestType()), true, true },
                    new object[] { nameof(ReturnGenericTask_MethodWithMixedTypeParametersAsync), Container.Resolve<ICustomTestService<CustomTestType>>(), new object[] { null }, ReturnGenericTask_MethodWithMixedTypeParametersAsync, null, true, true },
                    new object[] { nameof(ReturnGenericTask_MethodWithClassTypeParameters), Container.Resolve<ICustomTestService<CustomTestType>>(), new object[] { new CustomTestType(new TestType()) }, ReturnGenericTask_MethodWithClassTypeParameters, new CustomTestType(new TestType()), true, false },
                    new object[] { nameof(ReturnGenericTask_MethodWithClassTypeParameters), Container.Resolve<ICustomTestService<CustomTestType>>(), new object[] { null }, ReturnGenericTask_MethodWithClassTypeParameters, null, true, false },

                    new object[] { nameof(ReturnTask_MethodWithMixedTypeParametersAsync), Container.Resolve<ICustomTestService<CustomTestType>>(), new object[] { new CustomTestType(new TestType()) }, ReturnTask_MethodWithMixedTypeParametersAsync, null, false, true },
                    new object[] { nameof(ReturnTask_MethodWithClassTypeParameters), Container.Resolve<ICustomTestService<CustomTestType>>(), new object[] { new CustomTestType(new TestType()) }, ReturnTask_MethodWithClassTypeParameters, null, false, false }
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
                Func<object, object[], ValueTask<CustomTestType>> ReturnGenericValueTask_MethodWithMixedTypeParametersAsync = (async (service, args) => await ((ICustomTestService<CustomTestType>)service).ReturnGenericValueTask_MethodWithMixedTypeParametersAsync((CustomTestType)args[0]).ConfigureAwait(false));
                Func<object, object[], ValueTask<CustomTestType>> ReturnGenericValueTask_MethodWithClassTypeParameters = ((service, args) => ((ICustomTestService<CustomTestType>)service).ReturnGenericValueTask_MethodWithClassTypeParameters((CustomTestType)args[0]));

                Func<object, object[], ValueTask> ReturnValueTask_MethodWithMixedTypeParametersAsync = (async (service, args) => await ((ICustomTestService<CustomTestType>)service).ReturnValueTask_MethodWithMixedTypeParametersAsync((CustomTestType)args[0]).ConfigureAwait(false));
                Func<object, object[], ValueTask> ReturnValueTask_MethodWithClassTypeParameters = ((service, args) => ((ICustomTestService<CustomTestType>)service).ReturnValueTask_MethodWithClassTypeParameters((CustomTestType)args[0]));

                return new[]
                {
                    new object[] { nameof(ReturnGenericValueTask_MethodWithMixedTypeParametersAsync), Container.Resolve<ICustomTestService<CustomTestType>>(), new object[] { new CustomTestType(new TestType()) }, ReturnGenericValueTask_MethodWithMixedTypeParametersAsync, new CustomTestType(new TestType()), true, true },
                    new object[] { nameof(ReturnGenericValueTask_MethodWithClassTypeParameters), Container.Resolve<ICustomTestService<CustomTestType>>(), new object[] { new CustomTestType(new TestType()) }, ReturnGenericValueTask_MethodWithClassTypeParameters, new CustomTestType(new TestType()), true, false },
                    new object[] { nameof(ReturnGenericValueTask_MethodWithMixedTypeParametersAsync), Container.Resolve<ICustomTestService<CustomTestType>>(), new object[] { null }, ReturnGenericValueTask_MethodWithMixedTypeParametersAsync, null, true, true },
                    new object[] { nameof(ReturnGenericValueTask_MethodWithClassTypeParameters), Container.Resolve<ICustomTestService<CustomTestType>>(), new object[] { null }, ReturnGenericValueTask_MethodWithClassTypeParameters, null, true, false },

                    new object[] { nameof(ReturnValueTask_MethodWithMixedTypeParametersAsync), Container.Resolve<ICustomTestService<CustomTestType>>(), new object[] { new CustomTestType(new TestType()) }, ReturnValueTask_MethodWithMixedTypeParametersAsync, null, false, true },
                    new object[] { nameof(ReturnValueTask_MethodWithClassTypeParameters), Container.Resolve<ICustomTestService<CustomTestType>>(), new object[] { new CustomTestType(new TestType()) }, ReturnValueTask_MethodWithClassTypeParameters, null, false, false },
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

        [Test]
        public void Intercept_TargetMethodException_UnwrapsAndThrowsException()
        {
            var service = Container.Resolve<ICustomTestService<CustomTestType>>();

            var ex1 = Assert.Throws<NotImplementedException>(() => service.ThrowsException());
            TestContext.WriteLine(ex1);
            Assert.AreEqual(@"The method or operation is not implemented.", ex1.Message);

            var ex2 = Assert.Throws<AggregateException>(() => service.ThrowsExceptionAsync().Wait());
            TestContext.WriteLine(ex2);
            Assert.AreEqual(@"One or more errors occurred. (The method or operation is not implemented.)", ex2.Message);
        }

        static object[] NullTask_Cases
        {
            get
            {
                Func<object, object[], Task<CustomTestType>> ReturnGenericTask_MethodReturnsNullGenericTask = ((service, args) => ((ICustomTestService<CustomTestType>)service).ReturnGenericTask_MethodReturnsNullGenericTask());
                Func<object, object[], Task> ReturnTask_MethodReturnsNullTask = ((service, args) => ((ICustomTestService<CustomTestType>)service).ReturnTask_MethodReturnsNullTask());

                return new[]
                {
                    new object[] { nameof(ReturnGenericTask_MethodReturnsNullGenericTask), Container.Resolve<ICustomTestService<CustomTestType>>(), new object[] { null }, ReturnGenericTask_MethodReturnsNullGenericTask,
 @"One or more errors occurred. (Target method result should be a generic task but is null.
Adapter method: {MethodInfo: ReturnGenericTask_MethodReturnsNullGenericTask, Parameters: {ParameterInfo[0]: []}, ReturnType: {Type: System.Threading.Tasks.Task`1[[com.github.akovac35.AdapterInterceptor.Tests.TestTypes.CustomTestType, com.github.akovac35.AdapterInterceptor.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]}, DeclaringType: {Type: com.github.akovac35.AdapterInterceptor.Tests.TestServices.ICustomTestService`1[[com.github.akovac35.AdapterInterceptor.Tests.TestTypes.CustomTestType, com.github.akovac35.AdapterInterceptor.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]}}
Target method: {MethodInfo: ReturnGenericTask_MethodReturnsNullGenericTask, Parameters: {ParameterInfo[0]: []}, ReturnType: {Type: System.Threading.Tasks.Task`1[[com.github.akovac35.AdapterInterceptor.Tests.TestTypes.TestType, com.github.akovac35.AdapterInterceptor.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]}, DeclaringType: {Type: com.github.akovac35.AdapterInterceptor.Tests.TestServices.TestService}})" },
                    new object[] { nameof(ReturnTask_MethodReturnsNullTask), Container.Resolve<ICustomTestService<CustomTestType>>(), new object[] { null }, ReturnTask_MethodReturnsNullTask,
 @"One or more errors occurred. (Target method result should be a task but is null.
Adapter method: {MethodInfo: ReturnTask_MethodReturnsNullTask, Parameters: {ParameterInfo[0]: []}, ReturnType: {Type: System.Threading.Tasks.Task}, DeclaringType: {Type: com.github.akovac35.AdapterInterceptor.Tests.TestServices.ICustomTestService`1[[com.github.akovac35.AdapterInterceptor.Tests.TestTypes.CustomTestType, com.github.akovac35.AdapterInterceptor.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]}}
Target method: {MethodInfo: ReturnTask_MethodReturnsNullTask, Parameters: {ParameterInfo[0]: []}, ReturnType: {Type: System.Threading.Tasks.Task}, DeclaringType: {Type: com.github.akovac35.AdapterInterceptor.Tests.TestServices.TestService}})" }
                };
            }
        }

        [TestCaseSource("NullTask_Cases")]
        public void Intercept_ForNullTask_ThrowsException(string testName, object service, object[] inputs, Func<object, object[], Task> function, string expectedMessage)
        {
            var exception = Assert.Throws<AggregateException>(() =>
            {
                function(service, inputs).Wait();
            });
            TestContext.WriteLine(exception);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        static object[] KnownMethod_Cases =
        {
            new object[] { typeof(ICustomTestService<CustomTestType>), nameof(TestService.ReturnVoid_MethodWithoutParameters), new Type[] { } },
            new object[] { typeof(ICustomTestService<CustomTestType>), nameof(TestService.ReturnObject_VirtualMethodWithValueTypeParameters), new Type[] { typeof(int), typeof(string) } },
            new object[] { typeof(ICustomTestService<CustomTestType>), nameof(TestService.ReturnGenericTask_MethodWithMixedTypeParametersAsync), new Type[] {typeof(CustomTestType), typeof(bool), typeof(bool), typeof(string) } }
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

            var exception = Assert.Throws<AdapterInterceptorException>(() => { MapTargetMethod(typeof(ICustomTestService<CustomTestType>), sourceMethodStub.Object, destinationTypes); });
            TestContext.WriteLine(exception);
            Assert.AreEqual(
@"Target method cannot be found. This is likely due to missing supported type pairs or because target type changed.
Target type: {Type: com.github.akovac35.AdapterInterceptor.Tests.TestServices.ICustomTestService`1[[com.github.akovac35.AdapterInterceptor.Tests.TestTypes.CustomTestType, com.github.akovac35.AdapterInterceptor.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], com.github.akovac35.AdapterInterceptor.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null}
Target method: {MethodInfo: xyz, Parameters: {ParameterInfo[0]: []}, ReturnType: , DeclaringType: }
Target method parameter types: {Type[2]: [{Type: System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e},{Type: System.String, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e}]}", exception.Message);
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
            var service = Container.ResolveNamed<ICustomTestService<CustomTestType>>("GenericCustomTestService");
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
            });
            TestContext.WriteLine(ex1);
            Assert.AreEqual("Source type mapping for type {Type: com.github.akovac35.AdapterInterceptor.Tests.TestTypes.TestType} is already defined for type {Type: com.github.akovac35.AdapterInterceptor.Tests.TestTypes.CustomTestType} and can't be added for type {Type: com.github.akovac35.AdapterInterceptor.Tests.TestTypes.UnknownType}. Review documentation and usage or use a less generic variant AdapterInterceptor<TTarget>.", ex1.Message);

            var ex2 = Assert.Throws<AdapterInterceptorException>(() =>
            {
                typePair.AddTypePair(typeof(CustomTestType), typeof(UnknownType));
            });
            TestContext.WriteLine(ex2.Message);
            Assert.AreEqual("Source type mapping for type {Type: com.github.akovac35.AdapterInterceptor.Tests.TestTypes.CustomTestType} is already defined for type {Type: com.github.akovac35.AdapterInterceptor.Tests.TestTypes.TestType} and can't be added for type {Type: com.github.akovac35.AdapterInterceptor.Tests.TestTypes.UnknownType}. Review documentation and usage or use a less generic variant AdapterInterceptor<TTarget>.", ex2.Message);
        }

        static object[] InvalidReturnTypesCombination_Cases
        {
            get
            {
                return new[]
                {
new object[] { InvocationTypes.Sync,typeof(void), typeof(int),
@"Adapter and target method return types should match if either is void.
Adapter method: {MethodInfo: , Parameters: {ParameterInfo[0]: []}, ReturnType: {Type: System.Void, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e}, DeclaringType: }
Target method: {MethodInfo: , Parameters: {ParameterInfo[0]: []}, ReturnType: {Type: System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e}, DeclaringType: }" },
new object[] { InvocationTypes.Task, typeof(Task<int>), typeof(Task),
@"Adapter and target method return types should match if target return type is Task.
Adapter method: {MethodInfo: , Parameters: {ParameterInfo[0]: []}, ReturnType: {Type: System.Threading.Tasks.Task`1[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e}, DeclaringType: }
Target method: {MethodInfo: , Parameters: {ParameterInfo[0]: []}, ReturnType: {Type: System.Threading.Tasks.Task, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e}, DeclaringType: }" },
new object[] { InvocationTypes.GenericTask, typeof(Task), typeof(Task<int>),
@"Adapter and target method return types should both be a generic Task.
Adapter method: {MethodInfo: , Parameters: {ParameterInfo[0]: []}, ReturnType: {Type: System.Threading.Tasks.Task, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e}, DeclaringType: }
Target method: {MethodInfo: , Parameters: {ParameterInfo[0]: []}, ReturnType: {Type: System.Threading.Tasks.Task`1[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e}, DeclaringType: }" },
new object[] { InvocationTypes.GenericValueTask, typeof(ValueTask), typeof(ValueTask<int>),
@"Adapter and target method return types should both be a generic ValueTask.
Adapter method: {MethodInfo: , Parameters: {ParameterInfo[0]: []}, ReturnType: {Type: System.Threading.Tasks.ValueTask, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e}, DeclaringType: }
Target method: {MethodInfo: , Parameters: {ParameterInfo[0]: []}, ReturnType: {Type: System.Threading.Tasks.ValueTask`1[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e}, DeclaringType: }" },
new object[] { InvocationTypes.ValueTask, typeof(ValueTask<int>), typeof(ValueTask),
@"Adapter and target method return types should match if target return type is ValueTask.
Adapter method: {MethodInfo: , Parameters: {ParameterInfo[0]: []}, ReturnType: {Type: System.Threading.Tasks.ValueTask`1[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]], System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e}, DeclaringType: }
Target method: {MethodInfo: , Parameters: {ParameterInfo[0]: []}, ReturnType: {Type: System.Threading.Tasks.ValueTask, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e}, DeclaringType: }" }
                };
            }
        }

        [TestCaseSource("InvalidReturnTypesCombination_Cases")]
        public void AssertReturnTypeCombination_InvalidReturnTypesCombination_ThrowsException(InvocationTypes invocationType, Type adapterReturnType, Type targetReturnType, string expectedErrorMessage)
        {
            var adapterMethodStub = new Mock<MethodInfo>();
            adapterMethodStub.Setup(instance => instance.ReturnType).Returns(adapterReturnType);

            var targetMethodStub = new Mock<MethodInfo>();
            targetMethodStub.Setup(instance => instance.ReturnType).Returns(targetReturnType);

            var ex = Assert.Throws<AdapterInterceptorException>(() => AssertReturnTypeCombination(adapterMethodStub.Object, targetMethodStub.Object, invocationType));
            TestContext.WriteLine(ex);
            Assert.AreEqual(expectedErrorMessage, ex.Message);
        }

        [Test]
        public async Task AdapterInterceptor_PassingExecutionContext_WorksAsync()
        {
            var customService = Container.ResolveNamed<ICustomTestService<CustomTestType>>("GenericCustomTestService");
            var interceptors = (IInterceptor[])customService.GetType().GetField("__interceptors", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(customService);
            var testService = (TestService)interceptors[0].GetType().GetProperty("Target", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(interceptors[0]);

            int testVal = 10;
            testService.AsyncInvocationContext.Value = testVal;
            KeyValuePair<int, int> result = await customService.ReturnTask_MedthodUsingExecutionContext().ConfigureAwait(false);

            // The following line can be uncommented for a more rigorous test
            // Assert.AreNotEqual(Thread.CurrentThread.ManagedThreadId, result.Key, "The tested method should be executed on a separate thread, which may occasionally not happen - please repeat this test.");
            Assert.AreEqual(testVal, result.Value);
        }
    }
}
