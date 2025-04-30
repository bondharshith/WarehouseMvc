using Microsoft.EntityFrameworkCore;
using WarehouseMvc.Models;

namespace WarehouseMvc.Data
{
    /// <summary>
    /// The database context for the WarehouseMvc application.
    /// Handles interaction with the database using Entity Framework Core.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the Users table.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the Products table.
        /// </summary>
        public DbSet<Product> Products { get; set; }
    }
}
