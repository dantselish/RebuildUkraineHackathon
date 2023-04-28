using System.Diagnostics;
using Invincible.MongoDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Invincible;
using Invincible.Controllers;
using Microsoft.AspNetCore.Authentication;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddControllers();
builder.Services.AddSingleton<IMongoDBController, MongoDBController>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( x =>
{
  x.SwaggerDoc("v1", new OpenApiInfo { Title = "Cool API", Version = "v1" });
  x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Description = "Google JWT Token",
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
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 

})
.AddJwtBearer(o =>
{
    o.IncludeErrorDetails = true;
    o.SecurityTokenValidators.Clear();
    o.SecurityTokenValidators.Add(new GoogleTokenValidator());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI( x =>
  {
    x.SwaggerEndpoint("/swagger/v1/swagger.json", "Cool API V1");
    x.OAuthClientId("780517687865-buts8ebadk6gqqlcje4ge1rvolcrtvig.apps.googleusercontent.com");
    x.OAuthClientSecret("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");
    x.InjectJavascript("SwaggerGoogleAuth.js");
  }
);

app.UseHttpsRedirection();

app.UseMiddleware<AuthenticationMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

void AddSwaggerOAuth2Configuration(SwaggerGenOptions swaggerGenOptions) 
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows()
        {
            Implicit = new OpenApiOAuthFlow()
            {
                AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/v2/auth"),
                Scopes = new Dictionary<string, string> {{"email", "https://www.googleapis.com/auth/userinfo.email"}, {"profile", "https://www.googleapis.com/auth/userinfo.profile	"}, {"openid", "openid"}}
            }
        },
        Extensions = new Dictionary<string, IOpenApiExtension>
        {
            {"x-tokenName", new OpenApiString("code")},
        },
    };
        
    swaggerGenOptions.AddSecurityDefinition("Bearer", securityScheme) ;
    
    var securityRequirements = new OpenApiSecurityRequirement 
    {
        {
            new OpenApiSecurityScheme 
            { 
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" 
                } 
            },
            new List<string> {"email", "profile"}
        } 
    };
    
    swaggerGenOptions.AddSecurityRequirement(securityRequirements);
}