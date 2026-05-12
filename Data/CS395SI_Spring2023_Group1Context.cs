using CS395SI_Spring2023_K4S.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CS395SI_Spring2023_Group1.Data
{
    public class CS395SI_Spring2023_Group1Context : DbContext
    {
        public CS395SI_Spring2023_Group1Context (DbContextOptions<CS395SI_Spring2023_Group1Context> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => 
                warnings.Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning));
        }

        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2023_Group1_Profile_Sys> Spring2023_Group1_Profile_Sys { get; set; } = default!;

        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2023_Group1_Services> Spring2023_Group1_Services { get; set; } = default!;
        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2023_Group1_Scheduling_Form> Spring2023_Group1_Scheduling_Form { get; set; } = default!;
        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2023_Group1_Schedules>? Spring2023_Group1_Schedules { get; set; }
        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2024_Group2_Schedule>? Spring2024_Group2_Schedule { get; set; }
        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2026_Group1_Sections>? Spring2026_Group1_Sections { get; set; }
        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2024_Group2_Sections>? Spring2024_Group2_Sections { get; set; }
        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2024_Group2_Session>? Spring2024_Group2_Session { get; set; }

        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2025_Group3_Attendance>? Spring2025_Group3_Attendance { get; set; }

        //Spring 2026 Group 1
        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2026_Group1_Students> Spring2026_Group1_Students { get; set; }
        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2026_Group1_Instructor> Spring2026_Group1_Instructor { get; set; }
        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2026_Group1_InstructorRequest> Spring2026_Group1_InstructorRequest { get; set; }
        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2026_Group1_InstructorAssignment> Spring2026_Group1_InstructorAssignment { get; set; }
        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2026_Group1_Manager> Spring2026_Group1_Manager { get; set; }
        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2026_Group1_ManagerRequest> Spring2026_Group1_ManagerRequest { get; set; }
        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2026_Group1_Strike> Spring2026_Group1_Strike { get; set; }
        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2026_Group1_Certificate> Spring2026_Group1_Certificate { get; set; }
        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2026_Group1_EmployeeApplication> Spring2026_Group1_EmployeeApplication { get; set; } = default!;
        public DbSet<CS395SI_Spring2023_K4S.Model.Spring2026_Group1_Schedule> Spring2026_Group1_Schedule { get; set; } = default!;
    }
}
