using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CS395SI_Spring2023_Group1.Data;
using Microsoft.AspNetCore.Authorization;

namespace CS395SI_Spring2023_Group1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Instructor")]
    public class StudentSearchController : ControllerBase
    {
        private readonly CS395SI_Spring2023_Group1Context _context;

        public StudentSearchController(CS395SI_Spring2023_Group1Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAvailableStudents(int sectionID, string query = null)
        {
            // Get all students already enrolled in this section
            var enrolledStudentEmails = await _context.Spring2024_Group2_Schedule
                .Where(s => s.SectionID == sectionID)
                .Select(s => s.StudentEmail)
                .ToListAsync();
            
            // Base query to get students not enrolled in this section
            var studentsQuery = _context.Spring2023_Group1_Profile_Sys
                .Where(p => !enrolledStudentEmails.Contains(p.Email));
            
            // Apply search filter if query is provided
            if (!string.IsNullOrWhiteSpace(query))
            {
                studentsQuery = studentsQuery.Where(p => 
                    p.Name.Contains(query) || p.Email.Contains(query));
            }
            
            // Get the students
            var students = await studentsQuery
                .OrderBy(p => p.Name) // Order by name for better readability
                .Select(p => new { name = p.Name, email = p.Email })
                .ToListAsync();
            
            return students;
        }
    }
}