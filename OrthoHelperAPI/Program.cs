using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrthoHelperAPI.Data;
using System.Text;
using SQLitePCL;
using OrthoHelperAPI.Services.Interfaces;
using OrthoHelperAPI.Services;
using OrthoHelperAPI.Repositories;
using OrthoHelper.Application.Features.TextCorrection.UseCases;
using OrthoHelper.Domain.Ports;
using OrthoHelper.Infrastructure.TextProcessing;
using OrthoHelper.Application.Tests.Features.TextCorrection.UseCases;

var builder = WebApplication.CreateBuilder(args);



// 1. Initialisation SQLite
Batteries.Init();

// 2. Configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

//OLD
builder.Services.AddScoped<ITextProcessingService, OrthoService>();


// 3. Configuration HttpClient pour Ollama
var ollamaConfig = builder.Configuration.GetSection("OllamaSettings");
builder.Services.AddHttpClient("Ollama", client =>
{
    client.BaseAddress = new Uri(ollamaConfig["Address"]!);
    client.Timeout = TimeSpan.FromMinutes(15);
});


// 3. Configuration de l'Infrastructure
// 4. Configuration de l'Infrastructure
builder.Services.AddSingleton<IOrthoEngine>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    return new OrthoEngine(
        httpClientFactory.CreateClient("Ollama"))
    {
        // Configuration supplémentaire si nécessaire
    };
});

builder.Services.AddScoped<ITextProcessingEngine, OrthoEngineAdapter>();

//builder.Services.AddSingleton<IOrthoEngine, OrthoEngine>(); // Interface -> Implémentation
//builder.Services.AddScoped<ITextProcessingEngine, OrthoEngineAdapter>();

// 4. Configuration de l'Application
builder.Services.AddScoped<ICorrectTextUseCase, CorrectTextUseCase>();

// 5. Configuration de l'API
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// 6. Configuration de la base de données
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 7. Autres services
builder.Services.AddScoped<ITextProcessingService, OrthoService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

// 8. Configuration JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key non trouvé"))),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// 9. Configuration Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OrthoHelper API",
        Version = "v1",
        Description = "API de correction orthographique"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 10. Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrthoHelper API v1"));
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();