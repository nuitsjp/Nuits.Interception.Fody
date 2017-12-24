namespace Nuits.Interception
{
    public interface IInterceptor
    {
        object Intercept(IInvocation invocation);
    }
}