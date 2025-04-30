// -------------------------------------------------------------------------------
// File: ProductController.cs
// Description: Controller for handling CRUD operations, search, and listing of products 
//              in the Warehouse Management System. Includes role-based authorization.
// Author: Harshith
// Date: 28-04-2025
// -----------------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WarehouseMvc.Models;
using WarehouseMvc.Services;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Linq;

namespace WarehouseMvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly IItemService _itemService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IItemService itemService, ILogger<ProductController> logger)
        {
            _itemService = itemService;
            _logger = logger;
        }

        /// <summary>
        /// Displays a paginated and sortable list of products.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1, string sortField = "Id", bool ascending = true)
        {
            _logger.LogInformation("Loading product index page. Page: {PageNumber}, Sort: {SortField}, Ascending: {Ascending}",
                pageNumber, sortField, ascending);

            const int pageSize = 5;
            var products = await _itemService.GetAllProducts(pageNumber, pageSize, sortField, ascending);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.SortField = sortField;
            ViewBag.Ascending = ascending;

            return View(products);
        }

        /// <summary>
        /// Displays the details of a specific product by ID.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Requesting details for product ID: {ProductId}", id);

            var product = await _itemService.GetProductById(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found with ID: {ProductId}", id);
                return NotFound();
            }

            return View(product);
        }

        /// <summary>
        /// Displays the product creation page (Admin only).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            _logger.LogInformation("Admin user accessing product creation page");
            return View();
        }

        /// <summary>
        /// Handles product creation form submission (Admin only).
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _itemService.CreateProduct(product);
                    _logger.LogInformation("New product created: {ProductName} (ID: {ProductId})", product.Name, product.Id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating product: {ProductName}", product.Name);
                    ModelState.AddModelError("", "An error occurred while creating the product.");
                }
            }
            else
            {
                _logger.LogWarning("Invalid model state for product creation");
            }

            return View(product);
        }

        /// <summary>
        /// Displays the edit page for a specific product.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Requesting edit page for product ID: {ProductId}", id);

            var product = await _itemService.GetProductById(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found for editing: {ProductId}", id);
                return NotFound();
            }

            return View(product);
        }

        /// <summary>
        /// Handles edit form submission for a product.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                _logger.LogWarning("ID mismatch in product edit. Route ID: {RouteId}, Product ID: {ProductId}", id, product.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _itemService.UpdateProduct(product);
                    _logger.LogInformation("Product updated: {ProductName} (ID: {ProductId})", product.Name, product.Id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating product ID: {ProductId}", product.Id);
                    ModelState.AddModelError("", "An error occurred while updating the product.");
                }
            }
            else
            {
                _logger.LogWarning("Invalid model state for product edit (ID: {ProductId})", product.Id);
            }

            return View(product);
        }

        /// <summary>
        /// Displays a confirmation page before deleting a product (Admin only).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Admin requesting delete confirmation for product ID: {ProductId}", id);

            var product = await _itemService.GetProductById(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found for deletion: {ProductId}", id);
                return NotFound();
            }

            return View(product);
        }

        /// <summary>
        /// Handles the confirmation of product deletion (Admin only).
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _itemService.DeleteProduct(id);
                _logger.LogInformation("Product deleted: ID {ProductId}", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product ID: {ProductId}", id);
                return RedirectToAction(nameof(Delete), new { id, error = true });
            }
        }

        /// <summary>
        /// Searches products by a partial name match.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Search(string namePart)
        {
            _logger.LogInformation("Searching products with name containing: {SearchTerm}", namePart);

            if (string.IsNullOrWhiteSpace(namePart))
            {
                return RedirectToAction(nameof(Index));
            }

            var products = await _itemService.SearchProductsByName(namePart);
            return View(products); // Assumes Search.cshtml is in Views/Product/
        }

        /// <summary>
        /// Returns a list of matching product names for autocomplete functionality.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Autocomplete(string term)
        {
            var products = await _itemService.SearchProductsByName(term);
            var results = products.Select(p => p.Name).ToList();
            return Json(results);
        }
    }
}
