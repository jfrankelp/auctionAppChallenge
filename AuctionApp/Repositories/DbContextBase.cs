using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AuctionApp.Repositories
{
    public abstract class DbContextBase : DbContext
    {
        public string ConnectionString { get; set; } = string.Empty;
        public Guid InstanceId { get; } = Guid.NewGuid();

        protected DbContextBase() { }

        protected DbContextBase(DbContextOptions options) : base(options) { }

        protected DbContextBase(string connectionString) : base()
        {
            this.ConnectionString = connectionString;
        }

        protected DbContextBase(DbContextOptions options, string connectionString) : base(options)
        {
            this.ConnectionString = connectionString;
        }
    }
}