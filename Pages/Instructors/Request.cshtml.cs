using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CS395SI_Spring2023_Group1.Pages.Instructors
{
    [Authorize]
    public class RequestModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RequestModel(CS395SI_Spring2023_Group1Context context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        [StringLength(1000)]
        public string Speciality { get; set; }

        [BindProperty]
        [Required]
        [StringLength(500)]
        public string Justification { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            Email = user.Email;

            // Check if already an instructor
            if (User.IsInRole("Instructor"))
            {
                TempData["ErrorMessage"] = "You are already an instructor.";
                return RedirectToPage("/Index");
            }

            // Check if already has pending request
            var existingRequest = await _context.Spring2026_Group1_InstructorRequest
                .FirstOrDefaultAsync(r => r.Email == Email && r.Status == "Pending");

            if (existingRequest != null)
            {
                TempData["ErrorMessage"] = "You already have a pending instructor request.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Check if already an instructor
            if (User.IsInRole("Instructor"))
            {
                TempData["ErrorMessage"] = "You are already an instructor.";
                return RedirectToPage("/Index");
            }

            // Check if already has pending request
            var existingRequest = await _context.Spring2026_Group1_InstructorRequest
                .FirstOrDefaultAsync(r => r.Email == Email && r.Status == "Pending");

            if (existingRequest != null)
            {
                TempData["ErrorMessage"] = "You already have a pending instructor request.";
                return Page();
            }

            // Create new instructor request
            var request = new Spring2026_Group1_InstructorRequest
            {
                Email = Email,
                FullName = FullName,
                Speciality = Speciality,
                Justification = Justification,
                Status = "Pending",
                RequestDate = DateTime.UtcNow
            };

            _context.Spring2026_Group1_InstructorRequest.Add(request);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Instructor application submitted successfully! You'll be notified when it's reviewed.";
            return RedirectToPage("/Index");
        }
    }
}