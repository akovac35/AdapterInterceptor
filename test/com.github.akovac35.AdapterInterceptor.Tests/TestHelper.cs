// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using Autofac;
using AutoMapper;
using Castle.DynamicProxy;
using com.github.akovac35.AdapterInterceptor.Helper;
using com.github.akovac35.AdapterInterceptor.Misc;
using com.github.akovac35.AdapterInterceptor.Tests.TestServices;
using com.github.akovac35.AdapterInterceptor.Tests.TestTypes;
using com.github.akovac35.Logging.Testing;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.github.akovac35.AdapterInterceptor.Tests
{
    public static class TestHelper
    {
        public static object Map(object source, Type sourceType, Type destinationType)
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.CreateMap(sourceType, destinationType));
            var mapper = mapperConfiguration.CreateMapper();
            return mapper.Map(source, sourceType, destinationType);
        }

        public static Dictionary<Type, Type> InitializeSupportedPairsFromMapper(this IMapper mapper, bool addCollectionVariants = true, bool addReverseVariants = false)
        {
            var maps = mapper.ConfigurationProvider.GetAllTypeMaps();
            var supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();

            foreach (var item in maps)
            {
                supportedTypePairs.AddTypePair(item.SourceType, item.DestinationType, addCollectionVariants: addCollectionVariants, addReverseVariants: addReverseVariants);
            }

            return supportedTypePairs;
        }

        public static List<TOfItem> AsList<TOfItem>(this TOfItem item)
        {
            var list = new List<TOfItem>();
            list.Add(item);
            return list;
        }

        public static IList<TOfItem> AsIList<TOfItem>(this TOfItem item)
        {
            var list = new List<TOfItem>();
            list.Add(item);
            return list as IList<TOfItem>;
        }

        public static TOfItem[] AsArray<TOfItem>(this TOfItem item)
        {
            return new TOfItem[] { item };
        }

        public static ICollection<TOfItem> AsICollection<TOfItem>(this TOfItem item)
        {
            var list = new List<TOfItem>();
            list.Add(item);
            return list as ICollection<TOfItem>;
        }

        public static ILoggerFactory LoggerFactory
        {
            get
            {
                var sink = new TestSink();
                sink.MessageLogged += Sink_MessageLogged;
                sink.ScopeStarted += Sink_ScopeStarted;
                return new TestLoggerFactory(sink);
            }
        }

        private static void Sink_ScopeStarted(ScopeContext obj)
        {
            TestContext.WriteLine(obj);
        }

        private static void Sink_MessageLogged(WriteContext obj)
        {
            TestContext.WriteLine(obj);
        }

        public static IContainer CreateContainerBuilder()
        {
            var builder = new ContainerBuilder();

            // Services
            builder.RegisterType<TestService>();

            // AutoMapper
            builder.RegisterInstance(new MapperConfiguration(cfg =>
            {
                // Just add a few mappings which will not actually be used
                IEnumerable<Type> builtInPrimitiveTypes = typeof(int).Assembly.GetTypes().Where(t => t.IsPrimitive);
                var supportedTypePairs = from source in builtInPrimitiveTypes
                                         from destination in builtInPrimitiveTypes
                                         where source != destination
                                         select new TypePair(source, destination);
                supportedTypePairs = supportedTypePairs.GroupBy(item => item.SourceType).Select(group => group.First());

                foreach (var item in supportedTypePairs)
                {
                    cfg.CreateMap(item.SourceType, item.DestinationType);
                }
                cfg.CreateMap<CustomTestType, TestType>().ReverseMap();
            })).Named<MapperConfiguration>("ComplexMapperConfig");
            builder.RegisterInstance(new MapperConfiguration(cfg => cfg.CreateMap<CustomTestType, TestType>().ReverseMap())).Named<MapperConfiguration>("SimpleMapperConfig");
            // Default
            builder.RegisterInstance(new MapperConfiguration(cfg => cfg.CreateMap<CustomTestType, TestType>().ReverseMap())).AsSelf();

            builder.Register(ctx => ctx.ResolveNamed<MapperConfiguration>("ComplexMapperConfig").CreateMapper()).Named<IMapper>("ComplexMapper");
            builder.Register(ctx => ctx.ResolveNamed<MapperConfiguration>("SimpleMapperConfig").CreateMapper()).Named<IMapper>("SimpleMapper");
            // Default
            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>();

            // IAdapterMapper
            builder.Register(ctx =>
            {
                var mapper = ctx.ResolveNamed<IMapper>("ComplexMapper");
                var adapterMapper = new DefaultAdapterMapper(mapper);
                return adapterMapper;
            }).Named<DefaultAdapterMapper>("ComplexDefaultAdapterMapperForBenchmarks");
            builder.Register(ctx =>
            {
                var mapper = ctx.ResolveNamed<IMapper>("SimpleMapper");
                var adapterMapper = new DefaultAdapterMapper(mapper);
                return adapterMapper;
            }).Named<DefaultAdapterMapper>("SimpleDefaultAdapterMapperForBenchmarks"); ;
            // Default
            builder.Register(ctx =>
            {
                var mapper = ctx.Resolve<IMapper>();
                var adapterMapper = new DefaultAdapterMapper(mapper);
                return adapterMapper;
            }).As<IAdapterMapper>().As<DefaultAdapterMapper>();


            // AdapterInterceptor
            builder.Register(ctx =>
            {
                var mapper = ctx.ResolveNamed<IMapper>("ComplexMapper");
                var adapterMapper = ctx.ResolveNamed<DefaultAdapterMapper>("ComplexDefaultAdapterMapperForBenchmarks");
                var target = ctx.Resolve<TestService>();
                // Must not use logging for benchmark
                return new AdapterInterceptor<TestService>(target, adapterMapper, mapper.InitializeSupportedPairsFromMapper(), new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>());
            }).Named<AdapterInterceptor<TestService>>("ComplexAdapterInterceptorForBenchmarks");
            builder.Register(ctx =>
            {
                var mapper = ctx.ResolveNamed<IMapper>("SimpleMapper");
                var adapterMapper = ctx.ResolveNamed<DefaultAdapterMapper>("SimpleDefaultAdapterMapperForBenchmarks");
                var target = ctx.Resolve<TestService>();
                // Must not use logging for benchmark
                return new AdapterInterceptor<TestService>(target, adapterMapper, mapper.InitializeSupportedPairsFromMapper(), new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>());
            }).Named<AdapterInterceptor<TestService>>("SimpleAdapterInterceptorForBenchmarks");
            // Default
            builder.Register(ctx =>
            {
                var mapper = ctx.Resolve<IMapper>();
                var adapterMapper = ctx.Resolve<DefaultAdapterMapper>();
                var target = ctx.Resolve<TestService>();
                return new AdapterInterceptor<TestService>(target, adapterMapper, mapper.InitializeSupportedPairsFromMapper(), new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>(), TestHelper.LoggerFactory);
            }).AsSelf();
            // Generics
            builder.Register(ctx =>
            {
                var adapterMapper = ctx.Resolve<IAdapterMapper>();
                var target = ctx.Resolve<TestService>();
                // Must not use logging for concurrency test
                return new AdapterInterceptor<TestService, TestType, CustomTestType>(target, adapterMapper);
            }).Named<AdapterInterceptor<TestService>>("GenericAdapterInterceptor");

            // Proxies
            builder.RegisterInstance(new ProxyGenerator(true)).AsSelf();
            builder.Register(ctx =>
            {
                var adapterInterceptor = ctx.ResolveNamed<AdapterInterceptor<TestService>>("ComplexAdapterInterceptorForBenchmarks");
                var proxyGen = ctx.Resolve<ProxyGenerator>();
                return proxyGen.CreateInterfaceProxyWithoutTarget<ICustomTestService<CustomTestType>>(adapterInterceptor);
            }).Named<ICustomTestService<CustomTestType>>("ComplexCustomTestServiceForBenchmarks");
            builder.Register(ctx =>
            {
                var adapterInterceptor = ctx.ResolveNamed<AdapterInterceptor<TestService>>("SimpleAdapterInterceptorForBenchmarks");
                var proxyGen = ctx.Resolve<ProxyGenerator>();
                return proxyGen.CreateInterfaceProxyWithoutTarget<ICustomTestService<CustomTestType>>(adapterInterceptor);
            }).Named<ICustomTestService<CustomTestType>>("SimpleCustomTestServiceForBenchmarks");
            // Default
            builder.Register(ctx =>
            {
                var adapterInterceptor = ctx.Resolve<AdapterInterceptor<TestService>>();
                var proxyGen = ctx.Resolve<ProxyGenerator>();
                return proxyGen.CreateInterfaceProxyWithoutTarget<ICustomTestService<CustomTestType>>(adapterInterceptor);
            }).As<ICustomTestService<CustomTestType>>();
            // Generic
            builder.Register(ctx =>
            {
                var adapterInterceptor = ctx.ResolveNamed<AdapterInterceptor<TestService>>("GenericAdapterInterceptor");
                var proxyGen = ctx.Resolve<ProxyGenerator>();
                return proxyGen.CreateInterfaceProxyWithoutTarget<ICustomTestService<CustomTestType>>(adapterInterceptor);
            }).Named<ICustomTestService<CustomTestType>>("GenericCustomTestService");

            return builder.Build();
        }
    }
}
