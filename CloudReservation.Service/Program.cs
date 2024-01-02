using System.Reflection;
using System.Text;
using CloudReservation.DAL.Data;
using CloudReservation.DAL.Repositories;
using CloudReservation.Service.Behaviours;
using CloudReservation.Service.Hubs;
using CloudReservation.Service.Middleware;
using CloudReservation.Service.Models.UserModels;
using CloudReservation.Service.Services.GraphServices.Builder;
using CloudReservation.Service.Services.GraphServices.Calendar;
using CloudReservation.Service.Services.GraphServices.Subscription;
using CloudReservation.Service.Services.HashingService;
using CloudReservation.Service.Services.NotificationCacheService;
using CloudReservation.Service.Services.TokenServices;
using CloudReservation.Service.Validation.Filters;
using CloudReservation.Service.Wrappers.ConfigurationWrapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

#region Configuration setup
var env = builder.Environment;
var configuration = builder.Configuration;

configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

if (env.IsDevelopment())
{
    configuration.AddJsonFile($"appsettings.{Environments.Development}.json", true, true);
    configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
}

builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddSingleton<IConfigurationWrapper, ConfigurationWrapper>();

#endregion

#region NotificationCacheService setup

builder.Services.AddSingleton<INotificationCacheService, NotificationCacheService>();

#endregion

#region SignalR setup

builder.Services.AddSignalR();

#endregion

#region GraphService setup

//This is the new setup for the graph service logic
builder.Services.AddSingleton<IGraphBuilder, GraphBuilder>();
builder.Services.AddScoped<IGraphCalendarService, GraphCalendarService>();

if (!Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!.Equals("Test"))
{
    builder.Services.AddHostedService<GraphSubscriptionService>();
}
#endregion

#region HashService setup

builder.Services.AddScoped<IHashService, HashService>();

#endregion

#region JwtService setup

builder.Services.AddScoped<IJwtService, JwtService>();

#endregion

#region MediatR setup

builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining(typeof(Program)));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

#endregion

#region AutoMapper setup

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

#endregion

#region FluentValidation setup

builder.Services.AddFluentValidationRulesToSwagger().AddFluentValidationClientsideAdapters();
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

#endregion

#region Database setup

var connectionString = configuration.GetConnectionString("DbConnectionString")!;
builder.Services.AddDbContext<CloudReservationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IUserRepository, UserRepository>();

#endregion

#region Controller setup

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ValidateModelStateFilter));
});
builder.Services.AddEndpointsApiExplorer();

#endregion

#region Authentication setup

builder.Services.AddAuthentication()
    .AddJwtBearer("JwtBearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
        };
    });

#endregion

#region Authorization setup

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MeetingRoomAccess", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var meetingRoomClaim = context.User.FindFirst(ClaimType.Booking.ToString())?.Value;
            return meetingRoomClaim == ClaimValue.MeetingRoom.ToString();
        });
    });
});

#endregion

#region Swagger setup

builder.Services.AddSwaggerGen(options =>
{
    var basePath = AppContext.BaseDirectory;
    var fileName = typeof(Program).GetTypeInfo().Assembly.GetName().Name + ".xml";
    options.IncludeXmlComments(Path.Combine(basePath, fileName));

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter the token in the input field below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                },
            },
            new List<string>()
        }
    });
});

#endregion

#region CORS setup
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policyBuilder =>
        {
            policyBuilder.WithOrigins(builder.Configuration.GetSection("AllowedHosts").Get<string[]>()!)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowedToAllowWildcardSubdomains();
        });
});
#endregion

var app = builder.Build();

#region Hub setup
app.MapHub<NotificationHub>("/notificationHub");
#endregion

#region Database migration

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CloudReservationDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or initializing the database.");
    }
}
#endregion

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<GlobalExceptionHandler>();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program {}