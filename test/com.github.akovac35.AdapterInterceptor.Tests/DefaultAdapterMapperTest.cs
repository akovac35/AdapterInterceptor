// Author: Aleksander Kovač

using AutoMapper;
using com.github.akovac35.AdapterInterceptor.Tests.TestServices;
using com.github.akovac35.AdapterInterceptor.Tests.TestTypes;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;

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
            var supportedTypePairs = AdapterHelper.CreateList().AddPair(sourceType, destinationType, addCollectionVariants: false);
            var adapterMapper = new DefaultAdapterMapper(mapper, supportedTypePairs, TestHelper.LoggerFactory);

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
            var supportedTypePairs = AdapterHelper.CreateList().AddPair(sourceType, destinationType, addCollectionVariants: true);
            var adapterMapper = new DefaultAdapterMapper(mapper, supportedTypePairs, TestHelper.LoggerFactory);

            var destination = adapterMapper.Map(source, sourceTypeCollection, destinationTypeCollection) as List<CustomTestType>;
            Assert.IsNotNull(destination);
            Assert.IsTrue(destination.Count == 1);
            Assert.IsTrue(source[0].Name == destination[0].Name);
            Assert.IsTrue(source[0].Value == destination[0].Value);
        }

        static object[] KnownMethod_Cases =
        {
            new object[] { typeof(ICustomTestService), "SimpleMethod", new Type[] { } },
            new object[] { typeof(ICustomTestService), "VirtualMethod", new Type[] { typeof(int), typeof(string) } },
            new object[] { typeof(ICustomTestService), "TestMethodAsync", new Type[] {typeof(CustomTestType)} }
        };

        [TestCaseSource("KnownMethod_Cases")]
        public void MapTargetMethod_ForKnownMethod_FindsMethod(Type targetType, string sourceMethodName, Type[] destinationTypes)
        {
            var sourceMethodStub = new Mock<MethodInfo>();
            sourceMethodStub.Setup(instance => instance.Name).Returns(sourceMethodName);
            var mapperStub = new Mock<IMapper>();
            // List can be empty for this test
            var supportedTypePairs = AdapterHelper.CreateList();
            var adapterMapper = new DefaultAdapterMapper(mapperStub.Object, supportedTypePairs, TestHelper.LoggerFactory);

            var targetMethodInfo = adapterMapper.MapTargetMethod(targetType, sourceMethodStub.Object, destinationTypes);

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
            var mapperStub = new Mock<IMapper>();
            // List can be empty for this test
            var supportedTypePairs = AdapterHelper.CreateList();
            var adapterMapper = new DefaultAdapterMapper(mapperStub.Object, supportedTypePairs, TestHelper.LoggerFactory);

            var exception = Assert.Throws<AdapterInterceptorException>(() => { adapterMapper.MapTargetMethod(typeof(ICustomTestService), sourceMethodStub.Object, destinationTypes); });
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
            var mapperStub = new Mock<IMapper>();
            var supportedTypePairs = AdapterHelper.CreateList().AddPair(configSourceType, configDestinationType, addCollectionVariants: true);
            var adapterInterceptorMapperHelper = new DefaultAdapterMapper(mapperStub.Object, supportedTypePairs, TestHelper.LoggerFactory);

            var mappedInstanceDestinationTypes = adapterInterceptorMapperHelper.MapSupportedTypes(new Type[] { instanceSourceType });
            Assert.IsNotNull(mappedInstanceDestinationTypes);
            Assert.IsTrue(mappedInstanceDestinationTypes.Length == 1);
            Assert.IsTrue(instanceSourceType != mappedInstanceDestinationTypes[0]);
            Assert.IsTrue(mappedInstanceDestinationTypes[0] == instanceDestinationType);
        }

        [Test]
        public void MapSupportedTypes_ForUnknownType_FindsSourceType([Values(typeof(TestType))] Type sourceType)
        {
            var mapperStub = new Mock<IMapper>();
            // We haven't defined any supported type pairs ...
            var supportedTypePairs = AdapterHelper.CreateList();
            var adapterInterceptorMapperHelper = new DefaultAdapterMapper(mapperStub.Object, supportedTypePairs, TestHelper.LoggerFactory);

            var mappedDestinationTypes = adapterInterceptorMapperHelper.MapSupportedTypes(new Type[] { sourceType });
            Assert.IsNotNull(mappedDestinationTypes);
            Assert.IsTrue(mappedDestinationTypes.Length == 1);
            Assert.IsTrue(sourceType == mappedDestinationTypes[0]);
        }
    }
}
