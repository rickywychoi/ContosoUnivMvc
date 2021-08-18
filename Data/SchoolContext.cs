using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Course>().ToTable("Course");
            builder.Entity<Enrollment>().ToTable("Enrollment");
            builder.Entity<Student>().ToTable("Student");
            builder.Entity<Department>().ToTable("Department");
            builder.Entity<Instructor>().ToTable("Instructor");
            builder.Entity<OfficeAssignment>().ToTable("OfficeAssignment");
            builder.Entity<CourseAssignment>().ToTable("CourseAssignment");

            // Composite PK for CourseAssignment
            // Not sure if this is needed since CourseAssignment already has CourseId and InstructorId as its properties
            builder.Entity<CourseAssignment>()
                .HasKey(c => new { c.CourseId, c.InstructorId });
        }
    }
}
