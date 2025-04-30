using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using WarehouseMvc.Data;
using WarehouseMvc.Models;
using WarehouseMvc.Services;
using Xunit;

namespace WarehouseMvc.Tests
{
    public class ItemServiceTests
    {
        private readonly AppDbContext _context;
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly ItemService _itemService;

        public ItemServiceTests()
        {
            // Use REAL in-memory DB
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Fresh DB per test
                .Options;

            _context = new AppDbContext(options);

            // Seed some test data
            _context.Products.AddRange(
                new Product { Id = 1, Name = "Pen", Quantity = 10, Description = "Blue ink" },
                new Product { Id = 2, Name = "Pencil", Quantity = 20, Description = "HB" }
            );
            _context.SaveChanges();

            _mockCache = new Mock<IMemoryCache>();

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c.GetConnectionString("DefaultConnection")).Returns("FakeConnectionString");

            _itemService = new ItemService(_context, mockConfig.Object, _mockCache.Object);
        }

        [Fact]
        public async Task GetProductById_ReturnsCorrectProduct()
        {
            var product = await _itemService.GetProductById(1);
            Assert.NotNull(product);
            Assert.Equal("Pen", product.Name);
        }

        [Fact]
        public async Task UpdateProduct_UpdatesExistingProduct()
        {
            var updatedProduct = new Product { Id = 1, Name = "Pen Updated", Quantity = 30, Description = "Black ink" };
            await _itemService.UpdateProduct(updatedProduct);

            var product = await _itemService.GetProductById(1);
            Assert.Equal("Pen Updated", product.Name);
            Assert.Equal(30, product.Quantity);
        }

        [Fact]
        public async Task SearchProductsByName_ReturnsMatchingProducts()
        {
            var result = await _itemService.SearchProductsByName("Pen");
            Assert.Single(result);
            Assert.Equal("Pen", result.First().Name);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsFromCacheIfAvailable()
        {
            // Arrange
            var cacheKey = "ProductList_Page1_Size10_SortId_AscTrue";

            var cachedProducts = new List<Product> { new Product { Id = 100, Name = "CachedProduct", Quantity = 1, Description = "Cached" } };

            object outValue = cachedProducts;
            _mockCache.Setup(mc => mc.TryGetValue(cacheKey, out outValue)).Returns(true);

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c.GetConnectionString("DefaultConnection")).Returns("FakeConnectionString");

            var itemServiceWithCache = new ItemService(_context, mockConfig.Object, _mockCache.Object);

            // Act
            var result = await itemServiceWithCache.GetAllProducts(1, 10, "Id", true);

            // Assert
            Assert.Single(result);
            Assert.Equal("CachedProduct", result[0].Name);
        }
    }
}
