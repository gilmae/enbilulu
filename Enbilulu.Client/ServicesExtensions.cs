
using Microsoft.Extensions.DependencyInjection;

namespace Enbilulu;
public static class ServicesExtensions {
    public static void AddEnbilulu (this IServiceCollection services) {
        services.AddEnbilulu(new EnbiluluSettings());
    }

    public static void AddEnbilulu(this IServiceCollection services, EnbiluluSettings settings)
    {
        Client enbilulu = new Client(settings.ConnectionString);
        services.AddSingleton(typeof(Client), enbilulu);
        services.AddTransient(typeof(EnbiluluBuilder));
    }
}

public class EnbiluluSettings
{
    public string ConnectionString { get; set; } = default!;
}

public class EnbiluluBuilder {
    Client _client;
    public EnbiluluBuilder (Client client) {
        _client = client;
    }
    
    public EnbiluluBuilder UseStream (string streamName) {
        _client.CreateStream (streamName);
        return this;
    }
}

