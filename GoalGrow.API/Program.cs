using GoalGrow.API.Extensions;
using GoalGrow.API.Services.Implementations;
using GoalGrow.API.Services.Interfaces;
using GoalGrow.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GoalGrow.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Preserva i claim JWT originali (non mappare in nomi Microsoft)
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            // HttpClient per AuthService
            builder.Services.AddHttpClient();

            // Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var keycloakSettings = builder.Configuration.GetSection("Keycloak");
                options.Authority = keycloakSettings["Authority"];
                options.RequireHttpsMetadata = bool.Parse(keycloakSettings["RequireHttpsMetadata"] ?? "true");
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Auth failed: {context.Exception?.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        // Log dei claim ricevuti (debug)
                        var claims = context.Principal?.Claims.Select(c => $"{c.Type}={c.Value}");
                        Console.WriteLine($"Token validated. Claims: {string.Join(", ", claims ?? [])}");
                        return Task.CompletedTask;
                    }
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudiences = new[] { "goalgrow-api", "account" }, // Accetta entrambi
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    // Mappa i claim con nomi standard
                    NameClaimType = "preferred_username", // User.Identity.Name
                    RoleClaimType = "realm_access.roles"  // User.IsInRole("admin")
                };
            });

            builder.Services.AddAuthorization();

            builder.Services.AddDbContext<GoalGrowDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("GoalGrowDb");
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
            });


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.WithTitle("GoalGrow API");
                    options.WithTheme(ScalarTheme.Moon);
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
