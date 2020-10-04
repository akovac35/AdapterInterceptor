// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;

namespace com.github.akovac35.AdapterInterceptor.Tests
{
    [TestFixture]
    public class CodeGen
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [SetUp]
        public void SetUp()
        {
        }

        private readonly ILogger _logger = TestHelper.LoggerFactory.CreateLogger<CodeGen>();

        [Test]
        public void GenerateAdapterInterceptorGenericConstructors()
        {
            string constructors = "";

            for (int i = 1; i <= 16; i++)
            {
                string description = "";
                string type = "";
                string supportedPairs = "";
                for (int j = 2; j <= i; j++)
                {
                    description += $@"    /// <typeparam name=""TSource{j}"">Source type {j}. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>" + Environment.NewLine;
                    description += $@"    /// <typeparam name=""TDestination{j}"">Destination type {j}.</typeparam>" + Environment.NewLine;

                    type += $", TSource{j}, TDestination{j}";

                    supportedPairs += $"            _supportedTypePairs.AddTypePair(typeof(TSource{j}), typeof(TDestination{j}), addCollectionVariants: true, addReverseVariants: true);" + Environment.NewLine;
                }
                description = description.TrimEnd();
                supportedPairs = supportedPairs.TrimEnd();

                string constructor = $@"
    /// <summary>
    /// Used for converting an interface into another interface through type mapping. Supports synchronous/asynchronous methods and out/ref arguments.
    /// Implicitly supports reverse mapping as well as mappings and reverse mappings of common collection variants, e.g. TSource[]/IList<TDestination>. Use a less generic variant AdapterInterceptor<TTarget> if this causes problems; for example, the following declaration will result in runtime exception because each source type must be unique and IList<CustomTestType> to TestType[] mapping is implicitly supported via reverse mapping of TestType to CustomTestType: AdapterInterceptor<TTarget, TestType, CusomTestType, IList<CustomTestType>, TestType[]>
    /// Implicitly supported collection variants are: T[], IList<T>, List<T>, IEnumerable<T>, ICollection<T>
    /// Type mapping does not implicitly account for variance/covariance or inheritance relationships, only explicit type pairs will be mapped.
    /// </summary>
    /// <typeparam name=""TTarget"">Type of target - adaptee</typeparam>
    /// <typeparam name=""TSource1"">Source type 1. Must be unique accounting for implicit reverse mapping as well as mappings and reverse mappings of common collection variants.</typeparam>
    /// <typeparam name=""TDestination1"">Destination type 1.</typeparam>
{ description}
    public class AdapterInterceptor<TTarget, TSource1, TDestination1{type}> : AdapterInterceptor<TTarget>
        where TTarget : notnull
    {{
        static AdapterInterceptor()
        {{            
            _supportedTypePairs.AddTypePair(typeof(TSource1), typeof(TDestination1), addCollectionVariants: true, addReverseVariants: true);
{supportedPairs}
        }}

        /// <summary>
        /// Initializes a new AdapterInterceptor instance.
        /// </summary>
        /// <param name=""target"">Adaptee instance.</param>
        /// <param name=""adapterMapper"">Adapter mapper instance.</param>
        /// <param name=""loggerFactory"">Logger factory instace.</param>
        public AdapterInterceptor(TTarget target, IAdapterMapper adapterMapper, ILoggerFactory? loggerFactory = null) : base(target, adapterMapper, loggerFactory)
        {{
            SupportedTypePairs = _supportedTypePairs;
            AdapterToTargetMethodDictionary = _dapterToTargetMethodDictionary;
        }}
        
        // Do note each generic type variant has its own copy
        protected static ConcurrentDictionary<MethodInfo, AdapterInvocationInformation> _dapterToTargetMethodDictionary = new ConcurrentDictionary<MethodInfo, AdapterInvocationInformation>();
        protected static Dictionary<Type, Type> _supportedTypePairs = AdapterHelper.InitializeSupportedTypePairs();
    }}";
                constructors += constructor + Environment.NewLine;
            }

            _logger.LogInformation(constructors);
        }

    }
}
