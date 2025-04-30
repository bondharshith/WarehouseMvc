// ---------------------------------------------------------------------------------------------------------------------
// File: LoginRequest.cs
// Author: Harshith
// Created Date: 28-Apr-2025
// Description: 
//     DTO (Data Transfer Object) for handling user login requests in the WarehouseMvc application.
// ---------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace WarehouseMvc.DTOs
{
    /// <summary>
    /// Represents the login request payload containing user credentials.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the username entered by the user.
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password entered by the user.
        /// Password must have a minimum length of 6 characters.
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
    }
}
