// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using com.github.akovac35.AdapterInterceptor.Tests.TestTypes;
using System;

namespace com.github.akovac35.AdapterInterceptor.Tests.TestServices
{
    public class TestServiceBase
    {
        public void ReturnVoid_MethodWithoutParameters()
        {
        }

        public TestType ReturnTestType_MethodWithClassParameters(TestType a)
        {
            return new TestType();
        }

        public virtual string ReturnObject_VirtualMethodWithValueTypeParameters(int a, string b)
        {
            throw new NotImplementedException();
        }
    }
}
