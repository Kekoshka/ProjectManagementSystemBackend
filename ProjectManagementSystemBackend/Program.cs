using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectManagementSystemBackend.Common;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.Options;
using ProjectManagementSystemBackend.Services;
using ProjectManagementSystemBackend.Services.Authorization.Handlers.ProjectHandlers;
using ProjectManagementSystemBackend.Services.Authorization.Requirements.ProjectRequirements;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Reflection;
using System.Text;
using IAuthorizationService = ProjectManagementSystemBackend.Interfaces.IAuthorizationService;
using Task = ProjectManagementSystemBackend.Models.Task;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers(options =>
options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<ApplicationContext>(config =>
{
    config.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionStringMSSQL"));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {

            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWTOptions:Issuer"],
            ValidateAudience = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTOptions:Key"])),
            AudienceValidator = (audiences, securityToken, validationParameters) =>
            {
                using var scope = builder.Services.BuildServiceProvider().CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                return dbContext.Users.Any(u => u.Id.ToString() == audiences.FirstOrDefault());
            }
        };
    });

builder.Services.AddMapster();

builder.Services.AddScoped<IAuthorizationHandler, ProjectOwnerHandler>();


builder.Services.AddAuthorization( options =>
    AuthorizationPoliciesConfig.Configure(options));

RegisterServices.RegisterExecutingAsseblyServices(builder.Services);

builder.Services.Configure<RoleOptions>(builder.Configuration.GetSection(nameof(RoleOptions)));
builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection(nameof(JWTOptions)));


TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseSwagger()
    .UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });

app.Run();


