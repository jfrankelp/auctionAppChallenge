using System.Linq;
using AuctionApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionApp.Repositories
{


    public class AuctionRepository : Repository<AuctionContext>
    {
        public AuctionRepository(IConfiguration config, AuctionContext context, IServiceCollection services = null)
            : base(config, context, services)
        {
        }

        public AuctionRepository(IConfiguration config, IServiceCollection services = null) : base(config, services)
        {
        }

        public IQueryable<Auction> GetAuctions(bool trackingEnabled = false, bool loadRelatedData = false)
        {
            var objSet = this.GetDbSet<Auction>(trackingEnabled, loadRelatedData);
            return objSet;
        }

        public IQueryable<Bid> GetBids(bool trackingEnabled = false, bool loadRelatedData = false)
        {
            var objSet = this.GetDbSet<Bid>(trackingEnabled, loadRelatedData);
            return objSet;
        }

        protected override IQueryable<T> LoadRelated<T>(IQueryable<T> dbSet)
        {
            IQueryable<T> result;
            switch (dbSet)
            {
                case IQueryable<Auction> auctionSet:
                    result = (IQueryable<T>)LoadAuctionRelatedData(auctionSet);
                    break;

                case IQueryable<Bid> bidSet:
                    result = (IQueryable<T>)LoadBidRelatedData(bidSet);
                    break;

                default:
                    result = dbSet;
                    break;
            }

            return result;
        }

        private static IQueryable<Auction> LoadAuctionRelatedData(IQueryable<Auction> dbSet)
        {
            dbSet.Include(o => o.Bids);
            return dbSet;
        }

        private static IQueryable<Bid> LoadBidRelatedData(IQueryable<Bid> dbSet)
        {
            dbSet.Include(o => o.Auction);
            return dbSet;
        }
    }
}