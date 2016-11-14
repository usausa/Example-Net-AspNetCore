namespace DependencyInjectionExample.Infrastructure.Resolver
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    using Smart.Resolver;
    using Smart.Resolver.Bindings;

    public static class IntegrationExtensions
    {
        public static void UseSmartResolverRequestScope(this IApplicationBuilder app, StandardResolver resolver)
        {
            var storage = new RequestScopeStorage(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            resolver.Configure(container => container.Register(storage));

            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                finally
                {
                    storage.Clear();
                }
            });
        }

        public static IBindingNamedWithSyntax InRequestScope(this IBindingInSyntax syntax)
        {
            return syntax.InScope(new RequestScope());
        }
    }
}
