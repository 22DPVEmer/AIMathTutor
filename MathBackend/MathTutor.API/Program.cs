using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using MathTutor.Application.Interfaces;
using MathTutor.Application.Services;
using MathTutor.Core.Entities;
using MathTutor.Core.Mappings;
using MathTutor.Infrastructure.Persistence;
using MathTutor.Infrastructure.Repositories;
using MathTutor.Infrastructure.Services;
using MathTutor.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MathTutor.Application.Services.AuthService).Assembly));

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MathTutor.Core.Mappings.AutoMapperProfile).Assembly,
                          typeof(MathTutor.Application.Mappings.ApplicationAutoMapperProfile).Assembly);

// Register application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

// Register HttpClient for AI services
builder.Services.AddHttpClient("AIClient", client =>
{
    client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

// Register Semantic Kernel
builder.Services.AddSingleton<Kernel>(sp => {
    var configuration = sp.GetRequiredService<IConfiguration>();

    var apiKey = configuration["AI:Gemini:ApiKey"] ??
        throw new InvalidOperationException("Gemini API Key not found in user secrets.");

    var httpClient = sp.GetRequiredService<HttpClient>();
#pragma warning disable SKEXP0070
    return Kernel.CreateBuilder()
        .AddGoogleAIGeminiChatCompletion(
            modelId: "gemini-2.0-flash",
            apiKey: apiKey,
            httpClient: httpClient,
            serviceId: "gemini")
        .Build();
});

// Register KernelProvider
builder.Services.AddScoped<IKernelProvider, KernelProvider>();

// Register JsonService
builder.Services.AddScoped<IJsonService, JsonService>();

// Register specialized AI services
builder.Services.AddScoped<IProblemGenerationService, ProblemGenerationService>();
builder.Services.AddScoped<IAnswerEvaluationService, AnswerEvaluationService>();
builder.Services.AddScoped<IGuidanceService, GuidanceService>();

// Register legacy AIService for backward compatibility
builder.Services.AddScoped<IAIservice, AIservice>();

// Register Math Problem Services and Repository
builder.Services.AddScoped<IMathProblemService, MathProblemService>();
builder.Services.AddScoped<IMathProblemRepository, MathProblemRepository>();

// Register Math Topic Services and Repository
builder.Services.AddScoped<IMathTopicService, MathTopicService>();
builder.Services.AddScoped<IMathTopicRepository, MathTopicRepository>();
builder.Services.AddScoped<IMathProblemAttemptRepository, MathProblemAttemptRepository>();


// Register User Math Problem Services and Repository
builder.Services.AddScoped<IUserMathProblemService, UserMathProblemService>();
builder.Services.AddScoped<IUserMathProblemRepository, UserMathProblemRepository>();

// Register School Class Services and Repository
builder.Services.AddScoped<ISchoolClassService, SchoolClassService>();
builder.Services.AddScoped<ISchoolClassRepository, SchoolClassRepository>();

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

// Register Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false; // Set to true in production
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);

var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings?.Secret ?? "DefaultSecretKeyForDevelopmentEnvironmentOnly12345");

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtSettings?.Issuer ?? "MathTutor",
            ValidAudience = jwtSettings?.Audience ?? "MathTutorUsers",
            ClockSkew = TimeSpan.Zero
        };
    });

// Define allowed origins for CORS
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
if (allowedOrigins == null || allowedOrigins.Length == 0)
{
    allowedOrigins = new[] { "http://localhost:5174", "http://localhost:5173" };
}

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });

    // Add a more permissive policy for development
    options.AddPolicy("DevCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// Register infrastructure services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Add services to the container.
builder.Services.AddScoped<MathKernelService>();

// Add controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Add API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Comment out HTTPS redirection in development
    // app.UseHttpsRedirection();
}
else
{
    app.UseHttpsRedirection();
}

// Apply CORS policy
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCorsPolicy");
    Console.WriteLine("Using development CORS policy (AllowAnyOrigin)");
}
else
{
    app.UseCors("CorsPolicy");
    Console.WriteLine("Using production CORS policy");
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
