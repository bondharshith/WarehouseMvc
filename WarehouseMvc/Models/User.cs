// ---------------------------------------------------------------------------------------------------------------------
// File: User.cs
// Author: Harshith
// Created Date: 28-Apr-2025
// Description: 
//     Represents a user entity in the WarehouseMvc application, containing user credentials and role information.
// ---------------------------------------------------------------------------------------------------------------------

namespace WarehouseMvc.Models
{
    /// <summary>
    /// Represents a user in the WarehouseMvc application.
    /// This class contains properties for user authentication and authorization.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username for the user.
        /// </summary>
        public string Username { get; set; } = null!;

        /// <summary>
        /// Gets or sets the password for the user.
        /// </summary>
        public string Password { get; set; } = null!;

        /// <summary>
        /// Gets or sets the role of the user (e.g., Admin or Employee).
        /// </summary>
        public string Role { get; set; }
    }
}
