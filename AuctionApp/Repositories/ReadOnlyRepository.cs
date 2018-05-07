using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuctionApp.Repositories
{
    public abstract class ReadOnlyRepository<TDbContext> : RepositoryBase, IReadOnlyRepository where TDbContext : DbContext
    {
        public IConfiguration Configuration { get; set; }
        public IServiceProvider Services { get; set; }
        protected ILogger Logger { get; set; }

        private TDbContext _context;

        protected ReadOnlyRepository(IConfiguration config, TDbContext context = null, IServiceCollection services = null)
            : this(config, services)
        {
            this._context = context;
        }

        protected ReadOnlyRepository(IConfiguration config, IServiceCollection services = null)
        {
            var servicesCollection = services ?? new ServiceCollection();
            this.ConfigureServices(servicesCollection, config);
        }

        private void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            this.Services = services.BuildServiceProvider();
            this.Configuration = config;

            var loggerFactory = this.Services.GetService<ILoggerFactory>();
            this.Logger = (loggerFactory ?? new LoggerFactory()).CreateLogger(this.GetType());
            this.Logger.LogInformation("Logging for {0} Initialized", this.GetType());
        }

        /// <summary>
        /// Gets or sets DbContext.
        /// </summary>
        public TDbContext Context
        {
            get => _context ?? (_context = this.Services?.GetRequiredService<TDbContext>());
            set => _context = Guard.NotNull(value, nameof(value));
        }

        #region Connection String Discovery

        protected override string GetOrCreateConnectionString()
        {
            // First try and read it from the config
            var keyName = this.GetConnectionStringKeyName();
            var connectionString = this.Configuration?.GetConnectionString(keyName);
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = base.GetOrCreateConnectionString();
            }

            return connectionString;
        }

        protected virtual string GetConnectionStringKeyName()
        {
            var keyName = this.GetType().FullName.Replace(".", "");
            return keyName;
        }

        #endregion

        protected abstract IQueryable<T> LoadRelated<T>(IQueryable<T> dbSet);

        public virtual IQueryable<T> GetDbSet<T>(bool trackingEnabled = false, bool loadRelatedData = false) where T : class
        {
            var options = trackingEnabled ? LoadOptions.TrackingEnabled : LoadOptions.TrackingDisabled;
            options |= loadRelatedData ? LoadOptions.LoadRelatedData : options;

            var dbSet = this.Context.Set<T>();
            var setWithOptions = options.HasFlag(LoadOptions.TrackingEnabled) ? dbSet : dbSet.AsNoTracking<T>();
            var result = setWithOptions;

            if (options.HasFlag(LoadOptions.LoadRelatedData))
            {
                result = LoadRelated(setWithOptions);
            }

            return result;
        }

        public virtual TEntity Get<TEntity, TId>(TId key) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            var entity = dbSet?.Find(key);
            return entity;
        }

        public virtual async Task<TEntity> GetAsync<TEntity, TId>(TId key) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            var entity = dbSet != null ? await dbSet.FindAsync(key) : null;
            return entity;
        }

        public virtual bool TryGet<TEntity, TId>(TId key, out TEntity entity) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            entity = dbSet?.Find(key);
            return (entity != null);
        }

        public virtual async Task<(bool, TEntity)> TryGetAsync<TEntity, TId>(TId key) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            var entity = dbSet != null ? await dbSet.FindAsync(key) : null;
            var entityFound = (entity != null);
            return (entityFound, entity);
        }

        public TEntity Find<TEntity, TId>(TId key) where TEntity : class
        {
            return this.Get<TEntity, TId>(key);
        }

        public IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            var results = dbSet.Where(filter);
            return results;
        }

        public async Task<TEntity> FindAsync<TEntity, TId>(TId key) where TEntity : class
        {
            return await this.GetAsync<TEntity, TId>(key);
        }

        public async Task<IQueryable<TEntity>> FindAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            var dbSet = this.Context.Set<TEntity>();
            var results = dbSet.Where(filter);
            return await Task.FromResult(results);
        }

        public bool KeyExists<TEntity, TId>(TId key) where TEntity : class
        {
            var result = TryGet(key, out TEntity _);
            return result;
        }

        public async Task<bool> KeyExistsAsync<TEntity, TId>(TId key) where TEntity : class
        {
            var (keyFound, _) = await TryGetAsync<TEntity, TId>(key);
            return keyFound;
        }
    }
}