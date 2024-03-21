using Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure
{
    public class StoreContext: IdentityDbContext<User>
    {
        public StoreContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Requirement> Requirements { get; set; }
        public DbSet<Learning> Learnings { get; set; }
        public DbSet<Basket> Basket { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        //
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // This is how you can write the configuration in this file (For Learning and Requirement table)

            // builder.Entity<Learning>()
            // .HasOne(U => U.Course)
            // .WithMany(a => a.Learnings)
            // .HasForeignKey(aa => aa.CourseId)
            // .OnDelete(DeleteBehavior.Cascade);

            // builder.Entity<Requirement>()
            // .HasOne(U => U.Course)
            // .WithMany(a => a.Requirements)
            // .HasForeignKey(aa => aa.CourseId)
            // .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Entity<IdentityRole>()
           .HasData(
               new IdentityRole { Name = "Student", NormalizedName = "STUDENT" },
               new IdentityRole { Name = "Instructor", NormalizedName = "INSTRUCTOR" });

            builder.Entity<UserCourse>()
                  .HasKey(uc => new { uc.UserId, uc.CourseId });

            builder.Entity<UserCourse>()
            .HasOne(uc => uc.User)
            .WithMany(u => u.UserCourses)
            .HasForeignKey(uc => uc.UserId);

            builder.Entity<UserCourse>()
           .HasOne(uc => uc.Course)
           .WithMany(c => c.UserCourses)
           .HasForeignKey(uc => uc.CourseId);

        }

    }
}
