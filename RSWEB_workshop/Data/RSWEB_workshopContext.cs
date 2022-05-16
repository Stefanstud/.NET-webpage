#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RSWEB_workshop.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using RSWEB_workshop.Areas.Identity.Data;

namespace RSWEB_workshop.Data
{
    public class RSWEB_workshopContext : IdentityDbContext<RSWEB_workshopUser>
    {
        public RSWEB_workshopContext (DbContextOptions<RSWEB_workshopContext> options)
            : base(options)
        {
        }

        public DbSet<RSWEB_workshop.Models.Course> Course { get; set; }

        public DbSet<RSWEB_workshop.Models.Student> Student { get; set; }

        public DbSet<RSWEB_workshop.Models.Teacher> Teacher { get; set; }

        public DbSet<RSWEB_workshop.Models.Enrollment> Enrollment { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Enrollment>()
            .HasOne<Course>(p => p.Course)
            .WithMany(p => p.StudentsEnrolled)
            .HasForeignKey(p => p.CourseId);
            //.HasPrincipalKey(p => p.Id);
            builder.Entity<Enrollment>()
            .HasOne<Student>(p => p.Student)
            .WithMany(p => p.CoursesEnrolled)
            .HasForeignKey(p => p.StudentId);
            //.HasPrincipalKey(p => p.Id);

            builder.Entity<Course>()
            .HasOne<Teacher>(p => p.Teacher1)
            .WithMany(p => p.Courses1)
            .HasForeignKey(p => p.FirstTeacherId);
            //.HasPrincipalKey(p => p.Id);
            builder.Entity<Course>()
           .HasOne<Teacher>(p => p.Teacher2)
           .WithMany(p => p.Courses2)
           .HasForeignKey(p => p.SecondTeacherId);
            //.HasPrincipalKey(p => p.Id);
        }
    }
}
