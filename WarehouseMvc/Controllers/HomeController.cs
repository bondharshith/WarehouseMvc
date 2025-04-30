// -------------------------------------------------------------------------------
// File: HomeController.cs
// Description: Basic controller for landing page of Warehouse Management System.
//              It handles navigation to the home (Index) view.
// Author: Harshith
// Date: 28-04-2025
// -----------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

namespace WarehouseMvc.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Displays the home page (Index view).
        /// </summary>
        /// <returns>Returns the Index view.</returns>
        public IActionResult Index()
        {
            return View();  // Render the "Index.cshtml" view located in Views/Home/
        }
    }
}
