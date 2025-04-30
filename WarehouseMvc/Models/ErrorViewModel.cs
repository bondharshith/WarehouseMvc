// ---------------------------------------------------------------------------------------------------------------------
// File: ErrorViewModel.cs
// Author: Harshith
// Created Date: 28-Apr-2025
// Description: 
//     Represents the error view model used to display error details in the WarehouseMvc application. 
//     This model is used for passing error information like request ID to the view.
// ---------------------------------------------------------------------------------------------------------------------

namespace WarehouseMvc.Models
{
    /// <summary>
    /// Represents the error details to be displayed in the error view of the application.
    /// Contains a RequestId and a property to determine if the RequestId should be shown.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Gets or sets the unique request identifier for the error.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Determines if the RequestId should be displayed based on whether it is null or empty.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
