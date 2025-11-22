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
using System.Text.Json;

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
                        if (context.Principal?.Identity is ClaimsIdentity identity)
                        {
                            // Estrai i ruoli da realm_access
                            var realmAccessClaim = identity.FindFirst("realm_access");
                            if (realmAccessClaim != null)
                            {
                                try
                                {
                                    var realmAccess = JsonSerializer.Deserialize<JsonElement>(realmAccessClaim.Value);
                                    if (realmAccess.TryGetProperty("roles", out var rolesElement))
                                    {
                                        var roles = rolesElement.EnumerateArray()
                                            .Select(role => role.GetString())
                                            .Where(role => !string.IsNullOrEmpty(role));

                                        // Aggiungi ogni ruolo come claim separato
                                        foreach (var role in roles)
                                        {
                                            identity.AddClaim(new Claim(ClaimTypes.Role, role!));
                                        }

                                        Console.WriteLine($"Roles extracted: {string.Join(", ", roles)}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error parsing realm_access: {ex.Message}");
                                }
                            }

                            // Estrai anche i ruoli da resource_access (opzionale)
                            var resourceAccessClaim = identity.FindFirst("resource_access");
                            if (resourceAccessClaim != null)
                            {
                                try
                                {
                                    var resourceAccess = JsonSerializer.Deserialize<JsonElement>(resourceAccessClaim.Value);
                                    foreach (var client in resourceAccess.EnumerateObject())
                                    {
                                        if (client.Value.TryGetProperty("roles", out var clientRoles))
                                        {
                                            var roles = clientRoles.EnumerateArray()
                                                .Select(role => role.GetString())
                                                .Where(role => !string.IsNullOrEmpty(role));

                                            foreach (var role in roles)
                                            {
                                                identity.AddClaim(new Claim(ClaimTypes.Role, $"{client.Name}:{role}"));
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error parsing resource_access: {ex.Message}");
                                }
                            }
                        }

                        // Log dei claim finali (debug)
                        var claims = context.Principal?.Claims.Select(c => $"{c.Type}={c.Value}");
                        Console.WriteLine($"Token validated. Final claims: {string.Join(", ", claims ?? [])}");
                        
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
                    RoleClaimType = ClaimTypes.Role  // Usa il claim type standard per i ruoli
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
