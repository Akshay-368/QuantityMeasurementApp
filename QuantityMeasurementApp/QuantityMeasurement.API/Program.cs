using QuantityMeasurement.Application.Interfaces;
using QuantityMeasurement.Infrastructure.Interfaces;
using QuantityMeasurement.Application.Services;
using QuantityMeasurement.Infrastructure.Repositories;

using QuantityMeasurement.API.Middleware; // For custom middleware
using Microsoft.Extensions.Caching.StackExchangeRedis; // optional

// This file contains the code for the Dependency Injection
var builder = WebApplication.CreateBuilder(args);

var redisConfig = builder.Configuration.GetSection("Redis").GetValue<string>("Configuration") ?? "localhost:6379";
var instanceName = builder.Configuration.GetSection("Redis").GetValue<string>("InstanceName") ?? "QuantityMeasurementApp:";
builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = redisConfig ; options.InstanceName = instanceName; });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// register services
builder.Services.AddScoped<IQuantityService, QuantityService>();

// register the Application->Infrastructure repository implementation
builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();




var app = builder.Build();
// Once this line runs , the pipeline begins forming. Everything from up ahead  that use app.Use... , app.Map... , app.Run... ,becomes part of the request pipeline in ASP.NET Core which is running on kestrel.

// Swagger Middleware Componenet.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // To expose the OpenAPi json endpoint , something like this /swagger/v1/swagger.json
    app.UseSwaggerUI(); // To expose the UI( basically serves the interactive swagger ui webpage in the browser when the endpoint is hit ( for current case which is the localhost port)) , something like this /swagger/index.html
}

///<summary>
/// Creating a custom MiddleWare to logging the time between the request and response.
///</summary>
app.UseTimeLogging();


///<summary>
/// MiddleWare Components : Code that runs between the request and controller .
/// As in Asp.Netcore the middleware is added using app.Usesomthing() , app.MapSOmething() ,app.RunSomething(). And middleware is added to the app not the builder 
/// They form the HTTP request pipeline.
/// Now this one : app.UseHttpsRedirection();  is to redirect HTTP to HTTPS. ( like http://localhost:5228/ -> https://localhost:7051/swager , 
/// which is inside the launch-settings.json inside properties folder in the applcationUrl which is for the server from Kestrel 
/// to listen to which network ports  , so they only can contain protocol + host + port number and not the protocol + host + port + path )
///s</summary>
app.UseHttpsRedirection(); 

///<summary>
/// This is the Middleware that handles the Authorization of the HTTP request.
/// And it comes in the second order in the pipeline after the HttpsRedirection middleware.
/// This checks if the user is allowed to access a resource to begin with. So basically it is utilised along with the JWT or controller attributes like [Authorize]
/// But currently this middleware is inactive but ready as there is no authentication yet.
///</summary>
app.UseAuthorization();

///<summary>
/// This is the Middleware that handles the routing of the HTTP request to controller actions .
/// And without it , every API endpoint would stop working.
/// For example if the request GET/api/history comes , then it maps it to the appropriate controler of the HistoryController class
///</summary>
app.MapControllers();

// And the flow of these middlewares is :
// app.UseHttpsRedirection(); -> app.UseAuthorization(); -> app.MapControllers(); and changing the order would break the authentication logic because the request would reach controllers before the authorization check even happens.

///<summary>
/// This is the Middleware that runs the application.
/// It means end the pipeline here . Do not call the next middleware. So no middleware after Run will ever execute.
/// And this starts the host that was created earlier by the WebApplication.CreateBuilder(args); as it internally starts the server from kestrel and begin listening on configured ports.
///</summary>
app.Run();