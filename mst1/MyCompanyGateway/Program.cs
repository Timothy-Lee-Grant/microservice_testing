using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 1. Tell the service container to load and prepare the YARP engine, 
// // using the "ReverseProxy" section from our appsettings.json file above.
// builder.Services.AddReverseProxy()
//     .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("StrictFixedWindow", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromSeconds(1);
    });
});

// builder.Services.AddAuthentication(options =>
// {
//     options.AddPolicy("RequireLoggedUsers", policy =>
//         policy.RequireAuthenticatedUser()
//     );
// });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireLoggedUsers", policy => 
        policy.RequireAuthenticatedUser()); // Must have a valid login token
});

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// 2. Inject YARP directly into the active HTTP request handling pipeline.
app.MapReverseProxy();

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();