using System;
using System.Reflection;
using System.Text.Json.Serialization;
using EventManagement.Application.Core.Abstractions.Services;
using EventManagement.Common.LanguageKey;
using EventManagement.Infrastructure.EntityFrameworkCore;
using EventManagement.Infrastructure.EntityFrameworkCore.DbContexts;
using EventManagement.Infrastructure.FakeTranslate;
using EventManagement.Infrastructure.HttpResolver;
using EventManagement.Infrastructure.Jwt.DependencyInjection;
using EventManagement.Infrastructure.PasswordHash;
using EventManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Neptunee.BaseCleanArchitecture.AppBuilder.ExceptionHandlerMiddleware;
using Neptunee.BaseCleanArchitecture.AppBuilder.InitialAppBuilder;
using Neptunee.BaseCleanArchitecture.DependencyInjection;
using Neptunee.EntityFrameworkCore.MultiLanguage.DependencyInjection;
using Neptunee.OperationResponse.DependencyInjection;
using Neptunee.Swagger;
using Neptunee.Swagger.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(o => o.AddDefaultPolicy(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()))
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .Services
    .AddOperationSerializerOptions()
    .AddLogging()
    .AddNeptuneeSwagger(o =>
    {
        o.AddJwtBearerSecurityScheme();
        o.OperationFilter<LanguageKeySwaggerFilter>();
    })
    .AddNeptuneeBaseCleanArchitecture(Assembly.GetEntryAssembly()!)
    .AddTransient<IFakeTranslate, FakeTranslateImp>()
    .AddTransient<IPasswordHasher, PasswordHasher>()
    .AddDbContext<EventManagementDbContext>(o =>
    {
        o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        if (!builder.Environment.IsProduction())
        {
            o.EnableSensitiveDataLogging();
        }
    })
    .AddMultiLanguage<EventManagementDbContext>()
    .AddJwt(builder.Configuration)
    .AddHttpContextAccessor()
    .AddTransient<IHttpResolver, HttpResolverImp>()
    .AddScoped<IRepository, Repository>()
    .Scan(selector => selector
        .FromEntryAssembly()
        .AddClasses(a => a.AssignableTo<IApplicationService>())
        .AsImplementedInterfaces()
        .WithTransientLifetime())
    ;


var app = builder.Build();


app
    .UseNeptuneeExceptionHandler()
    .UseCors()
    .UseNeptuneeSwagger(o => o.SetDocExpansion());

app.MapControllers();
app.MapGet("/", () => Results.LocalRedirect("/swagger/index.html"));

await app.MigrationAsync<EventManagementDbContext>(DataSeed.Seed);

app.Run();