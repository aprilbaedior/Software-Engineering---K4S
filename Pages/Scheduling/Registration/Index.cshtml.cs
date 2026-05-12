using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;

namespace CS395SI_Spring2023_Group1.Pages.Registration
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public IndexModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public IList<Spring2023_Group1_Profile_Sys> Spring2023_Group1_Profile_Sys { get;set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FilterBy { get; set; }

        public async Task OnGetAsync()
        {
            if (_context.Spring2023_Group1_Profile_Sys == null)
            {
                Spring2023_Group1_Profile_Sys = new List<Spring2023_Group1_Profile_Sys>();
                return;
            }

            var students = _context.Spring2023_Group1_Profile_Sys.AsQueryable();

            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                Spring2023_Group1_Profile_Sys = await students.ToListAsync();
                return;
            }
            
            switch (FilterBy)
            {
                case "Email":
                    students = students.Where(s => s.Email.Contains(SearchQuery));
                    break;
                case "PhoneNum":
                    students = students.Where(s => s.PhoneNum.Contains(SearchQuery));
                    break;
                case "Address":
                    students = students.Where(s => s.Address.Contains(SearchQuery));
                    break;
                case "ApplicationStatus":
                    students = students.Where(s => s.ApplicationStatus.Contains(SearchQuery));
                    break;
                default: // Default is name search
                    students = students.Where(s => s.Name.Contains(SearchQuery));
                    break;
            }

            Spring2023_Group1_Profile_Sys = await students.ToListAsync();
        }



        public IActionResult OnPost(string StudentEmail)
        {
            HttpContext.Session.SetString("studentEmail", StudentEmail);
            return RedirectToPage("../../StudentSchedule/Index");
        }

    }
}

