using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AuctionApp.Repositories
{
    public class AuditableDbContext : DbContextBase
    {
        public bool AuditingEnabled { get; set; } = true;
        protected string CurrentUserName { get; set; }

        public AuditableDbContext(string userName, string connectionString) : base(connectionString)
        {
            this.CurrentUserName = userName;
        }

        public AuditableDbContext(string userName)
        {
            this.CurrentUserName = userName;
        }

        public AuditableDbContext(DbContextOptions options) : base(options)
        {
            if (string.IsNullOrEmpty(this.CurrentUserName))
            {
                this.CurrentUserName = this.GetType().Namespace;
            }
        }

        public AuditableDbContext(DbContextOptions options, string connectionString) : base(options, connectionString)
        {
            if (string.IsNullOrEmpty(this.CurrentUserName))
            {
                this.CurrentUserName = this.GetType().Namespace;
            }
        }

        public override int SaveChanges()
        {
            if (this.AuditingEnabled)
            {
                this.UpdateAuditableEntities();
            }
            
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (this.AuditingEnabled)
            {
                this.UpdateAuditableEntities();
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public virtual void EnsureSeedData() { }

        protected void UpdateAuditableEntities()
        {
            var addedEntryCollection = this.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added)
                .Where(x => x.Entity != null);

            var modifiedEntryCollection = this.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Modified)
                .Where(x => x.Entity != null);

            // Set audit fields of added entries
            foreach (var entry in addedEntryCollection)
            {
                if (entry.Entity is ICreationAuditableEntity addedEntity)
                {
                    addedEntity.CreatedDate = DateTime.UtcNow;
                    addedEntity.CreatedBy = this.CurrentUserName;
                }

                if (entry.Entity is IAuditableEntity auditableEntity)
                {
                    auditableEntity.ModifiedDate = DateTime.UtcNow;
                    auditableEntity.ModifiedBy = this.CurrentUserName;
                }
            }

            // Set audit fields of modified entries
            foreach (var entry in modifiedEntryCollection)
            {
                if (entry.Entity is IAuditableEntity modifiedEntity)
                {
                    modifiedEntity.ModifiedDate = DateTime.UtcNow;
                    modifiedEntity.ModifiedBy = this.CurrentUserName;
                }
            }
        }
    }
}