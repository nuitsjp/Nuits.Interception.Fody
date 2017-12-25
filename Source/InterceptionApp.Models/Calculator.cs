using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nuits.Interception;

namespace InterceptionApp.Models
{
    public class Calculator
    {
        [Intercept(typeof(LoggingInterceptor))]
        public async Task<int> Add(int left, int right)
        {
            using (var transactionContext = new TransactionContext())
            {
                await transactionContext.BeginTransaction();
            }
            return left + right;
        }
    }
}
