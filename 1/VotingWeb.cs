namespace VotingWeb 
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.IO; 
    using System.Net.Http;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
    using Microsoft.ServiceFabric.Services.Communication.Runtime; 
    using Microsoft.ServiceFabric.Services.Runtime;
    internal sealed class VotingWeb : StatelessService
    {
        public VotingWeb(StatelessServiceContext context)
        : base(context)
        {
        }
        protected override IEnumerable < ServiceInstanceListener > CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>

                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint",
                        (url, listener) => {
                            ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                            return new WebHostBuilder()
                                .UseKestrel()
                                .ConfigureServices(services => services
                                    .AddSingleton < HttpClient > (new HttpClient())
                                        .AddSingleton < FabricClient > (new FabricClient())
                                        .AddSingleton < StatelessServiceContext > (serviceContext))
                                .UseContentRoot(Directory.GetCurrentDirectory())
                                .UseStartup < Startup > ()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url)
                                    .Build();
                        }))
            };
        }
internal static Uri GetVotingDataServiceName(ServiceContext context)
        {
            return new Uri($"{context.CodePackageActivationContext.ApplicationName}/VotingData");
        }
    }
}
