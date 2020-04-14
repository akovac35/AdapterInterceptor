// Author: Aleksander Kovač

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

        public AdapterInterceptorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
