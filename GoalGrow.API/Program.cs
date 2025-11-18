using GoalGrow.API.Extensions;
using GoalGrow.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace GoalGrow.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/goalgrow-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            // Add services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Database
            builder.Services.AddDbContext<GoalGrowDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("GoalGrowDb");
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
            });

            // JWT Authentication
            var keycloakAuthority = builder.Configuration["Keycloak:Authority"];
            var keycloakAudience = builder.Configuration["Keycloak:Audience"];

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = keycloakAuthority;
                    options.Audience = keycloakAudience;
                    options.RequireHttpsMetadata = builder.Configuration.GetValue<bool>("Keycloak:RequireHttpsMetadata");
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = builder.Configuration.GetValue<bool>("Keycloak:ValidateIssuer"),
                        ValidateAudience = builder.Configuration.GetValue<bool>("Keycloak:ValidateAudience"),
                        ValidateLifetime = builder.Configuration.GetValue<bool>("Keycloak:ValidateLifetime"),
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("Keycloak:ClockSkew"))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Log.Error("Authentication failed: {Error}", context.Exception.Message);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Log.Information("Token validated for user: {User}", context.Principal?.Identity?.Name);
                            return Task.CompletedTask;
                        }
                    };
                });

            // Authorization Policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("InvestorOnly", policy => policy.RequireRole("investor"));
                options.AddPolicy("ConsultantOnly", policy => policy.RequireRole("consultant"));
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
                options.AddPolicy("KycVerified", policy => policy.RequireRole("kyc-verified"));
            });

            // CORS
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowGoalGrowOrigins", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(Program));

            // Application Services
            builder.Services.AddApplicationServices();

            var app = builder.Build();

            // Configure HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseCors("AllowGoalGrowOrigins");
            app.UseAuthentication(); // ? Must be before UseAuthorization
            app.UseAuthorization();
            app.MapControllers();

            // Database connection check
            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<GoalGrowDbContext>();
                try
                {
                    if (db.Database.CanConnect())
                    {
                        Log.Information("? Database connection successful");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "? Database connection failed");
                }
            }

            Log.Information("?? GoalGrow API started on {Urls}", string.Join(", ", builder.Configuration.GetSection("urls").Get<string[]>() ?? new[] { "https://localhost:5001" }));

            app.Run();
        }
    }
}
