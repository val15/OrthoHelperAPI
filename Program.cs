using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrthoHelperAPI.Data;
using System.Text;
using SQLitePCL;
using OrthoHelperAPI.Services.Interfaces;
using OrthoHelperAPI.Services;
using OrthoHelperAPI.Repositories; // Add this using directive

var builder = WebApplication.CreateBuilder(args);

// Initialize SQLite provider
Batteries.Init(); // Add this line

// Program.cs
// Ajouter avec les autres services
builder.Services.AddScoped<ITextProcessingService, OrthoService>();

// Add services to the container.
builder.Services.AddControllers();

// Add IHttpClientFactory service
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Configuration base de données SQLite
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ajoutez le repository ici, après le DbContext
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

// Configuration JWT
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

// Configuration Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Mon API",
        Version = "v1",
        Description = "API avec authentification et traitement de texte"
    });

    // Configuration Swagger pour JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
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
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Ajout de l'authentification
app.UseAuthorization();

app.MapControllers();

app.Run();