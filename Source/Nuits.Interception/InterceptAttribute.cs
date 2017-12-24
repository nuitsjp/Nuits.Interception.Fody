﻿using System;

namespace Nuits.Interception
{
    public class InterceptAttribute : Attribute
    {
        public Type[] InterceptorTypes { get; }

        public InterceptAttribute(params Type[] interceptorTypes)
        {
            InterceptorTypes = interceptorTypes;
        }
    }
}
