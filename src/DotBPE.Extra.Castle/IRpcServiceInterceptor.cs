using System;
using Castle.DynamicProxy;

namespace DotBPE.Extra
{
    public interface IRpcServiceInterceptor
    {
        void Before(IInvocation invocation);

        void After(IInvocation invocation);

        void Exception(IInvocation invocation, Exception ex);
    }
}
