using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AuctionApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuctionApp
{
    public abstract class Repository<TDbContext> : ReadOnlyRepository<TDbContext>, IDisposable, IRepository<TDbContext> where TDbContext : DbContext
    {
        protected Repository(IConfiguration config, TDbContext context = null, IServiceCollection services = null) 
            : base(config, context, services)
        {
            this.Context = context;
        }

        protected Repository(IConfiguration config, IServiceCollection services = null) 
            : base(config, services)
        {
        }

        #region Add, AddAsync, AddRange, and AddRangeAsync

        public TEntity Add<TEntity>(TEntity entity) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            dbSet.Add(entity);
            this.Context.SaveChanges();
            return entity;
        }

        public async Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            var result = await dbSet.AddAsync(entity);
            return entity;
        }

        public void AddRange<TEntity>(params TEntity[] entities) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            dbSet.AddRange(entities);
        }

        public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            dbSet.AddRange(entities);
        }

        public async Task AddRangeAsync<TEntity>(params TEntity[] entities) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            await dbSet.AddRangeAsync(entities);
        }

        public async Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            await dbSet.AddRangeAsync(entities);
        }

        #endregion

        #region Remove and RemoveRange

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            dbSet.Remove(entity);
        }

        public void Remove<TEntity, Tid>(Tid key) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            var entity = this.Get<TEntity, Tid>(key);
            dbSet.Remove(entity);
        }

        public void RemoveRange<TEntity>(params TEntity[] entities) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            dbSet.RemoveRange(entities);
        }

        public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            dbSet.RemoveRange(entities);
        }

        #endregion

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            dbSet.Update(entity);
        }

        public void UpdateRange<TEntity>(params TEntity[] entities) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            dbSet.UpdateRange(entities);
        }

        public void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            dbSet.UpdateRange(entities);
        }

        #region SaveChanges and SaveChangesAsync        
        
        public void SaveChanges()
        {
            this.Context.SaveChanges();
        }

        public void SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.Context.SaveChanges(acceptAllChangesOnSuccess);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await this.Context.SaveChangesAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            await this.Context.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        
        #endregion

        protected virtual void Detach<TEntity>(TEntity entity) where TEntity : class
        {
            var entry = this.Context.Entry(entity);
            entry.State = EntityState.Detached;
        }

        protected virtual void Attach<TEntity>(TEntity entity) where TEntity : class
        {
            var entry = this.Context.Attach(entity);
            entry.State = EntityState.Modified;
        }

        #region Dispose Methods

        public void Dispose()
        {
            this.Dispose(true);
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Context.Dispose();
                }
            }

            this.disposed = true;
        }

        #endregion
    }

    public abstract class RepositoryBase
    {
        private const string DefaultConnectionStringTemplate = "Server=(local);Database={0};Trusted_Connection=True;MultipleActiveResultSets=true";

        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(internalConnectionString))
                {
                    internalConnectionString = this.GetOrCreateConnectionString();
                }

                return internalConnectionString;
            }

            set => internalConnectionString = value;
        }

        private string internalConnectionString = string.Empty;

        protected virtual string GetOrCreateConnectionString()
        {
            var connectionString = string.Format(DefaultConnectionStringTemplate, this.GetType().FullName.Replace(".", ""));
            return connectionString;
        }
    }
}