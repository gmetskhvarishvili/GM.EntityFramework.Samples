using System.Reflection;
using GM.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace GM.EntityFramework.Sample.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddGMMediator(Assembly.GetExecutingAssembly());
    }
}