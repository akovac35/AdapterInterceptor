// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using AutoMapper;
using com.github.akovac35.AdapterInterceptor.Default;
using com.github.akovac35.AdapterInterceptor.Tests.TestTypes;
using NUnit.Framework;
using System.Collections.Generic;

namespace com.github.akovac35.AdapterInterceptor.Tests
{
    [TestFixture]
    public class DefaultAdapterMapperTest
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
        public void Map_ForSimpleTypes_Works()
        {
            var source = new TestType();
            var sourceType = typeof(TestType);
            var destinationType = typeof(CustomTestType);

            var mapperConfiguration = new MapperConfiguration(cfg => cfg.CreateMap(sourceType, destinationType));
            var mapper = mapperConfiguration.CreateMapper();
            var adapterMapper = new DefaultAdapterMapper(mapper);

            var destination = adapterMapper.Map(source, sourceType, destinationType) as CustomTestType;
            Assert.IsNotNull(destination);
            Assert.IsTrue(source.Name == destination.Name);
            Assert.IsTrue(source.Value == destination.Value);
        }

        [Test]
        public void Map_ForCollections_Works()
        {
            var source = new List<TestType>();
            source.Add(new TestType());
            var sourceType = typeof(TestType);
            var sourceTypeCollection = typeof(List<TestType>);
            var destinationType = typeof(CustomTestType);
            var destinationTypeCollection = typeof(List<CustomTestType>);

            var mapperConfiguration = new MapperConfiguration(cfg => cfg.CreateMap(sourceType, destinationType));
            var mapper = mapperConfiguration.CreateMapper();
            var adapterMapper = new DefaultAdapterMapper(mapper);

            var destination = adapterMapper.Map(source, sourceTypeCollection, destinationTypeCollection) as List<CustomTestType>;
            Assert.IsNotNull(destination);
            Assert.IsTrue(destination.Count == 1);
            Assert.IsTrue(source[0].Name == destination[0].Name);
            Assert.IsTrue(source[0].Value == destination[0].Value);
        }
    }
}
