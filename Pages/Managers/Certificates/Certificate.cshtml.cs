using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS395SI_Spring2023_Group1.Pages.Manager.Certificates
{
    [Authorize(Roles = "Manager,Admin,Student")]
    public class CertificateModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public CertificateModel(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        public Spring2026_Group1_Certificate Certificate { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var cert = await _context.Spring2026_Group1_Certificate!
                .FirstOrDefaultAsync(c => c.CertificateID == id);

            if (cert == null)
            {
                TempData["ErrorMessage"] = "Certificate not found.";
                return RedirectToPage("./Index");
            }

            Certificate = cert;
            return Page();
        }
    }
}
