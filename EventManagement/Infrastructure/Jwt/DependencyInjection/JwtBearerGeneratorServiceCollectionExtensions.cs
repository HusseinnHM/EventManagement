using System.IdentityModel.Tokens.Jwt;
using System.Text;
using EventManagement.Identity.Policies;
using EventManagement.Identity.Policies.UserType;
using EventManagement.Infrastructure.Jwt.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EventManagement.Infrastructure.Jwt.DependencyInjection;

public static class JwtBearerGeneratorServiceCollectionExtensions
{
    public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(options => configuration.GetSection(JwtOptions.Jwt).Bind(options));
        var jwtOptions = configuration.GetSection(JwtOptions.Jwt).Get<JwtOptions>()!;
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        return services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
                };
            })
            .Services
            .AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            })
            .AddSingleton<IAuthorizationHandler, UserTypeHandler>()
            .AddSingleton<IAuthorizationPolicyProvider, EventManagementAuthorizationPolicyProvider>()
            .AddTransient<IJwtBearerGenerator, JwtBearerGenerator>();
    }
}