// Author: Aleksander Kovač

using Autofac;
using AutoMapper;
using Castle.DynamicProxy;
using com.github.akovac35.AdapterInterceptor.Tests.TestServices;
using com.github.akovac35.AdapterInterceptor.Tests.TestTypes;
using DeepEqual.Syntax;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace com.github.akovac35.AdapterInterceptor.Tests
{
    [TestFixture]
    public class AdapterInterceptorTest
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [SetUp]
        public void SetUp()
        {
        }

        static IContainer Container { get; set; } = CreateContainerBuilder();

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

                return new[]
                {
                    new object[] { Container.Resolve<ICustomTestService>(), new object[] { new CustomTestType(new TestType()) }, ICustomTestServiceTestMethodAsync, new CustomTestType(new TestType()) }

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

        static IContainer CreateContainerBuilder()
        {
            var builder = new ContainerBuilder();

            // Services
            builder.RegisterType<TestService>();

            // AutoMapper
            builder.RegisterInstance(new MapperConfiguration(cfg => cfg.CreateMap<CustomTestType, TestType>().ReverseMap()));
            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>();

            // AdapterInterceptor
            builder.Register(ctx =>
            {
                var mapper = ctx.Resolve<IMapper>();
                var supportedTypes = AdapterHelper.CreateList().AddPair<CustomTestType, TestType>(addReverseVariants: true);
                var adapterMapper = new DefaultAdapterMapper(mapper, supportedTypes, TestHelper.LoggerFactory);
                var target = ctx.Resolve<TestService>();
                return new AdapterInterceptor<TestService>(target, adapterMapper, TestHelper.LoggerFactory);
            }).As<AdapterInterceptor<TestService>>();
            
            // Proxies
            builder.RegisterInstance(new ProxyGenerator(true));
            builder.Register(ctx =>
            {
                var adapterInterceptor = ctx.Resolve<AdapterInterceptor<TestService>>();
                var proxyGen = ctx.Resolve<ProxyGenerator>();
                return proxyGen.CreateInterfaceProxyWithoutTarget<ICustomTestService>(adapterInterceptor);
            }).As<ICustomTestService>();

            return builder.Build();
        }
    }
}
