using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

public class EnbiluluBuilder
{
    Enbilulu.Client _client;
    IConfiguration _config;
    public EnbiluluBuilder(Enbilulu.Client client, IConfiguration config)
    {
        _client = client;
        _config = config;
    }

    public EnbiluluBuilder UseConnectionString(string connectionString)
    {
        _client.Endpoint = new Uri(connectionString);
        return this;
    }

    public EnbiluluBuilder UseConnectionStringName(string connectionStringName)
    {
        return UseConnectionString(_config.GetConnectionString(connectionStringName));
    }

    public EnbiluluBuilder UseStream(string streamName)
    {
        _client.CreateStream(streamName);
        return this;
    }
}

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