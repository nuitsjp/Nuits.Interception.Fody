﻿using System;
using System.Collections.Generic;

namespace Nuits.Interception
{
    public abstract class Invocation : IInvocation
    {
        private int _currentIndex = 0;
        private readonly List<IInterceptor> _interceptors = new List<IInterceptor>();

        protected Invocation(Type[] interceptorTypes)
        {
            foreach (var interceptorType in interceptorTypes)
            {
                _interceptors.Add((IInterceptor)Activator.CreateInstance(interceptorType));
            }
        }

        public abstract object[] Arguments { get; }

        public object Invoke()
        {
            if (_currentIndex < _interceptors.Count)
            {
                var interceptor = _interceptors[_currentIndex++];
                return interceptor.Intercept(this);
            }
            else
            {
                return InvokeEndpoint();
            }
        }

        protected abstract object InvokeEndpoint();
    }
}