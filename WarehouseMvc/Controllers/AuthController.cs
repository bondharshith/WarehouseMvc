// -------------------------------------------------------------------------------
// File: AuthController.cs
// Description: Handles user authentication for the Warehouse Management System.
//              Provides user registration, login, JWT generation, and logout functionality.
// Author: [Harshith]
// Date: [29-04-2025]
// -----------------------------------------------------------------------------

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WarehouseMvc.Data;
using WarehouseMvc.DTOs;
using WarehouseMvc.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace WarehouseMvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        // Constructor to inject database context and configuration settings
        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Displays the user registration page.
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Handles user registration form submission.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterRequest dto)
        {
            if (!ModelState.IsValid)
                return View(dto); // If the model validation fails, reload the form with errors

            // Check if username already exists
            if (_context.Users.Any(u => u.Username == dto.Username))
            {
                ModelState.AddModelError(string.Empty, "Username exists");
                return View(dto);
            }

            // Create new user entity
            var user = new User
            {
                Username = dto.Username,
                Role = dto.Role
            };

            // Hash the password before saving
            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, dto.Password);

            // Add user to database and save changes
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login"); // Redirect to login page after successful registration
        }

        /// <summary>
        /// Displays the login page.
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Handles user login form submission.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest dto)
        {
            if (!ModelState.IsValid)
                return View(dto); // Reload form if input is invalid

            // Find user by username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials");
                return View(dto);
            }

            // Verify the provided password with the stored hashed password
            var hasher = new PasswordHasher<User>();
            if (hasher.VerifyHashedPassword(user, user.Password, dto.Password) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials");
                return View(dto);
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            // Set JWT token in HTTP-only cookie
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,          // Prevent JavaScript access (security)
                Secure = Request.IsHttps, // Use secure cookie if HTTPS
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddHours(1) // Set token expiration
            });

            return RedirectToAction("Index", "Product"); // Redirect to Product page after successful login
        }

        /// <summary>
        /// Generates a JWT token containing user information.
        /// </summary>
        /// <param name="user">Authenticated user</param>
        /// <returns>Signed JWT token string</returns>
        private string GenerateJwtToken(User user)
        {
            // Define claims for the token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // Create signing key from secret stored in appsettings
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:SecretKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token expires after 1 hour
                signingCredentials: creds
            );

            // Return the serialized token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Logs the user out by deleting the JWT cookie.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Delete the jwt cookie to logout the user
            Response.Cookies.Delete("jwt");

            return RedirectToAction("Index", "Home"); // Redirect to home page after logout
        }
    }
}
