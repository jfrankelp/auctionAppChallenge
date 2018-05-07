using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AuctionApp.Repositories
{
    public interface IReadOnlyRepository
    {
        IQueryable<T> GetDbSet<T>(bool trackingEnabled = false, bool loadRelatedData = false) where T : class;

        TEntity Get<TEntity, TId>(TId key) where TEntity : class;
        Task<TEntity> GetAsync<TEntity, TId>(TId key) where TEntity : class;

        bool TryGet<TEntity, TId>(TId id, out TEntity entity) where TEntity : class;
        Task<(bool, TEntity)> TryGetAsync<TEntity, TId>(TId id) where TEntity : class;

        TEntity Find<TEntity, TId>(TId key) where TEntity : class;
        IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        Task<TEntity> FindAsync<TEntity, TId>(TId key) where TEntity : class;
        Task<IQueryable<TEntity>> FindAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;

        bool KeyExists<TEntity, TId>(TId key) where TEntity : class;
        Task<bool> KeyExistsAsync<TEntity, TId>(TId key) where TEntity : class;
    }

    public interface IRepository : IReadOnlyRepository
    {
        TEntity Add<TEntity>(TEntity entity) where TEntity : class;
        Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : class;

        void AddRange<TEntity>(params TEntity[] entities) where TEntity : class;
        void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        Task AddRangeAsync<TEntity>(params TEntity[] entities) where TEntity : class;
        Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        void Remove<TEntity>(TEntity entity) where TEntity : class;
        void RemoveRange<TEntity>(TEntity[] entities) where TEntity : class;
        void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        void Update<TEntity>(TEntity entity) where TEntity : class;
        void UpdateRange<TEntity>(params TEntity[] entities) where TEntity : class;
        void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        void SaveChanges();
        void SaveChanges(bool acceptAllChangesOnSuccess);
        Task SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IRepository<out TDbContext> : IRepository where TDbContext : DbContext
    {
        TDbContext Context { get; }
    }
}