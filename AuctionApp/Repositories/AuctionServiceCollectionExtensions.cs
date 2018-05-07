using System;
using System.Collections.Generic;
using System.Text;
using AuctionApp;
using AuctionApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up essential CEA Identity Management in an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
    /// </summary>
    public static class AuctionServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Auction Repository to the specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add services to.</param>
        public static void AddAuctionRepository(this IServiceCollection services)
        {
            Guard.NotNull(services, nameof(services));

            var serviceProvider = services.BuildServiceProvider();
            var config = serviceProvider.GetService<IConfiguration>();

            services.AddDbContext<AuctionContext>(options => options
                .UseSqlServer(config.GetConnectionString("Auctions")));

            services.AddScoped<AuctionRepository, AuctionRepository>();
        }
    }
}
