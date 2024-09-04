using ConcurrentDataflowWorkerPoc;
using ConcurrentDataflowWorkerPoc.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<IGetEndpointData, GetEndpointData>(httpClient =>
{
    httpClient.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
});

builder.Services.AddTransient<IDataWorkflowService, DataWorkflowService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
