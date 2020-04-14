﻿// Author: Aleksander Kovač

using Autofac.Extras.DynamicProxy;
using com.github.akovac35.AdapterInterceptor.Tests.TestTypes;
using System.Threading.Tasks;

namespace com.github.akovac35.AdapterInterceptor.Tests.TestServices
{
    public interface ICustomTestService
    {
        void SimpleMethod();

        CustomTestType MethodUsingType(CustomTestType a);

        object VirtualMethod(int a, string b);

        Task<CustomTestType> TestMethodAsync(CustomTestType a);

        Task<CustomTestType> TestMethodReturningTask(CustomTestType a);
    }
}
