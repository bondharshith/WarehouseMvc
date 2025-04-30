// ---------------------------------------------------------------------------------------------------------------------
// File: Product.cs
// Author: Harshith
// Created Date: 28-Apr-2025
// Description: 
//     Represents a product model in the WarehouseMvc application. This model is used to store product information
//     such as name, quantity, and description, which is then displayed or managed within the warehouse system.
// ---------------------------------------------------------------------------------------------------------------------

namespace WarehouseMvc.Models
{
    /// <summary>
    /// Represents a product in the WarehouseMvc application.
    /// This class is used for storing product information like Id, Name, Quantity, and Description.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Gets or sets the unique identifier for the product.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the quantity of the product available in stock.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets a description of the product.
        /// </summary>
        public string Description { get; set; } = null!;
    }
}
