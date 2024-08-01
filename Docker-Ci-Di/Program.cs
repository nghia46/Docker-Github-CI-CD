using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//Name the Swagger 
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test Ci/Di Api", Version = "v1" }); });

var app = builder.Build();

// Configure the HTTP request pipeline.

//Load swagger.json following root directory 
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/v1/swagger.json", "Test Ci/Di Api"); c.RoutePrefix = string.Empty; });
//Get swagger.json following root directory 
app.UseSwagger(options => { options.RouteTemplate = "{documentName}/swagger.json"; });

app.UseAuthorization();

app.MapControllers();

app.Run();
