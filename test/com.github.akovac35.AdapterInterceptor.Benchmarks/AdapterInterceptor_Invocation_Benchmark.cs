// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using Autofac;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using com.github.akovac35.AdapterInterceptor.Tests;
using com.github.akovac35.AdapterInterceptor.Tests.TestServices;
using com.github.akovac35.AdapterInterceptor.Tests.TestTypes;
using Perfolizer.Horology;
using System.Collections.Generic;

namespace com.github.akovac35.AdapterInterceptor.Benchmarks
{
    [Config(typeof(Config))]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    [MarkdownExporterAttribute.GitHub]
    public class AdapterInterceptor_Invocation_Benchmark
    {
        private class Config : ManualConfig
        {
            public Config()
            {
                WithSummaryStyle(new SummaryStyle(System.Globalization.CultureInfo.InvariantCulture, printUnitsInHeader: true, sizeUnit: SizeUnit.KB, TimeUnit.Microsecond, printUnitsInContent: false, printZeroValuesInContent: true));
            }
        }

        public AdapterInterceptor_Invocation_Benchmark()
        {
            _container = TestHelper.CreateContainerBuilder();

            _original = new TestService();
            _adaptedComplex = _container.ResolveNamed<ICustomTestService>("ComplexCustomTestServiceForBenchmarks");
            _adaptedSimple = _container.ResolveNamed<ICustomTestService>("SimpleCustomTestServiceForBenchmarks");
        }

        protected IContainer _container;

        protected TestService _original;

        protected ICustomTestService _adaptedComplex;

        protected ICustomTestService _adaptedSimple;

        [BenchmarkCategory("Two arguments, sync"), Benchmark(Baseline = true, Description = "Direct")]
        [ArgumentsSource(nameof(TwoArguments))]
        public UnknownType ReturnUnknownType_MethodUsingTwoArgumentsOfUnknownType_Original(UnknownType a, UnknownType b)
        {
            return _original.ReturnUnknownType_MethodUsingTwoArgumentsOfUnknownType(a, b);
        }

        [BenchmarkCategory("Five arguments, sync"), Benchmark(Baseline = true, Description = "Direct")]
        [ArgumentsSource(nameof(FiveArguments))]
        public UnknownType ReturnUnknownType_MethodUsingFiveArgumentsOfUnknownType_Original(UnknownType a, UnknownType b, UnknownType c, UnknownType d, UnknownType e)
        {
            return _original.ReturnUnknownType_MethodUsingFiveArgumentsOfUnknownType(a, b, c, d, e);
        }

        [BenchmarkCategory("Two arguments, sync"), Benchmark(Description = "Adapter, one mapping")]
        [ArgumentsSource(nameof(TwoArguments))]
        public UnknownType ReturnUnknownType_MethodUsingTwoArgumentsOfUnknownType_AdaptedSimple(UnknownType a, UnknownType b)
        {
            return _adaptedSimple.ReturnUnknownType_MethodUsingTwoArgumentsOfUnknownType(a, b);
        }

        [BenchmarkCategory("Two arguments, sync"), Benchmark(Description = "Adapter, several mappings")]
        [ArgumentsSource(nameof(TwoArguments))]
        public UnknownType ReturnUnknownType_MethodUsingTwoArgumentsOfUnknownType_AdaptedComplex(UnknownType a, UnknownType b)
        {
            return _adaptedComplex.ReturnUnknownType_MethodUsingTwoArgumentsOfUnknownType(a, b);
        }

        [BenchmarkCategory("Five arguments, sync"), Benchmark(Description = "Adapter, one mapping")]
        [ArgumentsSource(nameof(FiveArguments))]
        public UnknownType ReturnUnknownType_MethodUsingFiveArgumentsOfUnknownType_AdaptedSimple(UnknownType a, UnknownType b, UnknownType c, UnknownType d, UnknownType e)
        {
            return _adaptedSimple.ReturnUnknownType_MethodUsingFiveArgumentsOfUnknownType(a, b, c, d, e);
        }

