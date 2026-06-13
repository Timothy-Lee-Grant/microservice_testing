using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 1. Tell the service container to load and prepare the YARP engine, 
// using the "ReverseProxy" section from our appsettings.json file above.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// 2. Inject YARP directly into the active HTTP request handling pipeline.
app.MapReverseProxy();

app.Run();