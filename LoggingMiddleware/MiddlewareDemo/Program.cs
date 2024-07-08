using System.Net;
using System.Threading.RateLimiting;

using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.RateLimiting;

using MiddlewareDemo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the rate limiting middleware
builder.Services.AddRateLimiter(_ =>
    _.AddFixedWindowLimiter(policyName: "fixed", options =>
        {
            options.PermitLimit = 5;
            options.Window = TimeSpan.FromSeconds(10);
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            options.QueueLimit = 2;
        }));

// Add the request timeout middleware
//builder.Services.AddRequestTimeouts();
builder.Services.AddRequestTimeouts(option =>
{
    option.DefaultPolicy = new RequestTimeoutPolicy { Timeout = TimeSpan.FromSeconds(5) };
    option.AddPolicy("ShortTimeoutPolicy", TimeSpan.FromSeconds(2));
    option.AddPolicy("LongTimeoutPolicy", TimeSpan.FromSeconds(10));
});


var app = builder.Build();

// This example is to enable the short-circuit middleware.
// The short-circuit middleware should be placed before other middleware components.
//app.MapGet("robots.txt", () => Results.Content("User-agent: *\nDisallow: /", "text/plain")).ShortCircuit();
app.MapShortCircuit((int)HttpStatusCode.NotFound, "robots.txt", "favicon.ico");

// This is a terminal middleware. It can short-circuit the pipeline because it does not call the `next()` method.
// app.Run(async context =>
// {
//    await context.Response.WriteAsync("Hello world!");
// });

// This is a simple middleware that outputs the Host URL and the response status code.
// app.Use(async (context, next) =>
// {
//    var logger = app.Services.GetRequiredService<ILogger<Program>>();
//    logger.LogInformation($"Request Host: {context.Request.Host}");
//    logger.LogInformation("My Middleware - Before");
//    await next(context);
//    logger.LogInformation("My Middleware - After");
//    logger.LogInformation($"Response StatusCode: {context.Response.StatusCode}");
// });

// This is to show how to use the `next()` method to call the next middleware in the pipeline.
// app.Use(async (context, next) =>
// {
//    var logger = app.Services.GetRequiredService<ILogger<Program>>();
//    logger.LogInformation($"ClientName HttpHeader in Middleware 1: {context.Request.Headers["ClientName"]}");
//    logger.LogInformation($"Add a ClientName HttpHeader in Middleware 1");
//    context.Request.Headers.TryAdd("ClientName", "Windows");
//    logger.LogInformation("My Middleware 1 - Before");
//    await next(context);
//    logger.LogInformation("My Middleware 1 - After");
//    logger.LogInformation($"Response StatusCode in Middleware 1: {context.Response.StatusCode}");
// });

// app.Use(async (context, next) =>
// {
//    var logger = app.Services.GetRequiredService<ILogger<Program>>();
//    logger.LogInformation($"ClientName HttpHeader in Middleware 2: {context.Request.Headers["ClientName"]}");
//    logger.LogInformation("My Middleware 2 - Before");
//    context.Response.StatusCode = StatusCodes.Status202Accepted;
//    await next(context);
//    logger.LogInformation("My Middleware 2 - After");
//    logger.LogInformation($"Response StatusCode in Middleware 2: {context.Response.StatusCode}");
// });

// This is an example showing how to combine multiple middleware components into a single middleware component.
// app.Map("/lottery", app =>
// {
//    var random = new Random();
//    var luckyNumber = random.Next(1, 6);
//    app.UseWhen(context => context.Request.QueryString.Value == $"?{luckyNumber.ToString()}", app =>
//    {
//        app.Run(async context =>
//        {
//            await context.Response.WriteAsync($"You win! You got the lucky number {luckyNumber}!");
//        });
//    });
//    app.UseWhen(context => string.IsNullOrWhiteSpace(context.Request.QueryString.Value), app =>
//    {
//        app.Use(async (context, next) =>
//        {
//            var number = random.Next(1, 6);
//            context.Request.Headers.TryAdd("number", number.ToString());
//            await next(context);
//        });
//        app.MapWhen(context => context.Request.Headers["number"] == luckyNumber.ToString(), app =>
//        {
//            app.Run(async context =>
//            {
//                await context.Response.WriteAsync($"You win! You got the lucky number {luckyNumber}!");
//            });
//        });
//    });
//    app.Run(async context =>
//    {
//        var number = "";
//        if (context.Request.QueryString.HasValue)
//        {
//            number = context.Request.QueryString.Value?.Replace("?", "");
//        }
//        else
//        {
//            number = context.Request.Headers["number"];
//        }
//        await context.Response.WriteAsync($"Your number is {number}. Try again!");
//    });
// });
// app.Run(async context =>
// {
//    await context.Response.WriteAsync($"Use the /lottery URL to play. You can choose your number with the format /lottery?1.");
// });


// This is an example to show the difference between `app.UseWhen()` and `app.MapWhen()`.
//app.MapWhen(context => context.Request.Query.ContainsKey("branch"), app =>
//{
//    app.Use(async (context, next) =>
//    {
//        var logger = app.ApplicationServices.GetRequiredService<ILogger<Program>>();
//        logger.LogInformation($"From MapWhen(): Branch used = {context.Request.Query["branch"]}");
//        await next();
//    });
//    app.Run(async context =>
//    {
//        var branchVer = context.Request.Query["branch"];
//        await context.Response.WriteAsync($"Branch used = {branchVer}");
//    });
//});
//app.UseWhen(context => context.Request.Query.ContainsKey("branch"), app =>
//{
//    app.Use(async (context, next) =>
//    {
//        var logger = app.ApplicationServices.GetRequiredService<ILogger<Program>>();
//        logger.LogInformation($"From UseWhen(): Branch used = {context.Request.Query["branch"]}");
//        await next();
//    });
//});
//app.Run(async context =>
//{
//    await context.Response.WriteAsync("Hello world!");
//});

// This example is to enable the rate-limiting middleware.
app.UseRateLimiter();

app.MapGet("/rate-limiting-mini", () => Results.Ok($"Hello {DateTime.Now.Ticks.ToString()}")).RequireRateLimiting("fixed");

// This is an example of custom middleware.
app.UseCorrelationId();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// This example is to enable the request timeout middleware.
app.UseRequestTimeouts();

app.MapGet("/request-timeout-mini", async (HttpContext context, ILogger<Program> logger) =>
{
    var random = new Random();
    var delay = random.Next(1, 10);
    logger.LogInformation($"Delaying for {delay} seconds");
    try
    {
        await Task.Delay(TimeSpan.FromSeconds(delay), context.RequestAborted);
    }
    catch
    {
        logger.LogWarning("The request timed out");
        return Results.Content("The request timed out", "text/plain");
    }

    return Results.Content($"Hello! The task is complete in {delay} seconds", "text/plain");
}).WithRequestTimeout(TimeSpan.FromSeconds(5));

app.Run();
