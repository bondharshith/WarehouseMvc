// ---------------------------------------------------------------------------------------------------------------------
// File: IItemService.cs
// Author: Harshith
// Created Date: 28-Apr-2025
// Description: 
//     Interface for item-related services, including CRUD operations and search functionality for products
//     in the WarehouseMvc application.
// ---------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using WarehouseMvc.Models;

namespace WarehouseMvc.Services
{
    /// <summary>
    /// Defines the contract for product-related operations in the warehouse management system.
    /// </summary>
    public interface IItemService
    {
        /// <summary>
        /// Retrieves a paginated and sorted list of all products.
        /// </summary>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="sortField">Field to sort by (e.g., Name, Quantity).</param>
        /// <param name="ascending">Whether sorting should be ascending.</param>
        /// <returns>A list of products matching the criteria.</returns>
        Task<List<Product>> GetAllProducts(int pageNumber, int pageSize, string? sortField, bool ascending);

        /// <summary>
        /// Retrieves a specific product by its ID.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>The matching product if found; otherwise, null.</returns>
        Task<Product?> GetProductById(int id);

        /// <summary>
        /// Creates a new product using Dapper.
        /// </summary>
        /// <param name="product">The product to create.</param>
        Task CreateProduct(Product product); // Dapper

        /// <summary>
        /// Updates an existing product using Entity Framework LINQ.
        /// </summary>
        /// <param name="product">The product to update.</param>
        Task UpdateProduct(Product product); // EF LINQ

        /// <summary>
        /// Deletes a product by its ID using inline SQL.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        Task DeleteProduct(int id); // Inline SQL

        /// <summary>
        /// Searches for products whose names contain the given partial name.
        /// </summary>
        /// <param name="namePart">The part of the product name to search for.</param>
        /// <returns>A list of matching products.</returns>
        Task<List<Product>> SearchProductsByName(string namePart); // EF for autocomplete
    }
}
