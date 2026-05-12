using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;

namespace CS395SI_Spring2023_Group1.Pages_Certificates
{
    [Authorize(Roles = "Manager")]
    public class EditModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public EditModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Spring2026_Group1_Certificate Spring2026_Group1_Certificate { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spring2026_group1_certificate =  await _context.Spring2026_Group1_Certificate.FirstOrDefaultAsync(m => m.CertificateID == id);
            if (spring2026_group1_certificate == null)
            {
                return NotFound();
            }
            Spring2026_Group1_Certificate = spring2026_group1_certificate;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Spring2026_Group1_Certificate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Spring2026_Group1_CertificateExists(Spring2026_Group1_Certificate.CertificateID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool Spring2026_Group1_CertificateExists(int id)
        {
            return _context.Spring2026_Group1_Certificate.Any(e => e.CertificateID == id);
        }
    }
}
