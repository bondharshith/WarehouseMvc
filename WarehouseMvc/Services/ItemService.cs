// ---------------------------------------------------------------------------------------------------------------------
// File: ItemService.cs
// Author: Harshith
// Created Date: 28-Apr-2025
// Description:
//     Implementation of IItemService interface providing CRUD, paging, sorting, searching operations for Product entities,
//     using Dapper, Entity Framework Core, inline SQL, caching via IMemoryCache, and best practices for database access.
// ---------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WarehouseMvc.Data;
using WarehouseMvc.Models;


namespace WarehouseMvc.Services
{
    /// <summary>
    /// Service for managing product operations using Dapper, Entity Framework Core, and caching.
    /// </summary>
    public class ItemService : IItemService
    {
        private readonly AppDbContext _context;
        private readonly string _connectionString;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Initializes a new instance of the ItemService class.
        /// </summary>
        /// <param name="context">Database context to interact with Products table.</param>
        /// <param name="configuration">Configuration for reading connection strings.</param>
        /// <param name="cache">Memory cache to store frequently accessed data temporarily.</param>
        public ItemService(AppDbContext context, IConfiguration configuration, IMemoryCache cache)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _cache = cache;
        }

        /// <summary>
        /// Retrieves a paginated, sorted list of products with optional caching.
        /// </summary>
        /// <param name="pageNumber">Current page number.</param>
        /// <param name="pageSize">Number of products per page.</param>
        /// <param name="sortField">Field name to sort by.</param>
        /// <param name="ascending">Sort direction: true for ascending, false for descending.</param>
        /// <returns>List of products for the requested page.</returns>
        public async Task<List<Product>> GetAllProducts(int pageNumber, int pageSize, string? sortField, bool ascending)
        {
            // Create a cache key based on the parameters
            string cacheKey = $"ProductList_Page{pageNumber}_Size{pageSize}_Sort{sortField}_Asc{ascending}";

            // Try to get data from cache first
            if (!_cache.TryGetValue(cacheKey, out List<Product> products))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var orderBy = !string.IsNullOrEmpty(sortField) ? sortField : "Id";
                    var direction = ascending ? "ASC" : "DESC";
                    var offset = (pageNumber - 1) * pageSize;

                    // SQL query with dynamic order by and pagination
                    var query = $@"
                        SELECT * FROM Products
                        ORDER BY {orderBy} {direction}
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                    // Execute query using Dapper
                    var result = await connection.QueryAsync<Product>(query, new { Offset = offset, PageSize = pageSize });
                    products = result.ToList();

                    // Set cache entry for 5 minutes
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                    _cache.Set(cacheKey, products, cacheOptions);
                }
            }

            return products;
        }

        /// <summary>
        /// Retrieves a single product by its ID.
        /// </summary>
        /// <param name="id">Product ID to search for.</param>
        /// <returns>Product if found, otherwise null.</returns>
        public async Task<Product?> GetProductById(int id)
        {
            // Find product using EF Core
            return await _context.Products.FindAsync(id);
        }

        /// <summary>
        /// Creates a new product record in the database using Dapper.
        /// </summary>
        /// <param name="product">Product object containing the new product's details.</param>
        public async Task CreateProduct(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "INSERT INTO Products (Name, Quantity, Description) VALUES (@Name, @Quantity, @Description)";
                // Insert new product into database
                await connection.ExecuteAsync(sql, new { product.Name, product.Quantity, product.Description });
            }
        }

        /// <summary>
        /// Updates an existing product's details using EF Core and LINQ.
        /// </summary>
        /// <param name="product">Updated product object.</param>
        public async Task UpdateProduct(Product product)
        {
            // Find existing product
            var existingProduct = await _context.Products.FindAsync(product.Id);
            if (existingProduct != null)
            {
                // Update properties
                existingProduct.Name = product.Name;
                existingProduct.Quantity = product.Quantity;
                existingProduct.Description = product.Description;

                // Save changes to database
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes a product from the database using inline SQL with Dapper.
        /// </summary>
        /// <param name="id">ID of the product to delete.</param>
        public async Task DeleteProduct(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "DELETE FROM Products WHERE Id = @Id";
                // Execute delete command
                await connection.ExecuteAsync(sql, new { Id = id });
            }
        }

        /// <summary>
        /// Searches for products by name containing a partial keyword using EF Core.
        /// </summary>
        /// <param name="namePart">Partial name keyword to search for.</param>
        /// <returns>List of products matching the search.</returns>
        public async Task<List<Product>> SearchProductsByName(string namePart)
        {
            // Perform case-insensitive search and limit results to top 10
            return await _context.Products
                .Where(p => p.Name.Contains(namePart))
                .OrderBy(p => p.Name)
                .Take(10)
                .ToListAsync();
        }
    }
}
