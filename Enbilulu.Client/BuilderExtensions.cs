using System;
using Microsoft.AspNetCore.Builder;
namespace Enbilulu;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseEnbilulu(this IApplicationBuilder builder, Action<EnbiluluBuilder> configure)
    {

        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        EnbiluluBuilder enbiluluBuilder = builder.ApplicationServices.GetService(typeof(EnbiluluBuilder)) as EnbiluluBuilder;

        if (enbiluluBuilder == null)
        {
            throw new ArgumentNullException(nameof(enbiluluBuilder));
        }

        configure(enbiluluBuilder);

        return builder;

    }
}