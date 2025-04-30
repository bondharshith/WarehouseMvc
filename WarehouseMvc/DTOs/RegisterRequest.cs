// ---------------------------------------------------------------------------------------------------------------------
// File: RegisterRequest.cs
// Author: Harshith
// Created Date: 28-Apr-2025
// Description: 
//     DTO (Data Transfer Object) for handling user registration requests in the WarehouseMvc application.
// ---------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace WarehouseMvc.DTOs
{
    /// <summary>
    /// Represents the registration request payload containing user details for account creation.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Gets or sets the username for the new account.
        /// Username must be at least 4 characters long.
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [MinLength(4, ErrorMessage = "Username must be at least 4 characters")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password for the new account.
        /// Password must be at least 6 characters long.
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the role assigned to the new user.
        /// Role must be either "Admin" or "Employee".
        /// </summary>
        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(Admin|Employee)$", ErrorMessage = "Role must be either 'Admin' or 'Employee'")]
        public string Role { get; set; }
    }
}
