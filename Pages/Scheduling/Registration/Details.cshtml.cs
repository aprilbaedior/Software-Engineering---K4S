using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;

namespace CS395SI_Spring2023_Group1.Pages.Registration
{
    public class DetailsModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public DetailsModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public Spring2023_Group1_Profile_Sys Spring2023_Group1_Profile_Sys { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.Spring2023_Group1_Profile_Sys == null)
            {
                return NotFound();
            }

            var spring2023_group1_profile_sys = await _context.Spring2023_Group1_Profile_Sys.FirstOrDefaultAsync(m => m.Email == id);
            if (spring2023_group1_profile_sys == null)
            {
                return NotFound();
            }
            else 
            {
                Spring2023_Group1_Profile_Sys = spring2023_group1_profile_sys;
            }
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            string studentId = Request.Form["StudentId"];
            string status = Request.Form["ApplicationStatus"];
            
            if (string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(status))
            {
                return BadRequest();
            }
            
            var profile = await _context.Spring2023_Group1_Profile_Sys.FirstOrDefaultAsync(m => m.Email == studentId);
            
            if (profile == null)
            {
                return NotFound();
            }
            
            profile.ApplicationStatus = status;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(studentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Details", new { id = studentId });
        }
        
        private bool ProfileExists(string id)
        {
            return (_context.Spring2023_Group1_Profile_Sys?.Any(e => e.Email == id)).GetValueOrDefault();
        }
    }
}