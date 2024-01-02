using CloudReservation.DAL.Data;
using CloudReservation.DAL.Repositories;
using CloudReservation.IntegrationTest.IntegrationUtilities.Authentication;
using CloudReservation.Service.Services.HashingService;
using CloudReservation.Service.Services.TokenServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MariaDb;

namespace CloudReservation.IntegrationTest.IntegrationUtilities;

// ReSharper disable once ConvertConstructorToMemberInitializers
// ReSharper disable once ClassNeverInstantiated.Global
public class IntegrationTestFactory<TProgram, TDbContext> : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class where TDbContext : DbContext
{
    private readonly MariaDbContainer _dbContainer;

    public IntegrationTestFactory()
    {
        _dbContainer = new MariaDbBuilder().WithImage("mariadb:latest").WithCleanUp(true).Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");

        builder.ConfigureServices(services =>
        {
            #region Database setup
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<TDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<TDbContext>(options =>
            {
                options.UseMySql(_dbContainer.GetConnectionString(),
                    ServerVersion.AutoDetect(_dbContainer.GetConnectionString()));
            });
            #endregion
            
            #region Services setup
            //Add services here that are needed for the integration tests to run
            services.AddAutoMapper(typeof(Program));
            services.AddScoped<IHashService, HashService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserRepository, UserRepository>();
            #endregion



            // Override the default authentication scheme provider
            services.AddTransient<IAuthenticationSchemeProvider, MockAuthenticationSchemeProvider>();
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
       await _dbContainer.DisposeAsync();
    }
}