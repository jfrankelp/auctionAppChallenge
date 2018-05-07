using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AuctionApp.Models;

namespace AuctionApp.Repositories
{
    public class AuctionContext : AuditableDbContext
    {
        public DbSet<Auction> Auctions{ get; set; }
        public DbSet<Bid> Bids { get; set; }

        public AuctionContext() : base("system") { }
        public AuctionContext(string userName, string connectionString) : base(userName, connectionString) { }
        public AuctionContext(string userName) : base(userName) { }

        // ReSharper disable once SuggestBaseTypeForParameter
        public AuctionContext(DbContextOptions<AuctionContext> options) : base(options) { }

        // ReSharper disable once SuggestBaseTypeForParameter
        public AuctionContext(DbContextOptions<AuctionContext> options, string connectionString) : base(options, connectionString) { }

        public static void EnsureSeedData(AuctionContext context)
        {
            context.Database.EnsureCreated();

            if (string.IsNullOrEmpty(context.CurrentUserName))
            {
                context.CurrentUserName = "jfrankel";
            }

            context.Auctions.Add(new Auction { ShortDescription = "Trip to Hawaii" });
            context.Auctions.Add(new Auction { ShortDescription = "Overclocked PC" });

            context.Bids.Add(CreateBid("jfrankel", 10));
            context.Bids.Add(CreateBid("jfrankel", 20));

            context.SaveChanges();
        }

        private static Bid CreateBid(string userName, float amount)
        {
            var newBid = new Bid()
            {
                UserName = userName,
                Amount = amount
            };

            return newBid;
        }
    }
}
