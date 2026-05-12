/**
 * User Roles Management JavaScript
 * 
 * This script provides interactive functionality for the User Roles management interface.
 * It handles DataTables initialization, form submissions, and user interactions.
 */

/**
 * Initialize functionality when the DOM is fully loaded
 */
$(document).ready(function() {
    initializeDataTable();
    setupRoleFilterAutoSubmit();
    setupConfirmationDialogs();
});

/**
 * Initialize DataTables with enhanced features for better user experience
 */
function initializeDataTable() {
    $('#usersTable').DataTable({
        // Set default display length and available options
        "pageLength": 10,
        "lengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
        
        // Default sorting by name (first column) ascending
        "order": [[0, "asc"]],
        
        // Enable responsive behavior for better mobile experience
        "responsive": true,
        
        // Customize the DOM layout for the DataTable
        "dom": '<"top"lf>rt<"bottom"ip>',
        
        // Customize text labels
        "language": {
            "search": "Quick filter:",
            "lengthMenu": "Show _MENU_ users per page",
            "info": "Showing _START_ to _END_ of _TOTAL_ users"
        },
        
        // Define column widths for better layout
        "columnDefs": [
            { "width": "20%", "targets": 0 }, // Name column
            { "width": "20%", "targets": 1 }, // Email column
            { "width": "15%", "targets": 2 }, // Current Role column
            { "width": "25%", "targets": 3 }, // Change Role column
            { "width": "20%", "targets": 4 }  // Actions column
        ]
    });
}

/**
 * Set up automatic form submission when role filter changes
 */
function setupRoleFilterAutoSubmit() {
    $('#roleFilterSelect').on('change', function() {
        $(this).closest('form').submit();
    });
}

/**
 * Set up confirmation dialogs for sensitive actions
 */
function setupConfirmationDialogs() {
    // Confirmation dialog for password reset
    $('.reset-password-form').on('submit', function(e) {
        if (!confirm('Are you sure you want to reset this user\'s password?')) {
            e.preventDefault();
            return false;
        }
        return true;
    });
    
    // Confirmation dialog for user deletion with strong warning
    $('.delete-user-form').on('submit', function(e) {
        if (!confirm('Are you sure you want to delete this user? This action cannot be undone.')) {
            e.preventDefault();
            return false;
        }
        return true;
    });
}