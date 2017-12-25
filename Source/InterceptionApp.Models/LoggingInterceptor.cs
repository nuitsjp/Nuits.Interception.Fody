using System;
using System.Collections.Generic;
using System.Text;
using Nuits.Interception;

namespace InterceptionApp
{
    public class LoggingInterceptor : IInterceptor
    {
        public object Intercept(IInvocation invocation)
        {
            return invocation.Invoke();
        }
    }
}
