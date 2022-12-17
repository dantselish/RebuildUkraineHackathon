using Microsoft.OpenApi.Models;
using RebuildUkraineHackathonWebAPI;
using RebuildUkraineHackathonWebAPI.Controllers;
using RebuildUkraineHackatonWebAPI.MongoDB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<BasicAuthServiceFilter>();
builder.Services.AddScoped<IMongoDBController, MongoDBController>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( x =>
{
  x.SwaggerDoc("v1", new OpenApiInfo { Title = "Cool API", Version = "v1" });
  x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Description = "Vasya?",
    Name = "Authorization",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer"
  });

  x.AddSecurityRequirement(new OpenApiSecurityRequirement()
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = "Bearer"
        },
        Scheme = "oauth2",
        Name = "Bearer",
        In = ParameterLocation.Header,

      },
      new List<string>()
    }
  });
});
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseSwagger();
app.UseSwaggerUI( x => x.SwaggerEndpoint( "/swagger/v1/swagger.json", "Cool API V1" ) );

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();