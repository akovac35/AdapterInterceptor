using Autofac;
using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using com.github.akovac35.AdapterInterceptor;
using com.github.akovac35.AdapterInterceptor.Tests;
using com.github.akovac35.AdapterInterceptor.Tests.TestTypes;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;

namespace com.github.akovac35.AdapterInterceptor.Benchmarks
{
    [MinColumn, MaxColumn]
    public class AdapterInterceptor_MapSupportedTypes_Benchmark: AdapterInterceptor
    {
        public AdapterInterceptor_MapSupportedTypes_Benchmark():base(new NullLoggerFactory())
        {
            _container = TestHelper.CreateContainerBuilder();
            _simple = _container.ResolveNamed<IMapper>("SimpleMapper").InitializeSupportedPairsFromMapper(addCollectionVariants: false);
            _complex = _container.ResolveNamed<IMapper>("ComplexMapper").InitializeSupportedPairsFromMapper(addCollectionVariants: true);
        }

        protected IContainer _container;
        protected IReadOnlyDictionary<Type, Type> _simple;
        protected IReadOnlyDictionary<Type, Type> _complex;

        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(Arguments))]
        public Type[] MapSupportedTypes_SimpleAdapterMapper(Type[] sourceTypes, string argumentName)
        {
            SupportedTypePairs = _simple;
            return MapSupportedTypes(sourceTypes);
        }

        [Benchmark]
        [ArgumentsSource(nameof(Arguments))]
        public Type[] MapSupportedTypes_ComplexAdapterMapper(Type[] sourceTypes, string argumentName)
        {
            SupportedTypePairs = _complex;
            return MapSupportedTypes(sourceTypes);
        }

        public IEnumerable<object[]> Arguments()
        {
            yield return new object[] { new Type[] { typeof(CustomTestType) }, "simpleArg" };
            yield return new object[] { new Type[] { typeof(CustomTestType), typeof(CustomTestType), typeof(CustomTestType), typeof(CustomTestType), typeof(CustomTestType) }, "complexArg" };
        }
    }
}
