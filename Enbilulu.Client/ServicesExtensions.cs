using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enbilulu;
public static class ServicesExtensions
{
    public static void AddEnbilulu(this IServiceCollection services)
    {
        Enbilulu.Client enbilulu = new Enbilulu.Client();
        services.AddSingleton(typeof(Enbilulu.Client), enbilulu);
        services.AddTransient(typeof(EnbiluluBuilder));
    }
}



