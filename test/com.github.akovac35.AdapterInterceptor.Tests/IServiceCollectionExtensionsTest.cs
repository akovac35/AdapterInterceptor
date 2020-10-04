// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using AutoMapper;
using com.github.akovac35.AdapterInterceptor.DependencyInjection;
using com.github.akovac35.AdapterInterceptor.Tests.TestServices;
using com.github.akovac35.AdapterInterceptor.Tests.TestTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NUnit.Framework;

namespace com.github.akovac35.AdapterInterceptor.Tests
{
    [TestFixture]
    public class IServiceCollectionExtensionsTest
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
        public void AddProxyImitator_Works()
        {
            var services = new ServiceCollection();
            services.TryAddScoped<TestService>();
            services.AddProxyImitator<ICustomTestService<TestType>, TestService>(targetFact => targetFact.GetRequiredService<TestService>(), ServiceLifetime.Scoped);
            var provider = services.BuildServiceProvider();

            var service = provider.GetRequiredService<ICustomTestService<TestType>>();
            var result = service.MethodUsingOneArgument(new TestType());

            Assert.IsNotNull(result);
        }

        [Test]
        public void AddAdapter_Works()
        {
            var services = new ServiceCollection();
            services.TryAddScoped<TestService>();
            services.TryAddSingleton<MapperConfiguration>(fact => new MapperConfiguration(cfg => cfg.CreateMap<CustomTestType, TestType>().ReverseMap()));
            services.TryAddScoped<DefaultAdapterMapper>(fact =>
            {
                var mapperConfig = fact.GetRequiredService<MapperConfiguration>();
                var mapper = new DefaultAdapterMapper(mapperConfig.CreateMapper());
                return mapper;
            });
            services.AddAdapter<ICustomTestService<CustomTestType>, TestService, CustomTestType, TestType>(targetFact => targetFact.GetRequiredService<TestService>(), adapterMapperFact => adapterMapperFact.GetRequiredService<DefaultAdapterMapper>(), ServiceLifetime.Scoped);
            var provider = services.BuildServiceProvider();

            var service = provider.GetRequiredService<ICustomTestService<CustomTestType>>();
            var result = service.MethodUsingOneArgument(new CustomTestType());

            Assert.IsNotNull(result);
        }
    }
}
