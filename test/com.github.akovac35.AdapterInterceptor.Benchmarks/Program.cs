using BenchmarkDotNet.Running;
using System;

namespace com.github.akovac35.AdapterInterceptor.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
