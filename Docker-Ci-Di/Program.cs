using Docker_Ci_Di.AMQP;
using MassTransit;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//Name the Swagger 
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test Ci/Di Api", Version = "v1" });
});
// 
builder.Services.AddScoped<IMessagePublisher, MessagePublisher>();
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((cxt, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration["RabbitMQ:Host"]), h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"]);
            h.Password(builder.Configuration["RabbitMQ:Password"]);
        });
    });
    // Add hosted service for MassTransit
});

#region defaultService

builder.Services.AddSingleton(builder.Configuration);
builder.Configuration.AddJsonFile($"appsettings.json");
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

#endregion

builder.Services.AddHostedService<MassTransitHostedService>();

var app = builder.Build();

// Start and stop MassTransit bus with the application
var bus = app.Services.GetRequiredService<IBusControl>();
var lifetime = app.Services.GetService<IHostApplicationLifetime>();

lifetime?.ApplicationStarted.Register(() => bus?.StartAsync());
lifetime?.ApplicationStopping.Register(() => bus?.StopAsync());

// Configure the HTTP request pipeline.

//Load swagger.json following root directory 
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/v1/swagger.json", "Test Ci/Di Api");
    c.RoutePrefix = string.Empty;
});
//Get swagger.json following root directory 
app.UseSwagger(options => { options.RouteTemplate = "{documentName}/swagger.json"; });

app.UseAuthorization();
app.UseCors();
app.MapControllers();

app.Run();