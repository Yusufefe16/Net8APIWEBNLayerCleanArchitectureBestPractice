﻿using App.Repositories.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Repositories.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = configuration.GetSection
                (ConnectionStringOption.Key).Get<ConnectionStringOption>();
            Console.WriteLine(connectionString?.SqlServer);

            options.UseSqlServer(connectionString!.SqlServer, sqlServerOptionsAction: sqlServerOptionsAction =>
            {
                sqlServerOptionsAction.MigrationsAssembly(typeof(RepositoryAssembly).Assembly.FullName);
            });
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}