        [BenchmarkCategory("Five arguments, sync"), Benchmark(Description = "Adapter, several mappings")]
        [ArgumentsSource(nameof(FiveArguments))]
        public UnknownType ReturnUnknownType_MethodUsingFiveArgumentsOfUnknownType_AdaptedComplex(UnknownType a, UnknownType b, UnknownType c, UnknownType d, UnknownType e)
        {
            return _adaptedComplex.ReturnUnknownType_MethodUsingFiveArgumentsOfUnknownType(a, b, c, d, e);
        }

        [BenchmarkCategory("Two arguments, async"), Benchmark(Baseline = true, Description = "Direct")]
        [ArgumentsSource(nameof(TwoArguments))]
        public UnknownType ReturnUnknownType_MethodUsingTwoArgumentsOfUnknownTypeAsync_Original(UnknownType a, UnknownType b)
        {
            return _original.ReturnUnknownType_MethodUsingTwoArgumentsOfUnknownTypeAsync(a, b).Result;
        }

        [BenchmarkCategory("Five arguments, async"), Benchmark(Baseline = true, Description = "Direct")]
        [ArgumentsSource(nameof(FiveArguments))]
        public UnknownType ReturnUnknownType_MethodUsingFiveArgumentsOfUnknownTypeAsync_Original(UnknownType a, UnknownType b, UnknownType c, UnknownType d, UnknownType e)
        {
            return _original.ReturnUnknownType_MethodUsingFiveArgumentsOfUnknownTypeAsync(a, b, c, d, e).Result;
        }

        [BenchmarkCategory("Two arguments, async"), Benchmark(Description = "Adapter, one mapping")]
        [ArgumentsSource(nameof(TwoArguments))]
        public UnknownType ReturnUnknownType_MethodUsingTwoArgumentsOfUnknownTypeAsync_AdaptedSimple(UnknownType a, UnknownType b)
        {
            return _adaptedSimple.ReturnUnknownType_MethodUsingTwoArgumentsOfUnknownTypeAsync(a, b).Result;
        }

        [BenchmarkCategory("Two arguments, async"), Benchmark(Description = "Adapter, several mappings")]
        [ArgumentsSource(nameof(TwoArguments))]
        public UnknownType ReturnUnknownType_MethodUsingTwoArgumentsOfUnknownTypeAsync_AdaptedComplex(UnknownType a, UnknownType b)
        {
            return _adaptedComplex.ReturnUnknownType_MethodUsingTwoArgumentsOfUnknownTypeAsync(a, b).Result;
        }

        [BenchmarkCategory("Five arguments, async"), Benchmark(Description = "Adapter, one mapping")]
        [ArgumentsSource(nameof(FiveArguments))]
        public UnknownType ReturnUnknownType_MethodUsingFiveArgumentsOfUnknownTypeAsync_AdaptedSimple(UnknownType a, UnknownType b, UnknownType c, UnknownType d, UnknownType e)
        {
            return _adaptedSimple.ReturnUnknownType_MethodUsingFiveArgumentsOfUnknownTypeAsync(a, b, c, d, e).Result;
        }

        [BenchmarkCategory("Five arguments, async"), Benchmark(Description = "Adapter, several mappings")]
        [ArgumentsSource(nameof(FiveArguments))]
        public UnknownType ReturnUnknownType_MethodUsingFiveArgumentsOfUnknownTypeAsync_AdaptedComplex(UnknownType a, UnknownType b, UnknownType c, UnknownType d, UnknownType e)
        {
            return _adaptedComplex.ReturnUnknownType_MethodUsingFiveArgumentsOfUnknownTypeAsync(a, b, c, d, e).Result;
        }

        public IEnumerable<object[]> TwoArguments()
        {
            yield return new object[] { new UnknownType(), new UnknownType() };
        }

        public IEnumerable<object[]> FiveArguments()
        {
            yield return new object[] { new UnknownType(), new UnknownType(), new UnknownType(), new UnknownType(), new UnknownType() };
        }
    }
}
