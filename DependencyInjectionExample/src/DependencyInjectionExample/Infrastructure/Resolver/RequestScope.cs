namespace DependencyInjectionExample.Infrastructure.Resolver
{
    using Smart.Resolver;
    using Smart.Resolver.Scopes;

    public class RequestScope : IScope
    {
        public IScopeStorage GetStorage(IKernel kernel)
        {
            return kernel.Components.Get<RequestScopeStorage>();
        }
    }
}
