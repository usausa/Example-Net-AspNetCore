namespace DependencyInjectionExample.Infrastructure.Resolver
{
    using System;

    using Microsoft.AspNetCore.Mvc.ViewComponents;

    using Smart.Resolver;

    public class SmartResolverViewComponentActivator : IViewComponentActivator
    {
        private readonly IResolver resolver;

        public SmartResolverViewComponentActivator(IResolver resolver)
        {
            this.resolver = resolver;
        }

        public object Create(ViewComponentContext context)
        {
            return resolver.Get(context.ViewComponentDescriptor.TypeInfo.AsType());
        }

        public void Release(ViewComponentContext context, object viewComponent)
        {
            (viewComponent as IDisposable)?.Dispose();
        }
    }
}
