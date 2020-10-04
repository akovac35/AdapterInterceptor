// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using AutoMapper;
using com.github.akovac35.AdapterInterceptor.Tests.TestServices;
using com.github.akovac35.AdapterInterceptor.Tests.TestTypes;
using NUnit.Framework;

namespace com.github.akovac35.AdapterInterceptor.Tests
{
    [TestFixture]
    public class ObjectExtensionsTest
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void GenerateProxyImitator_Works()
        {
            var service = new TestService();
            var proxyImitator = service.GenerateProxyImitator<ICustomTestService<TestType>, TestService>(target => new ProxyImitatorInterceptor<TestService>(target));
            var result = proxyImitator.MethodUsingOneArgument(new TestType());

            Assert.IsNotNull(result);

            proxyImitator = service.GenerateProxyImitator<ICustomTestService<TestType>, TestService>(target => new ProxyImitatorInterceptor<TestService>(target, TestHelper.LoggerFactory));
            result = proxyImitator.MethodUsingOneArgument(new TestType());

            Assert.IsNotNull(result);
        }

        [Test]
        public void GenerateAdapter_Works()
        {
            var service = new TestService();
            var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<CustomTestType, TestType>().ReverseMap());
            var mapper = new DefaultAdapterMapper(mapperConfig.CreateMapper());
            var adapter = service.GenerateAdapter<ICustomTestService<CustomTestType>, TestService>(target => new AdapterInterceptor<TestService, CustomTestType, TestType>(target, mapper));
            var result = adapter.MethodUsingOneArgument(new CustomTestType());

            Assert.IsNotNull(result);

            adapter = service.GenerateAdapter<ICustomTestService<CustomTestType>, TestService>(target => new AdapterInterceptor<TestService, CustomTestType, TestType>(target, mapper, TestHelper.LoggerFactory));
            result = adapter.MethodUsingOneArgument(new CustomTestType());

            Assert.IsNotNull(result);
        }
    }
}
