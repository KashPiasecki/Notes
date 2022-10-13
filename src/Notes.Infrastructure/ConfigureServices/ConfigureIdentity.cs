using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Notes.Domain.Contracts.Constants;
using Notes.Infrastructure.Persistence;

namespace Notes.Infrastructure.ConfigureServices;

public static class ConfigureIdentity
{
    public static void AddIdentity(this IServiceCollection serviceCollection, string secret)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = false,
            ValidateLifetime = true
        };
        serviceCollection.AddSingleton(tokenValidationParameters);
        serviceCollection.AddIdentity<IdentityUser, IdentityRole>(
            options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredLength = 6;
            }
        ).AddEntityFrameworkStores<DataContext>();
        serviceCollection.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = tokenValidationParameters;
            });
        serviceCollection.AddAuthorization(options =>
        {
            options.AddPolicy("RolePolicy", policy => policy.RequireRole(RoleNames.User));
            options.AddPolicy("RolePolicy", policy => policy.RequireRole(RoleNames.Admin));
        });
    }
}