using System;
using System.Collections.Generic;
using System.Text;
using Nuits.Interception;

namespace InterceptionApp.Models
{
    public class Calculator
    {
        [Intercept(typeof(LoggingInterceptor))]
        public int Add(int left, int right)
        {
            return left + right;
        }
    }
}
