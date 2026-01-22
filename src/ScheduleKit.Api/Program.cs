using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ScheduleKit.Api.Hubs;
using ScheduleKit.Api.Services;
using ScheduleKit.Application;
using ScheduleKit.Application.Common;
using ScheduleKit.Application.Common.Interfaces;
using ScheduleKit.Infrastructure;
using ScheduleKit.Infrastructure.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/schedulekit-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ScheduleKit API",
        Version = "v1",
        Description = "Enterprise-grade scheduling module API - like Calendly"
    });

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// Add Application layer services (MediatR, FluentValidation)
builder.Services.AddApplication();

// Add Infrastructure layer services (EF Core, Repositories)
builder.Services.AddInfrastructure(options =>
{
    options.UseInMemoryDatabase("ScheduleKitDb");
});

// Add external integration services (calendar, video conferencing, OAuth)
builder.Services.AddExternalIntegrations(builder.Configuration);

// Configure JWT settings
builder.Services.AddJwtConfiguration(builder.Configuration);

// Configure JWT authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "ScheduleKit-Development-Secret-Key-Min-32-Chars!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ScheduleKit";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ScheduleKit";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    // Support JWT tokens in query string for SignalR WebSocket connections
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Register HttpContextAccessor and CurrentUserService
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Add SignalR for real-time updates
builder.Services.AddSignalR();
builder.Services.AddScoped<IRealTimeNotificationService, ScheduleKitHubService>();

// Configure email settings
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection(EmailSettings.SectionName));

// Configure reminder settings and background service
builder.Services.Configure<ReminderSettings>(
    builder.Configuration.GetSection(ReminderSettings.SectionName));
builder.Services.AddHostedService<BookingReminderService>();

// Add health checks
builder.Services.AddHealthChecks();

// Add response caching (required for VaryByQueryKeys in ResponseCache attribute)
builder.Services.AddResponseCaching();

// CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Seed the database in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<ScheduleKit.Infrastructure.Data.DatabaseSeeder>();
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ScheduleKit API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map SignalR hub
app.MapHub<ScheduleKitHub>("/hubs/schedulekit");

app.MapHealthChecks("/health");

try
{
    Log.Information("Starting ScheduleKit API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
