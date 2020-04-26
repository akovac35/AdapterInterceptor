// License:
// Apache License Version 2.0, January 2004

// Authors:
//   Aleksander Kovač

using System;

namespace com.github.akovac35.AdapterInterceptor
{
    public class AdapterInterceptorException : Exception
    {
        public AdapterInterceptorException()
        {
        }

        public AdapterInterceptorException(string message)
            : base(message)
        {
        }

        public AdapterInterceptorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
