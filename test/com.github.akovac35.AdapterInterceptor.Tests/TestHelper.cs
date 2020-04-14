// Author: Aleksander Kovač

using AutoMapper;
using com.github.akovac35.Logging.Testing;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;

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
                return new TestLoggerFactory(sink, true);
            }
        }

        private static void Sink_MessageLogged(WriteContext obj)
        {
            KeyValuePair<string, object>[] loggerScope = obj.Scope as KeyValuePair<string, object>[];
            TestContext.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {obj.LogLevel.ToString()} <{obj.LoggerName}:{loggerScope?[0].Value}:{loggerScope?[2].Value}> {(obj.Exception != null ? obj.Exception.Message : obj.Message)}");
        }
    }
}
