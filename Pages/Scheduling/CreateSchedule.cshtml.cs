using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CS395SI_Spring2023_Group1.Data;
using CS395SI_Spring2023_K4S.Model;
using Microsoft.AspNetCore.Authorization;

namespace CS395SI_Spring2023_Group1.Pages.Scheduling
{
    [Authorize]
    public class CreateScheduleModel : PageModel
    {
        private readonly CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context _context;

        public CreateScheduleModel(CS395SI_Spring2023_Group1.Data.CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string ServiceID { get; set; }


        [BindProperty]
        public Spring2023_Group1_Scheduling_Form Spring2023_Group1_Scheduling_Form { get; set; }


        public IActionResult OnGet()
        {  
            if ( String.IsNullOrEmpty( ServiceID ) ) 
                return RedirectToPage("../Services/ServiceListing");


            Spring2023_Group1_Scheduling_Form.ServiceID = ServiceID;
            string userEmail = HttpContext.Session.GetString("userEmail");

            if ( string.IsNullOrEmpty( userEmail ) )
            {
                return RedirectToPage( "../Services/ServiceListing" );
            }

            Spring2023_Group1_Scheduling_Form.Email = userEmail;

            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Spring2023_Group1_Scheduling_Form == null || Spring2023_Group1_Scheduling_Form == null)
            {
                return Page();
            }

            _context.Spring2023_Group1_Scheduling_Form.Add(Spring2023_Group1_Scheduling_Form);
            await _context.SaveChangesAsync();

            return RedirectToPage("../Index");
        }
    }
}
