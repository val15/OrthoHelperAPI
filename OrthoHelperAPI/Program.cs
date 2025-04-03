using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using SQLitePCL;
using OrthoHelperAPI.Services.Interfaces;
using OrthoHelperAPI.Services;
using OrthoHelperAPI.Repositories;
using OrthoHelper.Application.Features.TextCorrection.UseCases;
using OrthoHelper.Application.Tests.Features.TextCorrection.UseCases;
using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Infrastructure.Features.TextProcessing;
using OrthoHelper.Infrastructure.Features.Auth.Repositories;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using OrthoHelper.Infrastructure.Features.TextProcessing.Repositories;
using OrthoHelper.Domain.Features.TextCorrection.Mappings;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Infrastructure.Features.Common.Services.OrthoHelper.Infrastructure.Features.Common.Services;

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
builder.Services
    .AddScoped<ICorrectionSessionRepository, CorrectionSessionRepository>()
    .AddHttpClient("Ollama", client =>
    {
        client.BaseAddress = new Uri(ollamaConfig["Address"]!);
        client.Timeout = TimeSpan.FromMinutes(15);
    });


// 3. Configuration de l'Infrastructure
// 4. Configuration de l'Infrastructure
builder.Services.AddScoped<IOrthoEngine>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var repository = provider.GetRequiredService<ICorrectionSessionRepository>();
    var currentUserService = provider.GetRequiredService<ICurrentUserService>();

    return new OrthoEngine(
        httpClient: httpClientFactory.CreateClient("Ollama"),
        repository: repository, // Ajout du paramètre manquant
    currentUserService: currentUserService
        );
});


builder.Services
    .AddHttpContextAccessor() // Nécessaire pour IHttpContextAccessor
    .AddScoped<ICurrentUserService, CurrentUserService>();

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
builder.Services.AddDbContext<OrthoHelper.Infrastructure.Features.Common.Persistence.DbContext.ApiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//OLD
builder.Services.AddDbContext<OrthoHelperAPI.Data.ApiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Enregistrement des dépendances Auth
builder.Services.AddScoped<OrthoHelper.Domain.Features.Auth.Ports.IUserRepository, UserRepository>();
builder.Services.AddScoped<OrthoHelper.Domain.Features.Auth.Ports.ITokenService, OrthoHelper.Infrastructure.Features.Auth.Services.TokenService>();

// ajout des handlers CQR 
builder.Services.AddScoped<OrthoHelper.Domain.Features.Auth.Ports.IUserRepository, UserRepository>();
builder.Services.AddScoped<OrthoHelper.Domain.Features.Auth.Ports.ITokenService, OrthoHelper.Infrastructure.Features.Auth.Services.TokenService>();

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

//// Ajouter MediatR
//builder.Services.AddMediatR(cfg =>
//    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(OrthoHelper.Application.AssemblyMarker).Assembly));


builder.Services.AddAutoMapper(typeof(CorrectionSessionProfile)); // Enregistrer le profil AutoMapper


//builder.Services.AddMediatR(cfg =>
//    cfg.RegisterServicesFromAssembly(typeof(OrthoHelper.Application.AssemblyMarker).Assembly));


var app = builder.Build();

// ------------------------- MIGRATE DB-------------------------
// Appliquer les migrations EF Core au démarrage
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<OrthoHelper.Infrastructure.Features.Common.Persistence.DbContext.ApiDbContext>();
        dbContext.Database.Migrate(); // Crée les tables manquantes
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erreur lors de l'application des migrations");
    }
}
// ------------------------- END MIGRATE DB -------------------------


// 10. Middleware pipeline
//TODO A DECOMMENTER
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrthoHelper API v1"));
//}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();