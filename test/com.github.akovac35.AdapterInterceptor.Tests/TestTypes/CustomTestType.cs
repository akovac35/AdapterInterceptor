// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

namespace com.github.akovac35.AdapterInterceptor.Tests.TestTypes
{
    public class CustomTestType
    {
        public CustomTestType(TestType source)
        {
            Name = source.Name;
            Value = source.Value;
        }

        public CustomTestType()
        {
        }

        public string Name { get; set; }

        public int Value { get; set; }
    }
}
