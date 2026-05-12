using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient; 

namespace CS395SI_Spring2023_Group1.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;

        public string? UserDisplayName { get; set; } 

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            UserDisplayName = null; 
        }

        public void OnGet()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                string userEmail = User.Identity.Name ?? string.Empty;
                
                if (!User.IsInRole("Admin"))
                {
                    if (HttpContext.Session != null)
                    {
                        HttpContext.Session.SetString("studentEmail", userEmail);
                    }
                    
                    try
                    {
                        string connectionString = _configuration.GetConnectionString("CS395SI_Spring2023_Group1Context");
                        if (string.IsNullOrEmpty(connectionString))
                        {
                            throw new InvalidOperationException("Connection string 'CS395SI_Spring2023_Group1Context' not found.");
                        }
                        
                        using (var connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            var command = new SqlCommand(
                                @"SELECT TOP 1 [Name], [ApplicationStatus] 
                                FROM [dbo].[Spring2023_Group1_Profile_Sys] 
                                WHERE [Email] = @Email", 
                                connection);
                            command.Parameters.AddWithValue("@Email", userEmail);
                            
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Get the user's name for display
                                    UserDisplayName = reader["Name"].ToString();
                                    
                                    string? applicationStatus = reader["ApplicationStatus"].ToString();
                                    ViewData["IsUserApproved"] = (applicationStatus == "Approved");
                                }
                                else
                                {
                                    UserDisplayName = userEmail;
                                    ViewData["IsUserApproved"] = false;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error retrieving user information for {Email}", userEmail);
                        UserDisplayName = userEmail;
                        
                        if (userEmail.Contains("@"))
                        {
                            string nameFromEmail = userEmail.Split('@')[0];
                            if (!string.IsNullOrEmpty(nameFromEmail))
                            {
                                UserDisplayName = char.ToUpper(nameFromEmail[0]) + nameFromEmail.Substring(1).ToLower();
                            }
                        }
                        
                        ViewData["IsUserApproved"] = true; 
                    }
                }
                else
                {
                    UserDisplayName = userEmail;
                    ViewData["IsUserApproved"] = true; // Admins are always "approved"
                }
            }
        }
    }
}