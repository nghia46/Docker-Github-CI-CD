using Docker_Ci_Di.AMQP;
using MassTransit;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Name the Swagger 
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test Ci/Di Api", Version = "v1" });
});

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

        // Thiết lập Retry
        cfg.UseRetry(retryConfig =>
        {
            retryConfig.Interval(5, TimeSpan.FromSeconds(10)); // Thử lại 5 lần, mỗi lần cách nhau 10 giây
        });

        // Tùy chọn khác như Timeout, CircuitBreaker nếu cần
        cfg.UseCircuitBreaker(cbConfig =>
        {
            cbConfig.TrackingPeriod = TimeSpan.FromMinutes(1);
            cbConfig.ActiveThreshold = 5;
            cbConfig.ResetInterval = TimeSpan.FromMinutes(5);
        });
    });
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

// Add hosted service for MassTransit
builder.Services.AddHostedService<MassTransitHostedService>();

var app = builder.Build();

// Start and stop MassTransit bus with the application
var bus = app.Services.GetRequiredService<IBusControl>();
var lifetime = app.Services.GetService<IHostApplicationLifetime>();

lifetime?.ApplicationStarted.Register(() => bus?.StartAsync());
lifetime?.ApplicationStopping.Register(() => bus?.StopAsync());

// Configure the HTTP request pipeline.

app.UsePathBase("/cidiapp"); // Thiết lập base path là /cidiapp

// Cấu hình Swagger UI
app.UseSwaggerUI(c =>
{
    // Cập nhật đường dẫn Swagger JSON với base path mới
    c.SwaggerEndpoint("/cidiapp/swagger/v1/swagger.json", "Test Ci/Di API");
    c.RoutePrefix = "swagger"; // Swagger UI sẽ phục vụ tại /cidiapp/swagger
});

// Cấu hình Swagger JSON
app.UseSwagger(options =>
{
    // Đảm bảo Swagger JSON có thể truy cập qua /cidiapp/swagger/v1/swagger.json
    options.RouteTemplate = "/swagger/{documentName}/swagger.json"; 
});

// Xử lý các yêu cầu
app.UseRouting();
app.UseAuthorization();
app.UseCors();
app.MapControllers();

app.Run();
