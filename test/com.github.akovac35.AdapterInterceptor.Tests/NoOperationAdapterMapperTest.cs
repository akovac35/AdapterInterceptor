// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using com.github.akovac35.AdapterInterceptor.Tests.TestTypes;
using NUnit.Framework;
using System.Collections.Generic;

namespace com.github.akovac35.AdapterInterceptor.Tests
{
    [TestFixture]
    public class NoOperationAdapterMapperTest
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
            var destinationType = typeof(TestType);

            var adapterMapper = new NoOperationAdapterMapper();

            var destination = adapterMapper.Map(source, sourceType, destinationType) as TestType;
            Assert.IsNotNull(destination);
            Assert.IsTrue(source.Name == destination.Name);
            Assert.IsTrue(source.Value == destination.Value);
        }

        [Test]
        public void Map_ForCollections_Works()
        {
            var source = new List<TestType>();
            source.Add(new TestType());
            var sourceTypeCollection = typeof(List<TestType>);
            var destinationTypeCollection = typeof(List<TestType>);

            var adapterMapper = new NoOperationAdapterMapper();

            var destination = adapterMapper.Map(source, sourceTypeCollection, destinationTypeCollection) as List<TestType>;
            Assert.IsNotNull(destination);
            Assert.IsTrue(destination.Count == 1);
            Assert.IsTrue(source[0].Name == destination[0].Name);
            Assert.IsTrue(source[0].Value == destination[0].Value);
        }
    }
}
