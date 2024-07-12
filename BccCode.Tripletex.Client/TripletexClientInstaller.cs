using BccCode.Tripletex.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TripltexClientInstaller
    {

        public static IServiceCollection AddTripletexClient(this IServiceCollection services)
        {
            services.AddSingleton((s) => s.GetRequiredService<IConfiguration>().GetRequiredSection("Tripletex").Get<TripletexClientOptions>());
            return services.AddTripletexClient(default(TripletexClientOptions));
        }

        public static IServiceCollection AddTripletexClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration.GetRequiredSection("Tripletex").Get<TripletexClientOptions>());
            return services.AddTripletexClient(default(TripletexClientOptions));
        }

        public static IServiceCollection AddTripletexClient(this IServiceCollection services, Action<TripletexClientOptions> options)
        {
            var opts = new TripletexClientOptions();
            if (options != null) options.Invoke(opts);
            return services.AddTripletexClient(opts);
        }

        public static IServiceCollection AddTripletexClient(this IServiceCollection services, TripletexClientOptions? options)
        {
            services.AddHttpClient("TripletexClient")
                    .AddHttpMessageHandler<TripletexRateLimitingHandler>();

            services.AddTransient<TripletexRateLimitingHandler>();

            return services.AddSingleton<ITripletexClient>(x =>
            {
                if (options == null)
                {
                    options = x.GetRequiredService<TripletexClientOptions>();
                }
                var clientFactory = x.GetRequiredService<IHttpClientFactory>();
                var client = new TripletexClient(options, clientFactory);
                return client;
            });
        }

    }
}
