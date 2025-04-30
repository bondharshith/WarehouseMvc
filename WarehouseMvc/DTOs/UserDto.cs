// ---------------------------------------------------------------------------------------------------------------------
// File: UserDto.cs
// Author: Harshith
// Created Date: 28-Apr-2025
// Description: 
//     DTO (Data Transfer Object) for representing user data in the WarehouseMvc application. 
//     This DTO is used for transferring user details, excluding sensitive data like passwords.
// ---------------------------------------------------------------------------------------------------------------------

namespace WarehouseMvc.DTOs
{
    /// <summary>
    /// Represents the data transfer object for a user, excluding sensitive information like passwords.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the role assigned to the user.
        /// The role could be "Admin" or "Employee".
        /// </summary>
        public string Role { get; set; } // Admin or Employee
    }
}
