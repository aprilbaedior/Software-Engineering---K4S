using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using System;
using System.Threading.Tasks;

namespace CS395SI_Spring2026Group1.Pages.ManagerRequest
{
    public class RequestModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public RequestModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Spring2026_Group1_ManagerRequest ManagerRequest { get; set; }

        public void OnGet()
        {
            // nothing needed on GET
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ManagerRequest.Status = "Pending";
            ManagerRequest.RequestDate = DateTime.Now;

            _context.Spring2026_Group1_ManagerRequest.Add(ManagerRequest);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your request has been submitted!";
            return RedirectToPage("/Index");
        }
    }
}