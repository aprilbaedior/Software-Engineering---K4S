using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using CS395SI_Spring2023_Group1.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS395SI_Spring2023_Group1.Pages.Admin
{
    [Authorize(Roles = MyConstants.ROLE_ADMIN)]
    public class ManagerRequestsModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ManagerRequestsModel(CS395SI_Spring2023_Group1Context context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Spring2026_Group1_ManagerRequest> PendingRequests { get; set; }

        public void OnGet()
        {
            PendingRequests = _context.Spring2026_Group1_ManagerRequest
                                      .Where(r => r.Status == "Pending")
                                      .OrderBy(r => r.RequestDate)
                                      .ToList();
        }

        public async Task<IActionResult> OnPostAsync(int requestId, string action)
        {
            var request = _context.Spring2026_Group1_ManagerRequest.FirstOrDefault(r => r.RequestID == requestId);
            if (request == null)
            {
                TempData["ErrorMessage"] = "Request not found.";
                return RedirectToPage();
            }

            request.Status = action.Equals("approve", StringComparison.OrdinalIgnoreCase) ? "Approved" : "Denied";
            request.ReviewDate = DateTime.Now;
            request.ReviewedBy = User.Identity.Name;

            // If approved, assign MANAGER role
            if (request.Status == "Approved")
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user != null && !await _userManager.IsInRoleAsync(user, MyConstants.ROLE_MANAGER))
                {
                    await _userManager.AddToRoleAsync(user, MyConstants.ROLE_MANAGER);
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Request {request.Status.ToLower()} successfully.";

            return RedirectToPage();
        }
    }
}